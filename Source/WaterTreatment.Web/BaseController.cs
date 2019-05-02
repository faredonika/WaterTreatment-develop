using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Models;

namespace WaterTreatment.Web
{

    public abstract class BaseController : Controller
    {

        protected readonly WTContext context;

        protected User CurrentUser
        {

            get
            {

                var userid = User.Identity.GetUserId<int>();
                return context.Users.First(x => x.Id == userid);
            }

        }

        public BaseController(WTContext context)
        {
            this.context = context;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            bool isAnonymous = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0;

            if (!isAnonymous && !CurrentUser.IsActive) {
                HttpContext.GetOwinContext().Authentication.SignOut();
                filterContext.Result = new RedirectResult("/");
                return;
            }

            var descriptor = filterContext.ActionDescriptor;
            if (!isAnonymous && !CurrentUser.EulaAgreedOn.HasValue && descriptor.ActionName != "Eula" && descriptor.ControllerDescriptor.ControllerName != "Account") {
                filterContext.Result = RedirectToAction("Eula", "Account");
                return;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            if (TempData.Keys.Contains("CreateSuccess"))
            {
                ViewBag.BannerMessage = TempData["CreateSuccess"];
                ViewBag.BannerClass = "alert-success";
            }
            else if (TempData.Keys.Contains("Error"))
            {
                ViewBag.BannerMessage = TempData["Error"];
                ViewBag.BannerClass = "alert-danger";
            }
            else if (TempData.Keys.Contains("Danger"))
            {
                ViewBag.BannerMessage = TempData["Danger"];
                ViewBag.BannerClass = "alert-danger";
            }
            else if (TempData.Keys.Contains("Warning"))
            {
                ViewBag.BannerMessage = TempData["Warning"];
                ViewBag.BannerClass = "alert-danger";
            }

            bool isAnonymous = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0;
            if (!isAnonymous)
            {
                var controller = this.ControllerContext.RouteData.Values["controller"] as string;
                var action = this.ControllerContext.RouteData.Values["action"] as string;

                var user = CurrentUser;
                var roleIds = user.Roles.Select(x => x.RoleId);
                var roles = context.Roles.Where(x => roleIds.Contains(x.Id));

                ViewData["NavBar"] = roles.SelectMany(x => x.SubSections).Distinct().ToList();

                var Main = context.MainSections.FirstOrDefault(x => x.Controller == controller);
                if (Main != null)
                {
                    List<NavSection> Nodes = new List<NavSection>();

                    Nodes.Add(Main);
                    Nodes.AddRange(Main.SubSections.Where(x => x.Action == action));

                    ViewData["PathNodes"] = Nodes;
                }

                var userInfo = new UserInfoModel { RoleName = roles.First().Name, IsDataRecorder = roles.Any(r => r.Id == context.Ref.Roles.DataRecorder.Id || r.Id == context.Ref.Roles.AuditRecorder.Id) };

                if (user.FirstName != null && user.LastName != null)
                    userInfo.FullName = user.FirstName + " " + user.LastName;
                else
                    userInfo.FullName = user.UserName;

                ViewData["UserInfo"] = userInfo;
            }            

            base.OnActionExecuted(filterContext);

        }

    }

}