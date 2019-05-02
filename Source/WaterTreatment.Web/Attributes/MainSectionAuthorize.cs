using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace WaterTreatment.Web.Attributes
{

    public class SectionAuthorize : AuthorizeAttribute
    {

        public WTContext Context { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            base.OnAuthorization(filterContext);

            var User = filterContext.HttpContext.User;

            if (User.Identity.IsAuthenticated)
            {

                var action = filterContext.RequestContext.RouteData.Values["action"] as String;
                var controller = filterContext.RequestContext.RouteData.Values["controller"] as String;

                var verifyAction = true;

                //Does an exact match exist?
                if (!Context.SubSections.Any(x => x.Action == action && x.MainSection.Controller == controller))
                {

                    verifyAction = false;//Downgrade to just a controller check

                    //Check if atleast the controller exists, if it doesn't we just stop checking.
                    if (!Context.MainSections.Any(x => x.Controller == controller))
                            return;//Basically ignore any path that doesn't even exist in the database, allow a 404 to occur
                }    

                var userid = User.Identity.GetUserId<int>();
                var user = Context.Users.First(x => x.Id == userid);
                var roleIds = user.Roles.Select(x => x.RoleId);
                var roles = Context.Roles.Where(x => roleIds.Contains(x.Id));

                //Override that grants sys admins access to everywhere regardless of ref data
                if (roles.Any(x => x.Id == Context.Ref.Roles.SystemAdministrator.Id))
                    return;

                //Otherwise verify that the subsection matches the action
                if (!roles.SelectMany(x => x.SubSections).Any(x => (!verifyAction || x.Action == action) && x.MainSection.Controller == controller))
                    HandleUnauthorizedRequest(filterContext);

            }
            
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Controller.TempData["Error"] = "You do not have permission to access this page.";
            filterContext.Result = new RedirectResult("/");
        }

    }

}