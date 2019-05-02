using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using WaterTreatment.Web.Attributes;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Models;
using WaterTreatment.Web.Services;

namespace WaterTreatment.Web.Controllers
{

    [SectionAuthorize]
    public class SiteController : BaseController
    {


        private readonly IUserService _userService;

        public SiteController(WTContext context, IUserService userService) : base(context)
        {
            _userService = userService;
        }

        [Route("Index")]
        [Route("View")]
        public ActionResult Index()
        {

            ViewBag.SystemTypes = JsonConvert.SerializeObject(context.SystemTypes.Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => new { Name = x.Name, Id = x.Id }));

            return View();
        }

        public ActionResult Create()
        {
            ViewBag.SystemTypes = JsonConvert.SerializeObject(context.SystemTypes.Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => new { Name = x.Name, Id = x.Id }));

            return View(new SiteModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SiteModel model)
        {
            var trimmedName = model.Name.Trim().ToLower();
            var nameExists = context.Sites.Any(s => s.Name.Trim().ToLower() == trimmedName);

            if (ModelState.IsValid && !nameExists)
            {

                var newSite = new Site { Name = model.Name, Location = model.Location, IsActive = model.IsActive };

                foreach(var b in model.Buildings)
                {

                    var newBuilding = new Building { Name = b.Name, BuildingNumber = b.BuildingNumber, RPSUID = b.RPSUID, RPUID = b.RPUID, IsActive = b.IsActive };

                    foreach (var s in b.Systems)
                    {
                        var systemType = context.SystemTypes.First(x => x.Id == s.SystemTypeId);
                        newBuilding.Systems.Add(new BuildingSystem { Location = s.Location, Description = s.Description, SystemType = systemType });
                    }

                    newSite.Buildings.Add(newBuilding);

                }

                context.Sites.Add(newSite);
                context.SaveChanges();

                TempData["CreateSuccess"] = model.Name + " was created successfully";
                return RedirectToAction("Index");

            }

            if (nameExists) {
                ModelState.AddModelError("", String.Format("A site with the name {0} already exists.  Please choose a unique site name.", model.Name));
                ViewBag.Data = JsonConvert.SerializeObject(model);
            }

            ViewBag.SystemTypes = JsonConvert.SerializeObject(context.SystemTypes.OrderBy(x => x.Name).Select(x => new { Name = x.Name, Id = x.Id }));
            return View(model);

        }

        [HttpGet]
        public ActionResult Edit(int Id = 0)
        {

            var site = context.Sites.FirstOrDefault(x => x.Id == Id);

            if (site == null)
            {
                TempData["Error"] = "Invalid Id";
                return RedirectToAction("Index");
            }

            if (!_userService.CanEditSite(site))
            {
                TempData["Error"] = "You do not have permisson to edit this site.";
                return RedirectToAction("Index");
            }

            var siteModel = new SiteModel { Id = site.Id, Name = site.Name, Location = site.Location, IsActive = site.IsActive };

            foreach (var b in site.Buildings)
            {

                var newBuilding = new BuildingModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    BuildingNumber = b.BuildingNumber,
                    RPUID = b.RPUID,
                    RPSUID = b.RPSUID,
                    IsActive = b.IsActive
                    
                };

                foreach (var s in b.Systems)
                {
                    if (s.IsActive)
                    {
                        newBuilding.Systems.Add(new BuildingSystemModel { Id = s.Id, Location = s.Location, Description = s.Description, SystemTypeId = s.SystemType.Id });
                    }
                }

                siteModel.Buildings.Add(newBuilding);

            }

            ViewBag.SystemTypes = JsonConvert.SerializeObject(context.SystemTypes.OrderBy(x => x.Name).Select(x => new { Name = x.Name, Id = x.Id, IsActive = x.IsActive }));
            ViewBag.Data = JsonConvert.SerializeObject(siteModel); 

