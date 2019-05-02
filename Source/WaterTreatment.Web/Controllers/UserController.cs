using jsTree3.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using WaterTreatment.Web.Attributes;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Extensions;
using WaterTreatment.Web.Models;
using WaterTreatment.Web.Services;

namespace WaterTreatment.Web.Controllers
{

    [SectionAuthorize]
    public class UserController : BaseController
    {

        private readonly UserManager<User, int> _userManager;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public UserController(UserManager<User, int> userManager, WTContext context, IEmailService emailService, IUserService userService)
            : base(context)
        {
            _userManager = userManager;
            _emailService = emailService;
            _userService = userService;
        }

        [Route("Index")]
        [Route("View")]
        public ViewResult Index()
        {
            ViewBag.Vendors = JsonConvert.SerializeObject(context.Vendors.OrderBy(v => v.Name).Select(v => new VendorModel { Id = v.Id, Name = v.Name }).ToList());
            ViewBag.Sites = JsonConvert.SerializeObject(context.Sites.OrderBy(s => s.Name).ToList());

            ViewBag.ManagedSites = JsonConvert.SerializeObject(_userService.GetActiveSites().OrderBy(s => s.Name).ToList());

            List<Role> roleList;
            if (_userService.IsAdmin())
            {
                roleList = context.Roles.ToList();

                ViewBag.InitialState = JsonConvert.SerializeObject(new { Role = "System Admin" }); 
            }
            else
            {
                ViewBag.InitialState = JsonConvert.SerializeObject(new
                {
                    SiteFilter = CurrentUser.userSiteAccess.Select(s => s.Id).ToList()
                });

                var adminRoleIds = new List<int>
                {
                    context.Ref.Roles.SystemAdministrator.Id,
                    context.Ref.Roles.SiteAdministrator.Id,
                    context.Ref.Roles.ExecutiveReportViewer.Id
                };
                roleList = context.Roles.Where(r => !adminRoleIds.Contains(r.Id)).ToList();
            }

            ViewBag.Roles = JsonConvert.SerializeObject(roleList.Select(r => new { Id = r.Id, Name = r.Name }).OrderBy(r => r.Id));

            return View();
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            BuildCreateEditViewData();
            return View();
        }

        private async Task<ActionResult> createUser(UserEditModel model, bool IsActive)
        {
            if (ModelState.IsValid)
            {
                var locationExists = context.Locations.Any(loc => loc.State == model.SelectedState && loc.International == model.IsInternational);
                if (locationExists)
                {
                    model.LocationId = context.Locations.FirstOrDefault(loc => loc.State == model.SelectedState && loc.International == model.IsInternational).Id;
                }
                else if (!string.IsNullOrEmpty(model.SelectedState))
                {
                    model.LocationId = CreateLocation(model.SelectedState, model.IsInternational);
                }
                else
                {
                    model.LocationId = null;
                }

                var userNameExists = context.Users.Any(u => u.Id != model.Id && u.UserName == model.Username);
                if (userNameExists)
                {
                    ViewBag.BannerMessage = "This username is already in use.  Please try a different user name.";
                    ViewBag.BannerClass = "alert-danger";
                    BuildCreateEditViewData();
                    return View();
                }

                var emailExists = context.Users.Any(u => u.Id != model.Id && u.Email == model.Email);
                if (emailExists)
                {
                    ViewBag.BannerMessage = "The email provided is already associated with another account.  Please ensure the email address is correct.";
                    ViewBag.BannerClass = "alert-danger";
                    BuildCreateEditViewData();
                    model.IsActive = IsActive;
                    return View();
                }

                var result = _userManager.Create(new User { UserName = model.Username }, model.Password ?? KeyGenerator.GetUniqueKey(6));
                if (result.Succeeded)
                {
                    var user = _userManager.FindByName(model.Username);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Vendor = context.Vendors.First(v => v.Id == model.VendorId);
                    user.Email = model.Email;
                    user.IsActive = IsActive;
                    await _userManager.SetLockoutEnabledAsync(user.Id, true);
                    if (model.LocationId != null)
                    {
                        user.Location = context.Locations.First(lo => lo.Id == model.LocationId);
                    }
                    var siteIds = new List<int>();
                    if (model.SiteList != null)
                    {
                        siteIds = model.SiteList.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                    }

                    var systemIds = new List<int>();
                    if (model.SystemList != null)
                    {
                        systemIds = model.SystemList.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                    }

                    user.userSiteAccess = context.Sites.Where(s => siteIds.Contains(s.Id)).ToList();
                    var newBuildingSystems = context.Buildings.SelectMany(b => b.Systems).Where(bs => systemIds.Contains(bs.Id)).ToList();

                    List<BuildingSystem> systemAdditions = new List<BuildingSystem>();
                    systemAdditions = newBuildingSystems.ToList();

                    foreach (var addition in systemAdditions)
                        user.BuildingSystems.Add(addition);

                    _userManager.AddToRole(user.Id, context.Roles.First(r => r.Id == model.RoleId).Name);
                    _userManager.Update(user);

                    model.Id = user.Id;

                    TempData["CreateSuccess"] = model.Username + " was created successfully";
                    return RedirectToAction("Index");
                }

                return View(model);
            }

            return View(model);
        }

