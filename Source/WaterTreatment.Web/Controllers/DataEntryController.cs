using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using WaterTreatment.Web.Attributes;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Extensions;
using WaterTreatment.Web.Models;
using WaterTreatment.Web.Services;
using WaterTreatment.Web.Services.Interface;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace WaterTreatment.Web.Controllers
{

    [SectionAuthorize]
    public class DataEntryController : BaseController
    {
        private readonly UserManager<User, int> userManager;
        private readonly IAWSS3 _s3Storage;
        private readonly IFileStorage _storage;
        private readonly IUserService _userService;
        private readonly IReportService _reportService;

        public DataEntryController(WTContext context, UserManager<User, int> userManager, IUserService userService, IReportService reportService, IFileStorage storage, IAWSS3 s3Storage)
            : base(context)
        {
            this.userManager = userManager;
            _storage = storage;
            _userService = userService;
            _reportService = reportService;
            _s3Storage = s3Storage;
        }

        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {

            var isDataRecorder = CurrentUser.Roles.Any(x => x.RoleId == context.Ref.Roles.DataRecorder.Id);
            var isAuditRecorder = CurrentUser.Roles.Any(x => x.RoleId == context.Ref.Roles.AuditRecorder.Id);
            var isAdmin = CurrentUser.Roles.Any(x => x.RoleId == context.Ref.Roles.SystemAdministrator.Id);

            if ((isDataRecorder && isAuditRecorder) || isAdmin || _userService.IsSiteAdmin())
                ViewBag.HasMultipleUse = true;

            var allowedSites = _userService.GetActiveSites();

            ViewBag.Sites = allowedSites.Select(x => new { Id = x.Id, Name = x.Name }).OrderBy(s => s.Name).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReportStartModel Model)
        {

            var isDataRecorder = CurrentUser.Roles.Any(x => x.RoleId == context.Ref.Roles.DataRecorder.Id);
            var isAuditRecorder = CurrentUser.Roles.Any(x => x.RoleId == context.Ref.Roles.AuditRecorder.Id);
            var isAdmin = _userService.IsAdmin() || _userService.IsSiteAdmin();

            if (ModelState.IsValid)
            {
                var site = context.Sites.FirstOrDefault(x => x.Id == Model.SiteId);

                if (site != null)
                {
                    var R = new Report();

                    R.User = CurrentUser;
                    R.Site = site;
                    R.CreationDate = DateTime.UtcNow;
                    R.SubmissionDate = null;

                    var userId = CurrentUser.Id;

                    if ((isDataRecorder && isAuditRecorder) || isAdmin)
                        R.ReportType = Model.Use;
                    else if (isDataRecorder)
                        R.ReportType = "Measurement";
                    else if (isAuditRecorder)
                        R.ReportType = "Independent Quality Assurance";

                    foreach (var building in site.Buildings.Where(x => x.IsActive))
                    {
                        //var BR = new BuildingReport();
                        //BR.Building = building;

                        //Admins can access everything, otherwise filter based on user access
                        foreach (var system in building.Systems.Where(x => isAdmin || x.Users.Any()))
                        {
                            if (!system.IsActive)
                            {
                                continue;
                            }
                            var SM = new SystemMeasurement();
                            SM.BuildingSystem = system;

                            foreach (var parameter in system.SystemType.Parameters.Where(x => x.Use == R.ReportType || x.Use == "Both"))
                            {
                                var M = new Measurement();
                                M.Value = String.Empty;
                                M.Comment = String.Empty;
                                M.Parameter = parameter;
                                M.IsApplicable = true;
                               // M.Parameter.AltParameter = -1;
                                SM.Measurements.Add(M);
                            }

                            if (SM.Measurements.Any())
                            {
                                R.SystemMeasurements.Add(SM);
                            }
                        }

                        //if (BR.SystemMeasurements.Any())
                        //{
                        //    R.SystemMeasurements.Add(BR);
                        //}
                    }
                    context.Reports.Add(R);
                    context.SaveChanges();
                    return RedirectToAction("Edit", new { Id = R.Id });
                }
                else
                {
                    ModelState.AddModelError("SiteId", "The selected site does not exist.");
                }
            }


            if ((isDataRecorder && isAuditRecorder) || isAdmin)
                ViewBag.HasMultipleUse = true;

            var allowedSites = _userService.GetActiveSites();

            ViewBag.Sites = allowedSites.Select(x => new { Id = x.Id, Name = x.Name }).ToList();
            return View();
        }

        private bool IsNumeric(string input)
        {
            bool returnVal = false;
            int testInt;
            float testFloat;
            returnVal = int.TryParse(input, out testInt)||float.TryParse(input, out testFloat);
            return returnVal;
        }

        private void SaveReport(Report Report, ReportModel Model)
        {
            Report.MeasurementDate = DateTime.Parse(Model.MeasurementDate);
            Report.Notes = Model.Notes;

            foreach (var b in Model.Buildings)
            {
                //var BR = Report.SystemMeasurements.First(x => x.Id == b.Id);  // todo
                foreach (var s in b.Systems)
                {
                    var SM = Report.SystemMeasurements.First(x => x.Id == s.Id);
                    int buildingSystemId = SM.BuildingSystem.Id;

                    if (!string.IsNullOrEmpty(s.ReasonSkipped))
                    {
                        SM.ReasonSkipped = s.ReasonSkipped;
                    }
                    else
                    {
                        SM.ReasonSkipped = s.ReasonSkipped;

                        foreach (var m in s.Measurements)
                        {
                            if (m.IsAdhoc)
                            {
                                int typeVal = IsNumeric(m.Value) ? 2 : 1;
                                var AHP = SaveAdhocParameter(m.Unit, m.Source, m.Name, typeVal, buildingSystemId);
                                if ((typeVal == 2) && (m.MinValue != null || m.MaxValue != null))
                                {
                                    SaveParameterBound(m.Range, "Value", m.MinValue, m.MaxValue, AHP);
                                }
                                SaveAdhocMeasurement(m.Value, m.Comment, AHP, SM.Id);
                            }
                            else
                            {
                                var measurement = SM.Measurements.First(x => x.Id == m.Id);
                                measurement.Value = m.Value;
                                measurement.IsApplicable = m.IsApplicable;
                                measurement.Comment = m.Comment;
                                BakeOOB(measurement, Report);
                            }
                        }
                    }
                }
            }

            context.SaveChanges();
        }

        private AdhocParameter SaveAdhocParameter(string unit, string source, string name, int type, int buildingSystemId)
        {
            AdhocParameter ahp = context.AdhocParameters.FirstOrDefault(x => x.BuildingSystem.Id == buildingSystemId && x.Name == name && x.Source == source);
            if (ahp == null)
            {
                ahp = new AdhocParameter();
                ahp.Unit = unit;
                ahp.Source = source;
                ahp.Name = name;
                ahp.Type = context.ParameterTypes.FirstOrDefault(PR => PR.Id == type);
                ahp.BuildingSystem = context.BuildingSystems.FirstOrDefault(x => x.Id == buildingSystemId);
                context.AdhocParameters.Add(ahp);
            }
            else
            {
                ahp.Unit = unit;
                ahp.Source = source;
                ahp.Name = name;
            }
            return ahp;
        }

        private void SaveParameterBound(string range, string type, decimal? minValue, decimal? maxValue, AdhocParameter adhocParameter)
        {
            ParameterBound APB = adhocParameter.ParameterBounds.FirstOrDefault();
            if (APB == null)
            {
                APB = new ParameterBound();
                APB.IsEnforced = true;
                APB.MaxValue = maxValue;
                APB.MinValue = minValue;
                APB.Type = type;
                APB.Range = range;
                APB.AdhocParameter = adhocParameter;
                adhocParameter.ParameterBounds.Add(APB);
            }
            else
            {
                APB.IsEnforced = true;
                APB.MaxValue = maxValue;
                APB.MinValue = minValue;
                APB.Type = type;
                APB.Range = range;
                APB.AdhocParameter = adhocParameter;
            }
        }

        private void SaveAdhocMeasurement(string value, string comment, AdhocParameter adhocParameter, int smId)
        {
            AdhocMeasurement AHM = context.AdhocMeasurements.FirstOrDefault(h => h.AdhocParameter.Id == adhocParameter.Id && h.SystemMeasurement.Id == smId);
            if (AHM == null)
            {
                AHM = new AdhocMeasurement();
                AHM.Value = value;
                AHM.Comment = comment;
                AHM.IsApplicable = true;
                AHM.SystemMeasurement = context.SystemMeasurements.FirstOrDefault(smt => smt.Id == smId);
                AHM.AdhocParameter = adhocParameter;
                adhocParameter.AdhocMeasurements.Add(AHM);
            }
            else
            {
                AHM.Value = value;
                AHM.Comment = comment;
                AHM.IsApplicable = true;
            }
        }

        private void BakeOOB(Measurement measurement, Report Report)
        {

            double value = 0d;

            //Skipped measurements cannot be OOB
            if (!measurement.IsApplicable)
                return;

            //Grab the first
            var PB = measurement.Parameter.ParameterBounds.FirstOrDefault();

            //If there are no global bounds try and grab the first local bound
            //if (PB == null)
            //{
            //    PB = measurement.Parameter.ParameterBounds.FirstOrDefault(x => x.Site == Report.Site);
            //}

            //If no bound then it cannot possibly be OOB
            if (PB == null)
            {
                measurement.BakedOOB = false;
            }
            else if (Double.TryParse(measurement.Value, out value))//Check if it's OOB
            {

                if (PB.Range == "Minimum")
                    measurement.BakedOOB = value < Convert.ToDouble(PB.MinValue);
                if (PB.Range == "Maximum")
                    measurement.BakedOOB = value > Convert.ToDouble(PB.MaxValue);
                if (PB.Range == "Both")
                    measurement.BakedOOB = value < Convert.ToDouble(PB.MinValue) || value > Convert.ToDouble(PB.MaxValue);

            }

        }
       

        [HttpGet]
        public ActionResult Edit(int? Id)
        {
    // var xxx=   GetParameterNamebyID(326);
            if (!Id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var Report = context.Reports.FirstOrDefault(x => x.Id == Id);

            if (Report == null)
            {
                TempData["Error"] = "Invalid Id";
                return RedirectToAction("Index");
            }

            if (Report.SubmissionDate.HasValue)
            {
                TempData["Error"] = "This report has already been submitted";
                return RedirectToAction("Index");
            }

            if (Report.User.Id != CurrentUser.Id)
            {
                TempData["Error"] = "You cannot edit someone else's report";
                return RedirectToAction("Index");
            }


            return View(_reportService.BuildReportModel2(Report));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReportModel Model)
        {

            var Report = context.Reports.FirstOrDefault(x => x.Id == Model.Id);

            if (Report == null)
            {
                TempData["Error"] = "Invalid Id";
                return RedirectToAction("Index");
            }

            if (Report.SubmissionDate.HasValue)
            {
                TempData["Error"] = "This report has already been submitted";
                return RedirectToAction("Index");
            }
             
            if (Report.User.Id != CurrentUser.Id)
            {
                TempData["Error"] = "You cannot edit someone else's report";
                return RedirectToAction("Index");
            }
            //clear model errors due to empty(null) min max values 
            clearModelErrorsDueToJavaScript();

            if (ModelState.IsValid)
            {
                SaveReport(Report, Model);

                TempData["CreateSuccess"] = "The report was updated successfully. Note that it was only saved not submitted.";
                return RedirectToAction("Index");

            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
            }

            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSubmit(ReportModel Model)
        {

            var Report = context.Reports.FirstOrDefault(x => x.Id == Model.Id);

            if (Report == null)
            {
                TempData["Error"] = "Invalid Id";
                return RedirectToAction("Index");
            }

            if (Report.SubmissionDate.HasValue)
            {
                TempData["Error"] = "This report has already been submitted";
                return RedirectToAction("Index");
            }

            if (Report.User.Id != CurrentUser.Id)
            {
                TempData["Error"] = "You cannot edit someone else's report";
                return RedirectToAction("Index");
            }
            //clear model errors due to empty(null) min max values 
            clearModelErrorsDueToJavaScript();
            if (ModelState.IsValid)
            {

                if (!Report.Attachments.Any())
                {
                    TempData["Error"] = "Cannot submit a report without an attachment.";
                    return RedirectToAction("Submit", new { Id = Report.Id });
                }

                SaveReport(Report, Model);
                return RedirectToAction("Submit", new { Id = Report.Id });

            }

            return View(Model);
        }

        private bool IsSubmissionReady(Report Report)
        {
            return !Report.SystemMeasurements.All(y => String.IsNullOrWhiteSpace(y.ReasonSkipped) && y.Measurements.All(z => String.IsNullOrWhiteSpace(z.Value)) && y.Measurements.All(z => z.IsApplicable));
        }

        [HttpGet]
        public ActionResult Submit(int Id)
        {

            var Report = context.Reports.FirstOrDefault(x => x.Id == Id);

            if (Report == null)
            {
                TempData["Error"] = "Invalid Id";
                return RedirectToAction("Index");
            }

            if (Report.SubmissionDate.HasValue)
            {
                TempData["Error"] = "This report has already been submitted";
                return RedirectToAction("Index");
            }

            if (!IsSubmissionReady(Report))
            {
                TempData["Warning"] = "Not Ready to Submit";
                return RedirectToAction("Edit", new { Id = Report.Id });
            }

            if (Report.User.Id != CurrentUser.Id)
            {
                TempData["Error"] = "You cannot submit someone else's report";
                return RedirectToAction("Index");
            }

            return View(_reportService.BuildViewReportModel(Report));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(ReportModel Model)
        {

            var Report = context.Reports.FirstOrDefault(x => x.Id == Model.Id);

            if (Report == null)
            {
                TempData["Error"] = "Invalid Id";
                return RedirectToAction("Index");
            }

            if (!IsSubmissionReady(Report))
            {
                TempData["Warning"] = "Not Ready to Submit";
                return RedirectToAction("Edit", new { Id = Report.Id });
            }

            if (Report.User.Id != CurrentUser.Id)
            {
                TempData["Error"] = "You cannot submit someone else's report";
                return RedirectToAction("Index");
            }

            if (!Report.Attachments.Any())
            {
                TempData["Error"] = "You cannot submit a report without an attachment.";
                return RedirectToAction("Edit", new { Id = Report.Id });
            }

            Report.SubmissionDate = DateTime.UtcNow;
            context.SaveChanges();

            TempData["Success"] = "The report has been submitted on " + DateTime.UtcNow + " (UTC)";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetInProgressReports(SearchModel criteria)
        {
            IQueryable<Report> filteredReports = context.Reports.Where(r => r.User.Id == CurrentUser.Id && !r.SubmissionDate.HasValue);

            switch (criteria.SortBy ?? string.Empty)
            {
                case "Site":
                    filteredReports = filteredReports.SortBy(x => x.Site.Name.ToLower(), criteria.ShouldForwardSearch);
                    break;
                case "Type":
                    filteredReports = filteredReports.SortBy(x => x.ReportType, criteria.ShouldForwardSearch);
                    break;
                default:
                    filteredReports = filteredReports.SortBy(x => x.Id.ToString(), criteria.ShouldForwardSearch);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["Type"]))
            {
                var type = criteria.Filters["Type"].ToLower();
                filteredReports = filteredReports.Where(x => x.ReportType.ToLower() == type || type == "Both"); 
            }

            var total = filteredReports.Count();
            var reports = filteredReports.Skip(criteria.Offset).Take(criteria.MaxResults);

            var data = new SearchResult<ReportViewModel>
            {
                Results = reports.ToList().Select(x => new ReportCopiedViewModel
                {
                    Id = x.Id,
                    Site = x.Site.Name,
                    Type = x.ReportType,
                    StartedOn = DateTime.SpecifyKind(x.CreationDate, DateTimeKind.Utc),
                    Copied = x.CopiedFrom,
                    DaysElapsed = (DateTime.Now - x.CreationDate).Days
                }),
                Total = total
            };

            return Json(data);

        }

        [HttpGet]
        [Route("Attachments/Download/{id}")]
        public ActionResult Download(int id)
        {
            var attachment = context.Attachments.Find(id);
            string storageType = ConfigurationManager.AppSettings["StorageType"].ToUpper();
            if (attachment == null || !_userService.CanViewReport(attachment.Report))
            {
                return new EmptyResult();
            }

            if (storageType == Constants.StorageType_AWSS3)
            {
                return File(_s3Storage.GetFile(attachment.StorageId), "application/force-download", attachment.Name);
            }
            else //(storageType == Constants.StorageType_FILE)
            {
                return File(_storage.Get(attachment.StorageId), "application/force-download", attachment.Name);
            }
        }

        [HttpGet]
        [ActionName("GetAll")]
        [Route("Attachments/{id}")]
        public JsonResult GetAllReportAttachments(int id)
        {
            var report = context.Reports.SingleOrDefault(r => r.Id == id);
            if (report != null)
            {
                return Json(report.Attachments.Select(a => new { Id = a.Id, Name = a.Name }), JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Attachments/Add/{id}")]
        public ActionResult Add(int id)
        {
            var report = context.Reports.SingleOrDefault(r => r.Id == id);
            string storageType = ConfigurationManager.AppSettings["StorageType"].ToUpper();

            if (report != null && Request.Files.Count > 0)
            {
                var data = Request.Files.Get(0);

                var extension = GetExtension(data.FileName);

                if (!context.ExtensionWhitelist.Any(x => x.Name == extension))
                {
                    return Json(new { Error = "Invalid file extension '" + data.FileName + "'." });
                }

                if (context.Attachments.Any(a => a.Name == data.FileName && a.Report.Id == id))
                {
                    return Json(new { Error = "Cannot upload file with duplicate name '" + data.FileName + "'." });
                }

                var attachment = new Attachment { Name = data.FileName };
                // Add Conditional for type of file storage
                if (storageType == Constants.StorageType_AWSS3)
                {
                    attachment.StorageId = _s3Storage.UploadFile(data.InputStream);
                }
                else if (storageType == Constants.StorageType_FILE)
                {
                    attachment.StorageId = _storage.Add(data.InputStream);
                }

                report.Attachments.Add(attachment);
                context.SaveChanges();

                return Json(new { Id = attachment.Id, Name = attachment.Name });
            }

            return Json(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Attachments/Update/{id}")]
        public ActionResult Update(int id)
        {
            var attachment = context.Attachments.SingleOrDefault(a => a.Id == id);
            string storageType = ConfigurationManager.AppSettings["StorageType"].ToUpper();

            if (attachment != null && Request.Files.Count > 0)
            {
                var data = Request.Files.Get(0);

                var extension = GetExtension(data.FileName);

                if (!context.ExtensionWhitelist.Any(x => x.Name == extension))
                {
                    return Json(new { Error = "Invalid file extension '" + data.FileName + "'." });
                }

                var originalExt = GetExtension(attachment.Name);
                if (extension != originalExt)
                {
                    return Json(new { Error = "File extension differs from original extension '" + originalExt + "'." });
                }

                if (storageType == Constants.StorageType_AWSS3)
                {
                    _s3Storage.Update(attachment.StorageId, data.InputStream);
                }
                else if (storageType == Constants.StorageType_FILE)
                {
                    _storage.Update(attachment.StorageId, data.InputStream);
                }

                return Json(new { Id = attachment.Id, Name = attachment.Name });
            }

            return Json(false);
        }

        private string GetExtension(string fileName)
        {
            if (fileName.Contains("."))
            {
                return fileName.Substring(fileName.LastIndexOf(".")).ToLower();
            }
            return "";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Attachments/Remove/{id}")]
        public ActionResult Remove(int id)
        {
            var attachment = context.Attachments.SingleOrDefault(a => a.Id == id);
            string storageType = ConfigurationManager.AppSettings["StorageType"].ToUpper();
            if (attachment != null)
            {
                if (storageType == Constants.StorageType_AWSS3)
                {
                    _s3Storage.Remove(attachment.StorageId);
                }
                else if (storageType == Constants.StorageType_FILE)
                {
                    _storage.Remove(attachment.StorageId);
                }
                context.Attachments.Remove(attachment);
                context.SaveChanges();
            }

            return Json(true);
        }


        [HttpGet]
        //[ValidateAntiForgeryToken]
        [Route("Attachments/RemoveAdhoc/{id}")]
        public ActionResult RemoveAdhoc(int id)
        {
            AdhocMeasurement AHM = context.AdhocMeasurements.FirstOrDefault(h => h.Id == id);
            if (AHM != null)
            {
                context.AdhocMeasurements.Remove(AHM);
                context.SaveChanges();
            }
            return Json(true);
        }
        /// <summary>
        /// This method romoves model errors added by min max values
        /// When min max values are null the javascript introduces following errors
        /// The value 'null' is not valid for MinValue.
        /// The value 'null' is not valid for MaxValue.
        /// </summary>
        /// <param name="modelState"></param>
        private void clearModelErrorsDueToJavaScript()
        {
            List<string> listErrorKey = new List<string>();
            foreach (var modelStateKey in ViewData.ModelState.Keys)
            {
                var modelStateVal = ViewData.ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    var key = modelStateKey;
                    var errorMessage = error.ErrorMessage;
                    //var exception = error.Exception;
                    listErrorKey.Add(key);
                }
            }

            foreach (string key in listErrorKey)
            {
                if (ModelState.ContainsKey(key))
                    ModelState[key].Errors.Clear();
            }

        }


        [HttpGet]
        public ActionResult CopyReport(string site)
        {
            string shouldCopyReports = ConfigurationManager.AppSettings["CopyReports"].ToUpper();
            if (shouldCopyReports.ToLower() == "true")
            {
                //set values for filter
                var startDate = DateTime.Now.AddMonths(-2).ToShortDateString();
                var endDate = DateTime.Now.ToShortDateString();

                var initialState = new
                {
                    ShowCreatedBy = _userService.IsAdmin() || _userService.IsSiteAdmin() || _userService.IsDataRecorder(),    // These roles can see others' reports
                    ShowAdminActions = _userService.IsAdmin() || _userService.IsSiteAdmin() || _userService.IsDataRecorder(),
                    Filters = new
                    {
                        MeasurementDateStart = startDate,
                        MeasurementDateEnd = endDate,
                        Site = string.IsNullOrWhiteSpace(site) ? null : new List<int> { Convert.ToInt32(site) }
                    }
                };
                List<Site> sites;
                if (_userService.IsAdmin())
                {
                    sites = context.Sites.OrderBy(s => s.Name).ToList();
                }
                else
                {
                    sites = CurrentUser.userSiteAccess.Select(s => s).OrderBy(s => s.Name).ToList();
                }

                ViewBag.InitialState = JsonConvert.SerializeObject(initialState);
                ViewBag.Sites = JsonConvert.SerializeObject(sites);
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public JsonResult CopyReport(int id)
        {
            int affectedRows = 0;
            string shouldCopyReports = ConfigurationManager.AppSettings["CopyReports"].ToUpper();
            if (shouldCopyReports.ToLower() == "true")
            {
                try
                {
                    using (var context = new WTContext())
                    {
                        SqlParameter reportCopiedFrom = new SqlParameter("@ReportCopiedFrom", SqlDbType.Int);
                        reportCopiedFrom.Value = id;
                        SqlParameter userId = new SqlParameter("@User_Id", SqlDbType.Int);
                        userId.Value = User.Identity.GetUserId<int>();
                        affectedRows = context.Database.ExecuteSqlCommand("spCopyReportWithReportData @User_Id, @ReportCopiedFrom", userId, reportCopiedFrom);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.BannerMessage = ex.Message;
                    ViewBag.BannerClass = "alert-danger";
                }
            }
            return Json(affectedRows);
        }

    }


}