            return View();

        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(SiteModel Model)
        {
            var trimmedName = Model.Name.Trim().ToLower();
            var nameExists = context.Sites.Any(s => s.Id != Model.Id && s.Name.Trim().ToLower() == trimmedName);
            if (ModelState.IsValid && !nameExists)
            {

                var site = context.Sites.FirstOrDefault(x => x.Id == Model.Id);

                if (site == null)
                {
                    TempData["Error"] = "Invalid Id";
                    return RedirectToAction("Index");
                }

                if (!_userService.CanEditSite(site))
                {
                    TempData["Error"] = "You do not have permisson to edit this site.";
                    return RedirectToAction("Index");
                }

                site.Name = Model.Name;
                site.Location = Model.Location;
                site.IsActive = Model.IsActive;

                var ToRemove = site.Buildings.Where(x => !Model.Buildings.Any(y => y.Id == x.Id)).ToList();
                var ToUpdate = site.Buildings.Where(x => Model.Buildings.Any(y => y.Id == x.Id)).ToList();
                var ToAdd = Model.Buildings.Where(x => !site.Buildings.Any(y => y.Id == x.Id)).ToList();

                //Remove buildings that no longer exist
                foreach (var b in ToRemove)
                    site.Buildings.Remove(b);

                //Update any existing buildings
                foreach (var b in ToUpdate)
                {

                    var NewData = Model.Buildings.First(x => x.Id == b.Id);

                    b.Name = NewData.Name;
                    b.BuildingNumber = NewData.BuildingNumber;
                    b.RPSUID = NewData.RPSUID;
                    b.RPUID = NewData.RPUID;
                    b.IsActive = NewData.IsActive;

                    var ToRemoveSystems = b.Systems.Where(x => !NewData.Systems.Any(y => y.Id == x.Id)).ToList();
                    var ToUpdateSystems = b.Systems.Where(x => NewData.Systems.Any(y => y.Id == x.Id && ( y.Location != x.Location || y.Description!= x.Description || y.SystemTypeId != x.SystemType.Id))).ToList();
                    var ToAddSystems = NewData.Systems.Where(x => !b.Systems.Any(y => y.Id == x.Id)).ToList();

                    //Remove BuildingSystems that no longer exist
                    foreach (var s in ToRemoveSystems)
                    {
                        //Remove BuildingSystem(s);
                        s.Location = s.Location;
                        s.Description = s.Description;
                        s.SystemType = s.SystemType;
                        s.UpdateDate = DateTime.Now;
                        s.IsActive = false;
                        s.Users = null;
                    }

                    //Update any existing BuildingSystems
                    foreach (var s in ToUpdateSystems)
                    {
                        var NewSystemData = NewData.Systems.First(x => x.Id == s.Id);
                        var siteReports = context.Reports.Where(r=>r.Site.Id == s.Building.Site.Id && r.SubmissionDate !=null);
                        if (siteReports.Count() > 0)
                        {
                            //add new system type
                            b.Systems.Add(new BuildingSystem { Location = NewSystemData.Location, Description = NewSystemData.Description, SystemType = context.SystemTypes.First(x => x.Id == NewSystemData.SystemTypeId), IsActive = true, CreateDate = DateTime.Now });
                            //update current sys type
                            s.IsActive = false;
                            s.UpdateDate = DateTime.Now;
                        }
                        else
                        {
                            s.Location = NewSystemData.Location;
                            s.Description = NewSystemData.Description;
                            s.SystemType = context.SystemTypes.First(x => x.Id == NewSystemData.SystemTypeId);
                            s.IsActive = true;
                            s.UpdateDate = DateTime.Now;
                        }
                    }

                    //Add any new BuildingSystems
                    foreach (var s in ToAddSystems)
                    {
                        var systemType = context.SystemTypes.First(x => x.Id == s.SystemTypeId);
                        b.Systems.Add(new BuildingSystem { Location = s.Location, Description = s.Description, SystemType = systemType, IsActive = true, CreateDate = DateTime.Now});
                    }

                    
                    //foreach (var system in b.Systems.Where(bs => bs.Id == 0))
                    //{
                    //    foreach (var access in site.Access)
                    //    {
                    //        access.BuildingSystems.Add(system);
                    //    }
                    //}

                }

                //Add any new buildings
                foreach (var b in ToAdd)
                {

                    var newBuilding = new Building { Name = b.Name, BuildingNumber = b.BuildingNumber, RPSUID = b.RPSUID, RPUID = b.RPUID, IsActive = b.IsActive };

                    foreach (var s in b.Systems)
                    {
                        var systemType = context.SystemTypes.First(x => x.Id == s.SystemTypeId);
                        newBuilding.Systems.Add(new BuildingSystem { Location = s.Location, Description = s.Description, SystemType = systemType });
                    }

                    site.Buildings.Add(newBuilding);

                    //foreach (var access in site.Access)
                    //{
                    //    foreach (var system in newBuilding.Systems)
                    //    {
                    //        access.BuildingSystems.Add(system);
                    //    }
                    //}
                }

                context.SaveChanges();

                TempData["CreateSuccess"] = Model.Name + " was updated successfully";
                return RedirectToAction("Index");
            }

            if (nameExists) {
                ModelState.AddModelError("", String.Format("A site with the name {0} already exists.  Please choose a unique site name.", Model.Name));
                ViewBag.SystemTypes = JsonConvert.SerializeObject(context.SystemTypes.OrderBy(x => x.Name).Select(x => new { Name = x.Name, Id = x.Id }));
                ViewBag.Data = JsonConvert.SerializeObject(Model);
                return View();
            }

            return Edit(Model.Id);

        }

