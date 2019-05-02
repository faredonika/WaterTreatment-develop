using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Models;
using System.Linq;
using WaterTreatment.Web.Services;

namespace WaterTreatment.Web.Controllers {

    public class AccountController : BaseController {
        private UserManager<User, int> userManager;
        private IEmailService _emailService;

        public AccountController(UserManager<User, int> userManager, WTContext context, IEmailService emailService)
            : base(context) {
            this.userManager = userManager;
            _emailService = emailService;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            if (ModelState.IsValid) {
                var invalidCredentialsMessage = "Invalid username or password.";
                var lockoutMinutes = userManager.DefaultAccountLockoutTimeSpan.Minutes;
                var lockedoutMessage = string.Format("Your account is currently locked due to multiple failed login attempts.  Please wait until the lockout expires ({0} minutes from when you were locked out) before attempting to log in again.",
                    lockoutMinutes);
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user != null) {
                    var userId = user.Id;
                    var validUser = await userManager.FindAsync(model.UserName, model.Password);
                    if (!user.IsActive) {
                        ModelState.AddModelError("", invalidCredentialsMessage);
                    } else if (await userManager.IsLockedOutAsync(userId)) {   // Regardless of whether their credentials are valid, if they attempt to login while they're locked out, they'll get the locked out error
                        ModelState.AddModelError("", lockedoutMessage);
                    } else if (validUser == null) {
                        if (await userManager.GetLockoutEnabledAsync(userId)) {
                            await userManager.AccessFailedAsync(userId);    // Increment the AccessFailedCount
                            string message;
                            if (await userManager.IsLockedOutAsync(userId)) {
                                message = lockedoutMessage;
                            } else {
                                var accessFailedCount = await userManager.GetAccessFailedCountAsync(userId);
                                var attemptsLeft = userManager.MaxFailedAccessAttemptsBeforeLockout - accessFailedCount;
                                message = string.Format("Invalid credentials.  You have {0} more attempts to log in before your account is locked for {1} minutes.", attemptsLeft, lockoutMinutes);
                            }
                            ModelState.AddModelError("", message);
                        } else {
                            ModelState.AddModelError("", invalidCredentialsMessage);
                        }
                    } else {    // not locked out and valid credentials
                        var identity = await userManager.CreateIdentityAsync(validUser, DefaultAuthenticationTypes.ApplicationCookie);
                        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
                        await userManager.ResetAccessFailedCountAsync(userId);
                        return RedirectToLocal(returnUrl);
                    }
                } else {
                    ModelState.AddModelError("", invalidCredentialsMessage);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Logout() {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword() {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            var user = userManager.FindByEmail(email);
            if (user == null)
            {
                ModelState.AddModelError("", string.Format("No account with email address \"{0}\" exists.", email));
                return View();
            }

            var code = KeyGenerator.GetUniqueKey(128);

            user.ResetCode = code;
            user.ResetCodeExpiration = DateTime.UtcNow.AddHours(24);
            userManager.Update(user);

            string resetUrl = string.Format("https://" + Request.Url.Host + "/Account/ResetPassword?code={0}&email={1}", code, email);
            string body = string.Format("Please click <a href=\"{0}\">here</a> to reset your password. This link will expire in 24 hours.", resetUrl);

            _emailService.Send(email, "Water Treatment Password Reset", body);

            TempData["CreateSuccess"] = string.Format("A link to reset your password has been sent to {0}.", email);

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string email, string code)
        {
            var user = userManager.FindByEmail(email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user.ResetCode != code)
            {
                TempData["Error"] = "Reset password link expired";
                return RedirectToAction("Login");
            }

            if ((DateTime.UtcNow > user.ResetCodeExpiration.Value))
            {
                user.ResetCodeExpiration = null;
                user.ResetCode = null;
                userManager.Update(user);

                TempData["Error"] = "Reset password link expired";
                return RedirectToAction("Login");
            }

            return View(new ResetPasswordModel()
            {
                Email = email,
                Code = code
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.FindByEmail(model.Email);
                if (user == null || user.ResetCode != model.Code)
                {
                    return RedirectToAction("Login");
                }

                if (DateTime.UtcNow > user.ResetCodeExpiration.Value)
                {
                    user.ResetCodeExpiration = null;
                    user.ResetCode = null;
                    userManager.Update(user);

                    TempData["Error"] = "Reset password link expired";
                    return RedirectToAction("Login");
                }

                user.PasswordHash = userManager.PasswordHasher.HashPassword(model.Password);
                user.ResetCode = null;
                user.ResetCodeExpiration = null;
                userManager.Update(user);

                TempData["CreateSuccess"] = string.Format("Successfully reset password for {0}.", model.Email);
                return RedirectToAction("Login");
            }

            return View(model);
        }

        public ActionResult UserProfile() {
            var user = context.Users.Find(CurrentUser.Id);

            var model = new UserEditModel
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("UserProfile")]
        public ActionResult UpdateUserProfile(UserEditModel model)
        {
            var nameExists = userManager.Users.Any(u => u.UserName == model.Username && u.Id != model.Id);
            var emailExists = userManager.Users.Any(u => u.Email == model.Email && u.Id != model.Id);

            if (ModelState.IsValid && !nameExists && !emailExists)
            {
                var user = userManager.FindById(model.Id);

                user.UserName = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;

                if (model.Password != null)
                {
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(model.Password);
                }
                
                userManager.Update(user);

                return Redirect("/");
            }
            
            if (nameExists)
            {
                ModelState.AddModelError("", string.Format("{0} already exists.  Please choose a unique username.", model.Username));
            }
            if (emailExists) {
                ModelState.AddModelError("", string.Format("There is a different account already associated with the email {0}.", model.Email));
            }

            return View(model);
        }

        [HttpGet]
        [ActionName("Eula")]
        public ActionResult GetEula() {
            return View("EULA");
        }

        [HttpPost]
        [ActionName("AgreeToEula")]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> AgreeToEula() {
            var user = context.Users.Find(CurrentUser.Id);
            if (user != null) {
                user.EulaAgreedOn = DateTime.UtcNow;
                await userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}