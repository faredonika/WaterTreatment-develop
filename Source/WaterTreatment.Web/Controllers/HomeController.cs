using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using WaterTreatment.Web.Entities;
using MoreLinq;

namespace WaterTreatment.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(WTContext context) : base(context) { }

        public ActionResult Index()
        {
            var userid = User.Identity.GetUserId<int>();
            var user = context.Users.First(x => x.Id == userid);
            var roleIds = user.Roles.Select(x => x.RoleId);
            var roles = context.Roles.Where(x => roleIds.Contains(x.Id));

            ViewBag.LandingActions = roles.SelectMany(x => x.Actions).OrderBy(x => x.Order).Select(x => x.Action).DistinctBy(x => x.Id).AsEnumerable();

            return View();
        }
    }
}