        public JsonResult Search(SearchModel Criteria)
        {

            var repo = _userService.GetSites().AsQueryable();
            IQueryable<Site> filterSites;

            Criteria.SortBy = Criteria.SortBy ?? String.Empty;

            //The nearly duplicated code is a limitation to base Linq to SQL and I don't want to write an extension right now
            switch (Criteria.SortBy)
            {
                case "Name":
                default:
                    if (Criteria.ShouldForwardSearch)
                        filterSites = repo.OrderBy((x) => x.Name);
                    else
                        filterSites = repo.OrderByDescending((x) => x.Name);
                    break;
                case "Location":
                    if (Criteria.ShouldForwardSearch)
                        filterSites = repo.OrderBy((x) => x.Location);
                    else
                        filterSites = repo.OrderByDescending((x) => x.Location);
                    break;
                case "BuildingCount":
                    if (Criteria.ShouldForwardSearch)
                        filterSites = repo.OrderBy((x) => x.Buildings.Count);
                    else
                        filterSites = repo.OrderByDescending((x) => x.Buildings.Count);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["Name"]))
            {
                var name = Criteria.Filters["Name"];
                filterSites = filterSites.Where(x => x.Name.Contains(name));
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["Location"]))
            {
                var name = Criteria.Filters["Location"];
                filterSites = filterSites.Where(x => x.Location.Contains(name));
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["BuildingName"]))
            {
                var name = Criteria.Filters["BuildingName"];
                filterSites = filterSites.Where(x => x.Buildings.Any(y => y.Name.Contains(name)));
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["SystemTypes"]))
            {
                var systemTypes = Criteria.Filters["SystemTypes"].Split(',').Select(x => Convert.ToInt32(x));
                filterSites = filterSites.Where(x => x.Buildings.SelectMany(y => y.Systems.Select(z => z.SystemType.Id)).Any(y => systemTypes.Any(z => z == y)));
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["IsActive"]) && Criteria.Filters["IsActive"] != "all")
            {
                var isActive = Convert.ToBoolean(Criteria.Filters["IsActive"]);
                filterSites = filterSites.Where(x => x.IsActive == isActive);
            }

            var total = filterSites.Count();

            var sites = filterSites.Skip(Criteria.Offset).Take(Criteria.MaxResults);

            var data = new SearchResult<SiteSearchModel>
            {
                Results = sites.Select(x => new SiteSearchModel{
                    Id = x.Id,
                    Name = x.Name,
                    Location = x.Location,
                    BuildingCount = x.Buildings.Count
                }).ToList(),
                Total = total
            };

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Buildings(int Id)
        {

            //Very important to security trim the site requested here, instead of just blindly assuming they have access to the site
            var data = _userService.GetActiveSites().Where(x => x.Id == Id).OrderBy(x => x.Name).SelectMany(x => x.Buildings).Select(x => new { Name = x.Name, Id = x.Id });

            return Json(data, JsonRequestBehavior.AllowGet);

        }


    }
}