        private int CreateLocation(string state, bool isInternational)
        {
            Location newLoc = new Location();
            newLoc.State = state;
            newLoc.International = isInternational;
            context.Locations.Add(newLoc);
            return context.SaveChanges();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserEditModel model)
        {
            return await createUser(model, true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUser(UserEditModel model)
        {
            var Result = await createUser(model, false);

            string Url = "https://" + Request.Url.Host + "/User/Invite/";
            string Key = KeyGenerator.GetUniqueKey(128);
            int UserId = model.Id;
            string Body = String.Format("You have been invited to the Water Treatment website. An account with username \"{3}\" has been created for you."
                + "<br /><a href=\"{0}{1}/{2}\">Complete Invitation</a>"
                , Url, UserId, Key, model.Username);

            try
            {
                _emailService.Send(model.Email, "Invitation to Water Treatment Site", Body);
            }
            catch
            {
                Result = View(model);
            }

            context.Users.First(x => x.Id == UserId).InviteCode = Key;
            context.SaveChanges();

            return Result;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Invite(int Id, String InviteCode)
        {

            var user = context.Users.FirstOrDefault(x => x.Id == Id);

            if (user == null)
                return RedirectToAction("Index", "Home");

            if (user.InviteCode != InviteCode)
                return RedirectToAction("Index", "Home");

            if (user.IsActive)
                return RedirectToAction("Index", "Home");

            return View(new InviteModel { Id = Id, Username = user.UserName });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Invite(InviteModel Model)
        {

            var user = context.Users.FirstOrDefault(x => x.Id == Model.Id);

            if (user == null)
                return RedirectToAction("Index", "Home");

            if (user.InviteCode != Model.InviteCode)
                return RedirectToAction("Index", "Home");

            if (user.IsActive)
                return RedirectToAction("Index", "Home");

            if (Model.Password != Model.PasswordCheck)
                return View(Model);

            if (String.IsNullOrEmpty(Model.Password))
                return View(Model);

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(Model.Password);
            user.IsActive = true;

            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ViewUser(int id)
        {
            var user = context.Users.Find(id);

            if (!_userService.CanEditUser(user))
            {
                TempData["Error"] = "You do not have permission to edit this user.";
                return RedirectToAction("Index");
            }

            var isLocked = _userManager.IsLockedOut(id);
            var model = new UserEditModel
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = user.Roles.First().RoleId,
                LocationId = user.Location != null ? user.Location.Id : 0,
                VendorId = user.Vendor != null ? user.Vendor.Id : 0,
                IsLocked = isLocked,
                IsActive = user.IsActive,
                SiteList = string.Join(",", user.userSiteAccess.Select(s => s.Id).ToArray()),
                SystemList = string.Join(",", user.BuildingSystems.Select(b => b.Id))
            };
            if (user.Location != null)
            {
                model.SelectedState = user.Location.State;
                model.IsInternational = user.Location.International;
            }


            BuildCreateEditViewData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReactivateUser(int id)
        {
            var user = context.Users.Find(id);

            if (!_userService.CanEditUser(user))
            {
                TempData["Error"] = "You do not have permission to edit this user.";
                return RedirectToAction("Index");
            }

            var code = KeyGenerator.GetUniqueKey(128);
            var email = user.Email;

            user.IsActive = true;
            user.ResetCode = code;
            user.ResetCodeExpiration = DateTime.UtcNow.AddHours(24);
            _userManager.SetLockoutEndDate(user.Id, DateTimeOffset.UtcNow);
            _userManager.ResetAccessFailedCount(user.Id);

            string resetUrl = string.Format("https://" + Request.Url.Host + "/Account/ResetPassword?code={0}&email={1}", code, email);
            string body = string.Format("Your account \"{1}\" has been reactivated. Please click <a href=\"{0}\">here</a> to reset your password. This link will expire in 24 hours.", resetUrl, user.UserName);

            _emailService.Send(email, "Water Treatment Account Reactivated", body);

            _userManager.Update(user);

            TempData["CreateSuccess"] = string.Format("Successfully reactivated \"{0}\". The user was notified by email.", user.UserName);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> EditUser(int id)
        {

            var user = context.Users.Find(id);

            if (!_userService.CanEditUser(user))
            {
                TempData["Error"] = "You do not have permission to edit this user.";
                return RedirectToAction("Index");
            }

            if (!user.IsActive)
            {
                TempData["Error"] = "You cannot edit an inactive user. Please reactivate.";
                return RedirectToAction("Index");
            }

            var isLocked = await _userManager.IsLockedOutAsync(id);
            var model = new UserEditModel
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LocationId = user.Location != null ? user.Location.Id : 0,
                Email = user.Email,
                RoleId = user.Roles.First().RoleId,
                VendorId = user.Vendor != null ? user.Vendor.Id : 0,
                IsLocked = isLocked,
                IsActive = user.IsActive,
                SiteList = string.Join(",", user.userSiteAccess.Select(s => s.Id).ToArray()),
                SystemList = string.Join(",", user.BuildingSystems.Select(b => b.Id))
            };
            if (user.Location != null)
            {
                model.SelectedState = user.Location.State;
                model.IsInternational = user.Location.International;
            }

            BuildCreateEditViewData();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(UserEditModel model)
        {
            if (ModelState.IsValid)
            {
                var user = context.Users.Find(model.Id);

                var locationExists = context.Locations.Any(loc => loc.State == model.SelectedState && loc.International == model.IsInternational );
                if (locationExists)
                {
                    model.LocationId = context.Locations.FirstOrDefault(loc => loc.State == model.SelectedState && loc.International == model.IsInternational).Id;
                }
                else if (!string.IsNullOrEmpty(model.SelectedState))
                {
                    model.LocationId = CreateLocation(model.SelectedState, model.IsInternational);
                }
                else
                {
                    model.LocationId = null;
                }

                if (!_userService.CanEditUser(user))
                {
                    TempData["Error"] = "You do not have permission to edit this user.";
                    return RedirectToAction("Index");
                }

                if (!user.IsActive)
                {
                    TempData["Error"] = "You cannot edit an inactive user. Please reactivate.";
                    return RedirectToAction("Index");
                }

                var userNameExists = context.Users.Any(u => u.Id != model.Id && u.UserName == model.Username);
                if (userNameExists)
                {
                    ViewBag.BannerMessage = "This username is already in use.  Please try a different user name.";
                    ViewBag.BannerClass = "alert-danger";
                    BuildCreateEditViewData();
                    return View(model);
                }

                var emailExists = context.Users.Any(u => u.Id != model.Id && u.Email == model.Email);
                if (emailExists)
                {
                    ViewBag.BannerMessage = "The email provided is already associated with another account.  Please ensure the email address is correct.";
                    ViewBag.BannerClass = "alert-danger";
                    BuildCreateEditViewData();
                    return View(model);
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Vendor = context.Vendors.First(v => v.Id == model.VendorId);
                user.Email = model.Email;
                user.IsActive = model.IsActive;
                if (model.LocationId != null)
                {
                    user.Location = context.Locations.First(lo => lo.Id == model.LocationId);
                }

                if (model.Password != null)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(model.Password);
                }

                var userId = user.Id;
                await _userManager.SetLockoutEnabledAsync(userId, true);
                if (await _userManager.IsLockedOutAsync(userId) && !model.IsLocked)
                {     // Only provide unlocking for now
                    await _userManager.SetLockoutEndDateAsync(userId, DateTimeOffset.UtcNow);
                }

                var siteIds = new List<int>();
                if (model.SiteList != null)
                {
                    siteIds = model.SiteList.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                }

                var systemIds = new List<int>();
                if (model.SystemList != null)
                {
                    systemIds = model.SystemList.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                }

                var newUserAccess = context.Sites.Where(s => siteIds.Contains(s.Id)).ToList();
                var newBuildingSystems = context.Buildings.SelectMany(b => b.Systems).Where(bs => systemIds.Contains(bs.Id)).ToList();

                List<Site> previousSites = user.userSiteAccess.Where(x => x.IsActive).ToList();

                var isAdmin = CurrentUser.Roles.Any(x => x.RoleId == context.Ref.Roles.SystemAdministrator.Id);
                if (!isAdmin)
                {
                    var allowedSites = CurrentUser.userSiteAccess.ToList();
                    previousSites = user.userSiteAccess.Where(s => allowedSites.Any(x => x.Id == s.Id)).ToList(); 
                }

                var siteRemovals = previousSites.Where(s => !siteIds.Contains(s.Id)).ToList();
                var siteAdditions = newUserAccess.Where(u => !previousSites.Any(x => x.Id == u.Id)).ToList();

                foreach (var removal in siteRemovals)
                    user.userSiteAccess.Remove(removal);
                foreach (var addition in siteAdditions)
                    user.userSiteAccess.Add(addition);

                List<BuildingSystem> systemAdditions = new List<BuildingSystem>();
                List<BuildingSystem> systemRemovals = new List<BuildingSystem>();
                List<BuildingSystem> sysIntersect = new List<BuildingSystem>();

                var previousSystems = user.BuildingSystems.ToList();
                sysIntersect = newBuildingSystems.Intersect(previousSystems).ToList();


                if (user.BuildingSystems.Count() != 0)
                {
                    systemAdditions = newBuildingSystems.Except(sysIntersect).ToList();//newBuildingSystems.Where(bs => !previousSystems.Contains(bs)).ToList();
                    systemRemovals = previousSystems.Except(sysIntersect).ToList(); //newBuildingSystems.Where(bs => !previousSystems.Any(bo => bo.Id == bs.Id)).ToList();
                }
                else
                {
                    systemAdditions = newBuildingSystems.ToList();
                }

                foreach (var addition in systemAdditions)
                    user.BuildingSystems.Add(addition);

                foreach (var removal in systemRemovals)
                    user.BuildingSystems.Remove(removal);

                var currentRole = context.Roles.Find(user.Roles.First().RoleId);
                if (model.RoleId != currentRole.Id)
                {
                    _userManager.RemoveFromRole(userId, currentRole.Name);
                    _userManager.AddToRole(userId, context.Roles.Find(model.RoleId).Name);
                }

                context.SaveChanges();
                _userManager.Update(user);

                TempData["CreateSuccess"] = model.Username + " was updated successfully";
                return RedirectToAction("Index");
            }

            BuildCreateEditViewData();
            return View(model);
        }

        public ActionResult CreateVendor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVendor(VendorModel model)
        {
            if (ModelState.IsValid)
            {
                var newVendor = new Vendor
                {
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode,
                    Phone = model.Phone,
                    PointOfContact = model.PointOfContact
                };

                context.Vendors.Add(newVendor);
                context.SaveChanges();

                TempData["CreateSuccess"] = model.Name + " was created successfully";
                return RedirectToAction("ViewVendors");
            }

            return View(model);
        }

        [HttpGet]
        public ViewResult EditVendor(int id)
        {
            var vendor = context.Vendors.Find(id);
            var model = new VendorModel
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Address = vendor.Address,
                City = vendor.City,
                State = vendor.State,
                ZipCode = vendor.ZipCode,
                PointOfContact = vendor.PointOfContact,
                Phone = vendor.Phone
            };

            model.Members = _userService.GetUsers().Where(u => u.Vendor != null && u.Vendor.Id == model.Id).Select(m => new UserEditModel
            {
                Id = m.Id,
                Username = m.UserName
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVendor(VendorModel model)
        {
            if (ModelState.IsValid)
            {
                var vendor = context.Vendors.Find(model.Id);

                vendor.Name = model.Name;
                vendor.Address = model.Address;
                vendor.City = model.City;
                vendor.State = model.State;
                vendor.ZipCode = model.ZipCode;
                vendor.Phone = model.Phone;
                vendor.PointOfContact = model.PointOfContact;

                context.SaveChanges();

                TempData["CreateSuccess"] = model.Name + " was updated successfully";
                return RedirectToAction("ViewVendors");
            }

            model.Members = _userService.GetUsers().Where(u => u.Vendor != null && u.Vendor.Id == model.Id).Select(m => new UserEditModel
            {
                Id = m.Id,
                Username = m.UserName
            }).ToList();

            return View(model);
        }

        public ActionResult ViewVendors()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SearchVendors(SearchModel criteria)
        {
            IQueryable<Vendor> filteredVendors = context.Vendors.AsQueryable();

            switch (criteria.SortBy ?? string.Empty)
            {
                case "Name":
                default:
                    filteredVendors = filteredVendors.SortBy(x => x.Name, criteria.ShouldForwardSearch);
                    break;
                case "State":
                    filteredVendors = filteredVendors.SortBy(x => x.State, criteria.ShouldForwardSearch);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["Name"]))
            {
                var name = criteria.Filters["Name"].ToLower();
                filteredVendors = filteredVendors.Where(x => x.Name.ToLower().Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(criteria.Filters["State"]))
            {
                var state = criteria.Filters["State"];
                if (state != "All States")
                    filteredVendors = filteredVendors.Where(x => x.State == state);
            }

            var total = filteredVendors.Count();
            var vendors = filteredVendors.Skip(criteria.Offset).Take(criteria.MaxResults).ToList();

            var data = new SearchResult<VendorViewModel>
            {
                Results = vendors.Select(x => new VendorViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    State = x.State
                }),
                Total = total
            };

            return Json(data);
        }

        [HttpPost]
        public async Task<JsonResult> SearchVendorMembership(int id, SearchModel criteria)
        {
            return await SearchUsers(_userService.GetUsers().Where(u => u.Vendor == null || u.Vendor.Id != id).AsQueryable(), criteria);
        }

        [HttpPost]
        [Route("User/AddVendorMember")]
        public JsonResult AddVendorMember(int id, string additions)
        {
            ValidateTokens();

            var vendor = context.Vendors.Find(id);
            var ids = additions.Split(',').Select(i => Convert.ToInt32(i)).ToList();
            var selected = context.Users.Where(m => ids.Contains(m.Id) && m.IsActive).ToList();

            foreach (var member in selected)
                vendor.Members.Add(member);
            context.SaveChanges();

            var data = _userService.GetUsers().Where(u => u.Vendor != null && u.Vendor.Id == vendor.Id).Select(m => new UserEditModel
            {
                Id = m.Id,
                Username = m.UserName
            }).ToList();

            return Json(data);
        }

        [HttpPost]
        [Route("User/RemoveVendorMember")]
        public JsonResult RemoveVendorMember(int id, string removals)
        {
            ValidateTokens();

            var vendor = context.Vendors.Find(id);
            var ids = removals.Split(',').Select(i => Convert.ToInt32(i)).ToList();
            var selected = vendor.Members.Where(m => ids.Contains(m.Id) && m.IsActive).ToList();

            foreach (var member in selected)
                vendor.Members.Remove(member);
            context.SaveChanges();

            var data = _userService.GetUsers().Where(u => u.Vendor != null && u.Vendor.Id == vendor.Id).Select(m => new UserEditModel
            {
                Id = m.Id,
                Username = m.UserName
            });

            return Json(data);
        }

        private async Task<JsonResult> SearchUsers(IQueryable<User> query, SearchModel criteria)
        {
            IQueryable<User> filteredUsers = query;

            switch (criteria.SortBy ?? string.Empty)
            {
                case "Name":
                default:
                    filteredUsers = filteredUsers.SortBy(x => x.UserName, criteria.ShouldForwardSearch);
                    break;
                case "Email":
                    filteredUsers = filteredUsers.SortBy(x => x.Email, criteria.ShouldForwardSearch);
                    break;
                case "Role":
                    filteredUsers = filteredUsers.SortBy(x => context.Roles.FirstOrDefault(r => r.Id == x.Roles.FirstOrDefault().RoleId).Name, criteria.ShouldForwardSearch);
                    break;
                case "Vendor":
                    filteredUsers = filteredUsers.SortBy(x => x.Vendor != null ? x.Vendor.Name : string.Empty, criteria.ShouldForwardSearch);
                    break;
            }

            if (_userService.IsSiteAdmin())
            {
                var adminRoleIds = new List<int>
                {
                    context.Ref.Roles.SystemAdministrator.Id,
                    context.Ref.Roles.SiteAdministrator.Id
                };
                filteredUsers = filteredUsers.Where(u => !u.Roles.Any(r => adminRoleIds.Contains(r.RoleId)));
            }

            filteredUsers = filteredUsers.Where(u => u.Id != CurrentUser.Id);

            if (!string.IsNullOrWhiteSpace(criteria.Filters["Name"]))
            {
                var name = criteria.Filters["Name"].ToLower();
                filteredUsers = filteredUsers.Where(x => x.UserName.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["Email"]))
            {
                var email = criteria.Filters["Email"].ToLower();
                filteredUsers = filteredUsers.Where(x => x.Email.ToLower().Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["Roles"]))
            {
                var roleIds = criteria.Filters["Roles"].Split(',').Select(v => Convert.ToInt32(v));
                filteredUsers = filteredUsers.Where(x => x.Roles.Any(r => roleIds.Contains(r.RoleId)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["Sites"]))
            {
                const int Unassigned = 0;

                var siteIds = criteria.Filters["Sites"].Split(',').Select(v => Convert.ToInt32(v)).ToList();
                if (siteIds.Contains(Unassigned))
                {
                    siteIds.Remove(Unassigned);
                    filteredUsers = filteredUsers.Where(x => x.userSiteAccess.Any(s => siteIds.Contains(s.Id)) || x.userSiteAccess.Count == 0);
                }
                else
                {
                    filteredUsers = filteredUsers.Where(x => x.userSiteAccess.Any(s => siteIds.Contains(s.Id)));
                }
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["Vendors"]))
            {
                var vendorIds = criteria.Filters["Vendors"].Split(',').Select(v => Convert.ToInt32(v));
                filteredUsers = filteredUsers.Where(x => x.Vendor != null ? vendorIds.Contains(x.Vendor.Id) : false);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["Statuses"]))
            {
                var statuses = criteria.Filters["Statuses"].Split(',');
                var includeActive = statuses.Contains("active");
                var includeInactive = statuses.Contains("inactive");
                if (includeActive && !includeInactive) filteredUsers = filteredUsers.Where(x => x.IsActive);
                if (!includeActive && includeInactive) filteredUsers = filteredUsers.Where(x => !x.IsActive);
            }

            var total = filteredUsers.Count();
            var users = filteredUsers.Skip(criteria.Offset).Take(criteria.MaxResults).ToList();

            var roles = context.Roles.ToList();
            var lockedUsers = new Dictionary<int, bool>();
            foreach (var u in users)
            {
                lockedUsers[u.Id] = await _userManager.IsLockedOutAsync(u.Id);
            }
            var data = new SearchResult<UserViewModel>
            {
                Results = users.Select(x => new UserViewModel
                {
                    Id = x.Id,
                    Name = x.UserName,
                    Email = x.Email,
                    Role = string.Join(",", x.Roles.Select(y => roles.Single(z => z.Id == y.RoleId).Name)),
                    Vendor = x.Vendor != null ? x.Vendor.Name : string.Empty,
                    Sites = string.Join(",", x.userSiteAccess.Select(s => s.Name)),
                    SiteCount = x.userSiteAccess.Count,
                    SiteIdList = string.Join(",", x.userSiteAccess.Select(s => s.Id)),
                    IsLocked = lockedUsers[x.Id],
                    IsActive = x.IsActive
                }),
                Total = total
            };

            return Json(data);
        }

        [HttpPost]
        public async Task<JsonResult> SearchUsers(SearchModel criteria)
        {
            return await SearchUsers(context.Users.AsQueryable(), criteria);
        }

        [HttpPost]
        [ActionName("UnlockUser")]
        public async Task<ActionResult> UnlockUser(int userId)
        {
            ValidateTokens();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (!_userService.CanEditUser(user))
                {
                    throw new Exception("Permission denied");
                }

                var isLockedOut = await _userManager.IsLockedOutAsync(userId);
                if (isLockedOut)
                {
                    _userManager.SetLockoutEndDate(userId, DateTimeOffset.UtcNow);
                    await _userManager.ResetAccessFailedCountAsync(userId);
                    ViewBag.BannerMessage = String.Format("Successfully unlocked user {0}", user.UserName);
                    ViewBag.BannerClass = "alert-success";
                }
                else
                {
                    throw new Exception(String.Format("User {0} is not currently locked", user.UserName));
                }
            }
            catch (Exception e)
            {
                ViewBag.BannerMessage = e.Message;
                ViewBag.BannerClass = "alert-danger";
            }
            return PartialView("_Banner");
        }

        [HttpPost]
        [ActionName("DeactivateUser")]
        public async Task<ActionResult> DeactivateUser(int userId)
        {
            ValidateTokens();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (!_userService.CanEditUser(user))
                {
                    throw new Exception("Permission denied");
                }

                if (user.IsActive)
                {
                    user.IsActive = false;
                    user.EulaAgreedOn = null;
                    await _userManager.UpdateAsync(user);
                    ViewBag.BannerMessage = String.Format("Successfully deactivated user {0}", user.UserName);
                    ViewBag.BannerClass = "alert-success";
                }
                else
                {
                    throw new Exception(String.Format("User {0} is not currently active", user.UserName));
                }
            }
            catch (Exception e)
            {
                ViewBag.BannerMessage = e.Message;
                ViewBag.BannerClass = "alert-danger";
            }
            return PartialView("_Banner");
        }

        [HttpPost]
        public ActionResult GrantSiteAccess(int siteId, int userId)
        {
            ValidateTokens();

            var site = context.Sites.Find(siteId);
            var user = _userManager.FindById(userId);

            if (_userService.CanEditSite(site) && user.IsActive)
            {
                var buildingSystems = site.Buildings.SelectMany(b => b.Systems).ToList(); 

                user.userSiteAccess.Add(site);
                foreach (BuildingSystem system in buildingSystems)
                {
                    user.BuildingSystems.Add(system);
                }
                //context.Users.Add(user);
                context.SaveChanges();

                ViewBag.BannerMessage = string.Format("{0} was granted access to {1}.", user.UserName, site.Name);
                ViewBag.BannerClass = "alert-success";
            }
            else if (!user.IsActive)
            {
                ViewBag.BannerMessage = string.Format("Cannot edit inactive user \"{0}\".", user.UserName);
                ViewBag.BannerClass = "alert-danger";
            }
            else
            {
                ViewBag.BannerMessage = "Permission Denied.";
                ViewBag.BannerClass = "alert-danger";
            }

            return PartialView("_Banner");
        }

        [HttpPost]
        public ActionResult RevokeSiteAccess(int siteId, int userId)
        {
            ValidateTokens();

            var site = context.Sites.Find(siteId);
            var user = _userManager.FindById(userId);

            if (_userService.CanEditSite(site) && _userService.CanEditUser(user) && user.IsActive)
            {
                var revoke = user.userSiteAccess.FirstOrDefault(s => s.Id == site.Id);
                if (revoke != null)
                {
                    user.userSiteAccess.Remove(revoke);
                }
                context.SaveChanges();

                ViewBag.BannerMessage = string.Format("{0}'s access to {1} was revoked.", user.UserName, site.Name);
                ViewBag.BannerClass = "alert-success";
            }
            else if (!user.IsActive)
            {
                ViewBag.BannerMessage = string.Format("Cannot edit inactive user \"{0}\".", user.UserName);
                ViewBag.BannerClass = "alert-danger";
            }
            else
            {
                ViewBag.BannerMessage = "Permission Denied.";
                ViewBag.BannerClass = "alert-danger";
            }

            return PartialView("_Banner");
        }

        [HttpGet]
        [Route("ValidateVendorName/{name}/{id}")]
        public JsonResult ValidateVendorName(string name, int id)
        {
            return Json(!context.Vendors.Any(v => v.Name == name && (v.Id == 0 || v.Id != id)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("ValidateUserName/{name}/{id}")]
        public JsonResult ValidateUserName(string name, int id)
        {
            var user = _userManager.FindByName(name);
            return Json(user == null || (id != 0 && user.Id == id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ValidateEmail(string email, int id)
        {
            var user = _userManager.FindByEmail(email);
            return Json(user == null || (id != 0 && user.Id == id), JsonRequestBehavior.AllowGet);
        }

        private void ValidateTokens()
        {
            var cookieToken = Request.Cookies["__RequestVerificationToken"].Value;
            var formToken = Request.Headers["__RequestVerificationToken"];
            AntiForgery.Validate(cookieToken, formToken);
        }

        private void BuildCreateEditViewData()
        {
            var isAdmin = _userService.IsAdmin();
            //If they aren't an admin remove sys admin and site admin roles.

            var roleList = context.Roles.Where(x => isAdmin || (x.Id != context.Ref.Roles.SystemAdministrator.Id && x.Id != context.Ref.Roles.SiteAdministrator.Id && x.Id != context.Ref.Roles.ExecutiveReportViewer.Id));

            ViewBag.Roles = JsonConvert.SerializeObject(roleList.Select(r => new { Id = r.Id, Name = r.Name }).OrderBy(r => r.Id));
            ViewBag.Vendors = JsonConvert.SerializeObject(context.Vendors.OrderBy(v => v.Name).Select(v => new VendorModel { Id = v.Id, Name = v.Name }).ToList());

            var siteAccessTree = _userService.GetActiveSites().OrderBy(s => s.Name).ToList().Select(BuildTree3Node).ToList();

            ViewBag.AccessTree = JsonConvert.SerializeObject(siteAccessTree);
        }

        private JsTree3Node BuildTree3Node(Site s)
        {
            return new JsTree3Node
            {
                id = "S-" + s.Id.ToString(),
                text = s.Name,
                state = new State(false, false, false),
                children = s.Buildings.Select(b => new JsTree3Node
                {
                    id = "B-" + b.Id.ToString(),
                    text = b.Name,
                    state = new State(false, false, false),
                    children = b.Systems.Where(sy=>sy.IsActive).Select(bs => new JsTree3Node
                    {
                        id = "T-" + bs.Id,
                        text = bs.SystemType.Name,
                        state = new State(false, false, false)
                    }).ToList()
                }).ToList()
            };
        }

    }
}
