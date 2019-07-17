using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WaterTreatment.Web.Attributes;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Models;

namespace WaterTreatment.Web.Controllers
{

    [SectionAuthorize]
    public class SystemController : BaseController
    {
        public SystemController(WTContext context) : base(context) { }

        [Route("Index")]
        [Route("View")]
        public ViewResult Index()
        {
            return View();
        }

        public ViewResult Create()
        {
            ViewBag.ParameterTypes = context.ParameterTypes.Select(x => new { Id = x.Id, Name = x.Name });
            ViewBag.Sites = context.Sites.Select(x => new { Id = x.Id, Name = x.Name });
            return View(new SystemCreateModel());
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(SystemCreateModel Model)
        {
            var trimmedName = Model.Name.Trim().ToLower();
            var nameExists = context.SystemTypes.Any(t => t.Name.Trim().ToLower() == trimmedName);
            if (ModelState.IsValid && !nameExists)
            {

                var newSystem = new SystemType { Name = Model.Name, IsActive = Model.IsActive };

                foreach (var p in Model.Parameters)
                {

                    var newParameter = new Parameter
                    {
                        Name = p.Name,
                        Frequency = p.Frequency,
                        Type = context.ParameterTypes.First(x => x.Id == p.ParameterTypeId),
                        Unit = p.Unit,
                        Source = p.Source,
                        Link = p.Link,
                        Use = p.Use,
                      
                        
                    };

                    foreach (var b in p.Bounds)
                    {

                        var site = context.Sites.FirstOrDefault(x => x.Id == b.SiteId);

                        var newBound = new ParameterBound
                        {
                            Type = b.Type,
                            Range = b.Range,
                            MinValue = b.MinValue,
                            MaxValue = b.MaxValue,
                            MinDescription = b.MinDescription,
                            MaxDescription = b.MaxDescription,
                            IsEnforced = b.IsEnforced
//                            Site = site
                        };
                        newParameter.ParameterBounds.Add(newBound);
                    }

                    newSystem.Parameters.Add(newParameter);

                }

                context.SystemTypes.Add(newSystem);
                context.SaveChanges();

                TempData["CreateSuccess"] = Model.Name + " was created successfully";
                return RedirectToAction("Index");
            }

            if (nameExists) {
                ModelState.AddModelError("", String.Format("A system type with the name {0} already exists.  Please choose a unique site name.", Model.Name));
                ViewBag.Data = Model;
                ViewBag.ParameterTypes = context.ParameterTypes.Select(x => new { Id = x.Id, Name = x.Name });
                ViewBag.Sites = context.Sites.Select(x => new { Id = x.Id, Name = x.Name });
            }

            ViewBag.ParameterTypes = context.ParameterTypes.Select(x => new { Id = x.Id, Name = x.Name });
            return View(Model);
        }

        public ActionResult Edit(int Id = 0)
        {

            var systemType = context.SystemTypes.FirstOrDefault(x => x.Id == Id);

            if (systemType == null)
            {
                TempData["Error"] = "Invalid Id";
                return RedirectToAction("Index");
            }

            var systemModel = new SystemCreateModel { Id = systemType.Id, Name = systemType.Name, IsActive = systemType.IsActive };

            foreach (var p in systemType.Parameters)
            {

                var newParameter = new ParameterModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Frequency = p.Frequency,
                    ParameterTypeId = p.Type.Id,
                    Unit = p.Unit,
                    Source = p.Source,
                    Link = p.Link,
                    Use = p.Use
                };

                foreach (var b in p.ParameterBounds)
                {
                    var newBound = new BoundModel
                    {
                        Id = b.Id,
                        Range = b.Range,
                        MinValue = b.MinValue,
                        MaxValue = b.MaxValue,
                        MinDescription = b.MinDescription,
                        MaxDescription = b.MaxDescription,
                        IsEnforced = b.IsEnforced,
                        Type = b.Type
//                        SiteId = b.Site == null ? 0 : b.Site.Id
                    };
                    newParameter.Bounds.Add(newBound);
                }

                systemModel.Parameters.Add(newParameter);

            }

            ViewBag.ParameterTypes = context.ParameterTypes.Select(x => new { Id = x.Id, Name = x.Name });
            ViewBag.Sites = context.Sites.Select(x => new { Id = x.Id, Name = x.Name });
            ViewBag.Data = systemModel;

            return View();
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(SystemCreateModel Model)
        {
            var trimmedName = Model.Name.Trim().ToLower();
            var nameExists = context.SystemTypes.Any(t => t.Id != Model.Id && t.Name.Trim().ToLower() == trimmedName);
            if (ModelState.IsValid && !nameExists)
            {

                var systemType = context.SystemTypes.FirstOrDefault(x => x.Id == Model.Id);

                if (systemType == null)
                {
                    TempData["Error"] = "Invalid Id";
                    return RedirectToAction("Index");
                }

                systemType.Name = Model.Name;
                systemType.IsActive = Model.IsActive;

                var ToRemove = systemType.Parameters.Where(x => !Model.Parameters.Any(y => y.Id == x.Id)).ToList();
                var ToUpdate = systemType.Parameters.Where(x => Model.Parameters.Any(y => y.Id == x.Id)).ToList();
                var ToAdd = Model.Parameters.Where(x => !systemType.Parameters.Any(y => y.Id == x.Id)).ToList();

                //Remove parameters that no longer exist
                foreach (var p in ToRemove)
                    systemType.Parameters.Remove(p);

                //Update any existing parameters
                foreach (var p in ToUpdate)
                {

                    var NewData = Model.Parameters.First(x => x.Id == p.Id);

                    p.Name = NewData.Name;
                    p.Frequency = NewData.Frequency;
                    p.Type = context.ParameterTypes.First(x => x.Id == NewData.ParameterTypeId);
                    p.Unit = NewData.Unit;
                    p.Source = NewData.Source;
                    p.Link = NewData.Link;
                    p.Use = NewData.Use;

                    var ToRemoveBounds = p.ParameterBounds.Where(x => !NewData.Bounds.Any(y => y.Id == x.Id)).ToList();
                    var ToUpdateBounds = p.ParameterBounds.Where(x => NewData.Bounds.Any(y => y.Id == x.Id)).ToList();
                    var ToAddBounds = NewData.Bounds.Where(x => !p.ParameterBounds.Any(y => y.Id == x.Id)).ToList();

                    //Remove bounds that no longer exist
                    foreach (var b in ToRemoveBounds)
                        p.ParameterBounds.Remove(b);

                    //Update any existing parameter bounds
                    foreach (var b in ToUpdateBounds)
                    {

                        var NewBoundData = NewData.Bounds.First(x => x.Id == b.Id);

                        var site = context.Sites.FirstOrDefault(x => x.Id == NewBoundData.SiteId);

                        b.Type = NewBoundData.Type;
                        b.Range = NewBoundData.Range;
                        b.MinValue = NewBoundData.MinValue;
                        b.MaxValue = NewBoundData.MaxValue;
                        b.MinDescription = NewBoundData.MinDescription;
                        b.MaxDescription = NewBoundData.MaxDescription;
                        b.IsEnforced = NewBoundData.IsEnforced;

                        //var forceLoad = b.Site;
                        //b.Site = site;

                    }

                    //Add any new bounds
                    foreach(var b in ToAddBounds)
                    {

                        var site = context.Sites.FirstOrDefault(x => x.Id == b.SiteId);
                        var newBound = new ParameterBound
                        {
                            Type = b.Type,
                            Range = b.Range,
                            MinValue = b.MinValue,
                            MaxValue = b.MaxValue,
                            MinDescription = b.MinDescription,
                            MaxDescription = b.MaxDescription,
                            IsEnforced = b.IsEnforced
//                            Site = site
                        };
                        p.ParameterBounds.Add(newBound);
                    }

                }

                //Add any new parameters
                foreach (var p in ToAdd)
                {

                    var newParameter = new Parameter
                    {
                        Name = p.Name,
                        Frequency = p.Frequency,
                        Type = context.ParameterTypes.First(x => x.Id == p.ParameterTypeId),
                        Unit = p.Unit,
                        Source = p.Source,
                        Link = p.Link,
                        Use = p.Use
                    };

                    foreach (var b in p.Bounds)
                    {

                        var site = context.Sites.FirstOrDefault(x => x.Id == b.SiteId);
                        var newBound = new ParameterBound
                        {
                            Type = b.Type,
                            Range = b.Range,
                            MinValue = b.MinValue,
                            MaxValue = b.MaxValue,
                            MinDescription = b.MinDescription,
                            MaxDescription = b.MaxDescription,
                            IsEnforced = b.IsEnforced
//                            Site = site
                        };
                        newParameter.ParameterBounds.Add(newBound);
                    }

                    systemType.Parameters.Add(newParameter);

                }

                context.SaveChanges();

                TempData["CreateSuccess"] = Model.Name + " was updated successfully";
                return RedirectToAction("Index");
            }

            if (nameExists) {
                ModelState.AddModelError("", String.Format("A system type with the name {0} already exists.  Please choose a unique site name.", Model.Name));
                ViewBag.Data = Model;
                ViewBag.ParameterTypes = context.ParameterTypes.Select(x => new { Id = x.Id, Name = x.Name });
                ViewBag.Sites = context.Sites.Select(x => new { Id = x.Id, Name = x.Name });
                return View();
            }

            return Edit(Model.Id);

        }

        public JsonResult Search(SearchModel Criteria)
        {

            var repo = context.SystemTypes;
            IQueryable<SystemType> filteredSystems;

            Criteria.SortBy = Criteria.SortBy ?? String.Empty;

            //The nearly duplicated code is a limitation to base Linq to SQL and I don't want to write an extension right now
            switch (Criteria.SortBy)
            {
                case "Name":
                default:
                    if (Criteria.ShouldForwardSearch)
                        filteredSystems = repo.OrderBy((x) => x.Name);
                    else
                        filteredSystems = repo.OrderByDescending((x) => x.Name);
                    break;
                case "HasParameters":
                    if (Criteria.ShouldForwardSearch)
                        filteredSystems = repo.OrderBy((x) => x.Parameters.Any());
                    else
                        filteredSystems = repo.OrderByDescending((x) => x.Parameters.Any());
                    break;
                case "InUse":
                    if (Criteria.ShouldForwardSearch)
                        filteredSystems = repo.OrderBy((x) => x.BuildingSystems.Any());
                    else
                        filteredSystems = repo.OrderByDescending((x) => x.BuildingSystems.Any());
                    break;
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["Name"]))
            {
                var name = Criteria.Filters["Name"];
                filteredSystems = filteredSystems.Where(x => x.Name.Contains(name));
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["HasParameters"]) && Criteria.Filters["HasParameters"] != "Any")
            {
                var hasParameters = Criteria.Filters["HasParameters"] == "Yes" ? true : false;
                filteredSystems = filteredSystems.Where(x => x.Parameters.Any() == hasParameters);
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["InUse"]) && Criteria.Filters["InUse"] != "Any")
            {
                var inUse = Criteria.Filters["InUse"] == "Yes" ? true : false;
                filteredSystems = filteredSystems.Where(x => x.BuildingSystems.Any() == inUse);
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["IsActive"]) && Criteria.Filters["IsActive"] != "all")
            {
                var isActive = Convert.ToBoolean(Criteria.Filters["IsActive"]);
                filteredSystems = filteredSystems.Where(x => x.IsActive == isActive);
            }

            var total = filteredSystems.Count();

            var systems = filteredSystems.Skip(Criteria.Offset).Take(Criteria.MaxResults);

            var data = new SearchResult<SystemModel>
            {
                Results = systems.Select(x => new SystemModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    HasParameters = x.Parameters.Any() ? "Yes" : "No",
                    InUse = x.BuildingSystems.Any() ? "Yes" : "No"
                }).ToList(),
                Total = total
            };

            return Json(data, JsonRequestBehavior.AllowGet);

        }

    }

}
