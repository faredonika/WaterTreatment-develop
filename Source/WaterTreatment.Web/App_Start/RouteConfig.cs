using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WaterTreatment.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Report Attachment",
                url: "Reports/Attachments/{action}/{id}",
                defaults: new { controller = "Reports", action = "Download" });

            routes.MapRoute(
                name: "Attachments",
                url: "DataEntry/Attachments/{action}/{id}",
                defaults: new { controller = "DataEntry" }
            );

            routes.MapRoute(
                name: "Get Report Attachments",
                url: "DataEntry/Attachments/{id}",
                defaults: new { controller = "DataEntry", action = "GetAll" }
            );

            routes.MapRoute(
                name: "MainAliases",
                url: "Main/{ignore}/",
                defaults: new { controller = "Home", action = "Index", ignore = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Invite",
                "User/Invite/{Id}/{InviteCode}",
                new { controller = "User", action = "Invite", Id = -1, InviteCode = "" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
