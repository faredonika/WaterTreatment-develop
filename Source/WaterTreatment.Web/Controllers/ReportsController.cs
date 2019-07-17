using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WaterTreatment.Web.Attributes;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Extensions;
using WaterTreatment.Web.Models;
using WaterTreatment.Web.Services;
using WaterTreatment.Web.Services.Interface;

namespace WaterTreatment.Web.Controllers
{

    [SectionAuthorize]
    public class ReportsController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IPDFService _pdfService;
        private readonly IFileStorage _storage;
        private readonly IAWSS3 _s3Storage;

        public ReportsController(WTContext context, IReportService reportService, IUserService userService, IEmailService emailService, IPDFService pdfService, IFileStorage storage, IAWSS3 s3Storage)
            : base(context)
        {
            _reportService = reportService;
            _userService = userService;
            _emailService = emailService;
            _pdfService = pdfService;
            _storage = storage;
            _s3Storage = s3Storage;
        }

        [Route("Index")]
        public ActionResult Index(string from, string to, string site)
        {
            DateTime parseDate;
            var startDate = string.Empty;
            var endDate = string.Empty;

            if (DateTime.TryParse(from, out parseDate))
                startDate = parseDate.ToShortDateString();
            if (DateTime.TryParse(to, out parseDate))
                endDate = parseDate.ToShortDateString();

            var initialState = new
            {
                ShowCreatedBy = _userService.IsAdmin() || _userService.IsSiteAdmin() || _userService.IsExecutiveReportViewer() || _userService.IsReportViewer(),    // These roles can see others' reports
                ShowAdminActions = _userService.IsAdmin() || _userService.IsSiteAdmin(),
                Subscriptions = GetSubscribedSiteIds(_userService.CurrentUser),
                Filters = new
                {
                    MeasurementDateStart = startDate,
                    MeasurementDateEnd = endDate,
                    Site = string.IsNullOrWhiteSpace(site) ? null : new List<int> { Convert.ToInt32(site) }
                }
            };
            List<Site> sites;
            if (_userService.IsAdmin() || _userService.IsExecutiveReportViewer())
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

        [HttpPost]
        public JsonResult SearchReports(SearchModel criteria)
        {
            IQueryable<Report> filteredReports = context.Reports.Where(r => r.SubmissionDate.HasValue).AsQueryable();

            var currentUserId = CurrentUser.Id;
            var isSysAdmin = _userService.IsAdmin();
            var isSiteAdmin = _userService.IsSiteAdmin();
            var siteAdminSites = new List<int>();
            if (isSiteAdmin)
            {
                siteAdminSites = CurrentUser.userSiteAccess.Select(s => s.Id).ToList();
                filteredReports = filteredReports.Where(r => r.User.Id == currentUserId || siteAdminSites.Any(s => r.Site.Id == s));
            }
            else if (_userService.IsReportViewer()|| _userService.IsDataRecorder())
            {
                var userSites = CurrentUser.userSiteAccess.Select(s => s.Id);
                filteredReports = filteredReports.Where(r => r.User.Id == currentUserId || userSites.Any(s => r.Site.Id == s) && r.SubmissionDate.HasValue);
            }
            else if (_userService.IsExecutiveReportViewer())
            {
                filteredReports = filteredReports.Where(r => r.User.Id == currentUserId || r.SubmissionDate.HasValue);
            }
            else if (!isSysAdmin)
            {
                filteredReports = filteredReports.Where(r => r.User.Id == currentUserId);
            }


            if (!string.IsNullOrWhiteSpace(criteria.Filters["Sites"]))
            {
                var siteIds = criteria.Filters["Sites"].Split(',').Select(v => Convert.ToInt32(v));
                filteredReports = filteredReports.Where(x => siteIds.Any(s => x.Site.Id == s));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["MeasurementDateStart"]))
            {
                var measurementStartStr = criteria.Filters["MeasurementDateStart"];
                DateTime measurementStart;
                if (DateTime.TryParse(measurementStartStr, out measurementStart))
                {
                    // The call to ToUniversalTime() is mostly to make it clearer during debugging that we're comparing UTC to UTC (i.e. rather than 19:00ET to 00:00UTC, which are equivalent depending on DST)
                    measurementStart = measurementStart.ToUniversalTime();
                    filteredReports = filteredReports.Where(r => r.MeasurementDate >= measurementStart);
                }
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["MeasurementDateEnd"]))
            {
                var measurementEndStr = criteria.Filters["MeasurementDateEnd"];
                DateTime measurementEnd;
                if (DateTime.TryParse(measurementEndStr, out measurementEnd))
                {
                    measurementEnd = measurementEnd.ToUniversalTime();
                    var startOfNextDay = measurementEnd.AddDays(1);     // can't do this in the Linq Entities query
                    filteredReports = filteredReports.Where(r => r.MeasurementDate < startOfNextDay);
                }
            }

            var now = DateTime.UtcNow;

            switch (criteria.SortBy ?? string.Empty)
            {
                case "Site":
                    filteredReports = filteredReports.SortBy(x => x.Site.Name, criteria.ShouldForwardSearch);
                    break;
                case "Type":
                    filteredReports = filteredReports.SortBy(x => x.ReportType, criteria.ShouldForwardSearch);
                    break;
                case "StartedOn":
                    filteredReports = filteredReports.SortBy(x => x.CreationDate, criteria.ShouldForwardSearch);
                    break;
                case "MeasuredOn":
                    filteredReports = filteredReports.SortBy(x => x.MeasurementDate.Value, criteria.ShouldForwardSearch);
                    break;
                case "SubmittedOn":
                    filteredReports = filteredReports.SortBy(x => x.SubmissionDate.HasValue ? x.SubmissionDate.Value : now, criteria.ShouldForwardSearch);
                    break;
                default:
                    filteredReports = filteredReports.OrderByDescending(x => x.UnsubmitRequestedDate).ThenByDescending(x => x.SubmissionDate).ThenByDescending(x => x.MeasurementDate);
                    break;
            }

            var total = filteredReports.Count();
            var reports = filteredReports.Skip(criteria.Offset).Take(criteria.MaxResults).ToList();
            var data = new SearchResult<ReportSummaryViewModel>
            {
                Results = reports.Select(x => new ReportSummaryViewModel
                {
                    Id = x.Id,
                    Site = x.Site.Name,
                    Type = x.ReportType,
                    StartedOn = DateTime.SpecifyKind(x.CreationDate, DateTimeKind.Utc),
                    SubmittedOn = x.SubmissionDate.HasValue ? DateTime.SpecifyKind(x.SubmissionDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                    MeasuredOn = x.MeasurementDate.HasValue ? x.MeasurementDate.Value.ToShortDateString() : string.Empty,
                    CreatedBy = String.Format("{0} {1} ({2})", x.User.FirstName, x.User.LastName, x.User.UserName),
                    Vendor = x.User.Vendor != null ? x.User.Vendor.Name : null,
                    CanRequestUnsubmit = x.User.Id == currentUserId && x.SubmissionDate.HasValue,
                    HasRequestedUnsubmit = _reportService.HasRequestedUnsubmit(x),
                    CanUnsubmit = x.SubmissionDate.HasValue && (isSysAdmin || isSiteAdmin && siteAdminSites.Any(s => s == x.Site.Id))
                }),
                Total = total
            };

            return Json(data);

        }

        public ActionResult View(int? Id)
        {

            var Report = context.Reports.FirstOrDefault(x => x.Id == Id);

            if (Report == null)
            {
                TempData["Error"] = "Invalid Id";
                return RedirectToAction("Index");
            }

            if (!_userService.CanViewReport(Report))
            {
                TempData["Error"] = "You do not have sufficient permissions to view this report";
                return RedirectToAction("Index");
            }

            return View(_reportService.BuildViewReportModel(Report));
        }

        [HttpPost]
        [ActionName("Unsubmit")]
        public ActionResult Unsubmit(int ReportId)
        {
            ValidateTokens();
            try
            {
                var report = context.Reports.FirstOrDefault(r => r.Id == ReportId);
                if (report == null)
                {
                    throw new Exception("Invalid report ID");
                }

                if (!(_userService.IsAdmin() || _userService.IsSiteAdmin() && CurrentUser.userSiteAccess.Select(s => s).Any(s => s.Id == report.Site.Id)))
                {
                    throw new Exception("Permission denied");
                }

                if (!report.SubmissionDate.HasValue)
                {
                    throw new Exception("The requested report is not currently submitted");
                }

                report.SubmissionDate = null;
                report.UnsubmitRequestedDate = null;
                context.SaveChanges();
                ViewBag.BannerMessage = String.Format("Successfully unsubmitted report ({0} - {1})", report.Site.Name, report.MeasurementDate.Value.ToShortDateString());
                ViewBag.BannerClass = "alert-success";
            }
            catch (Exception e)
            {
                ViewBag.BannerMessage = e.Message;
                ViewBag.BannerClass = "alert-danger";
            }
            return PartialView("_Banner");
        }

        [HttpPost]
        [ActionName("RequestUnsubmit")]
        public ActionResult RequestUnsubmit(int ReportId)
        {
            ValidateTokens();
            try
            {
                var report = context.Reports.FirstOrDefault(r => r.Id == ReportId);
                if (report == null)
                {
                    throw new Exception("Invalid report ID");
                }

                if (CurrentUser.Id != report.User.Id)
                {
                    throw new Exception("Permission denied");
                }

                if (!report.SubmissionDate.HasValue)
                {
                    throw new Exception("The requested report is not currently submitted");
                }

                if (_reportService.HasRequestedUnsubmit(report))
                {
                    throw new Exception("The requested report already has a pending Unsubmit request");
                }

                report.UnsubmitRequestedDate = DateTime.UtcNow;
                context.SaveChanges();

                var siteAdmins = context.Users.Where(u => !String.IsNullOrEmpty(u.Email) && u.Roles.Any(r => r.RoleId == context.Ref.Roles.SiteAdministrator.Id) && u.userSiteAccess.Any(s => s.Id == report.Site.Id));
                var recipients = siteAdmins.Count() > 0 ? siteAdmins : context.Users.Where(u => u.Roles.Any(r => r.RoleId == context.Ref.Roles.SystemAdministrator.Id));

                string Url = "https://" + Request.Url.Host + "/Reports";
                var nl = "<br />";
                string Body = String.Format("A user has requested that a report be unsubmitted." + nl
                    + "User: {0} {1} ({2})" + nl
                    + "\r\nReport Site: {3}" + nl
                    + "\r\nDate Submitted: {4}" + nl
                    + nl
                    + "\r\n<a href=\"{5}\">View Reports</a>"
                    , CurrentUser.FirstName, CurrentUser.LastName, CurrentUser.UserName, report.Site.Name, report.SubmissionDate.HasValue ? report.SubmissionDate.Value.ToShortDateString() : "", Url);

                _emailService.Send(recipients.Select(r => r.Email), new List<string>(), new List<string>(), "Water Treatment: Request to Unsubmit Report", Body);

                ViewBag.BannerMessage = String.Format("Successfully sent Unsubmit request for report ({0} - {1})", report.Site.Name, report.MeasurementDate.Value.ToShortDateString());
                ViewBag.BannerClass = "alert-success";
            }
            catch (Exception e)
            {
                ViewBag.BannerMessage = e.Message;
                ViewBag.BannerClass = "alert-danger";
            }
            return PartialView("_Banner");
        }

        [HttpPost]
        public ActionResult DismissUnsubmit(int ReportId, string Comments)
        {
            ValidateTokens();
            try
            {
                var report = context.Reports.FirstOrDefault(r => r.Id == ReportId);
                if (report == null)
                {
                    throw new Exception("Invalid report ID");
                }

                if (!(_userService.IsAdmin() || _userService.IsSiteAdmin() && CurrentUser.userSiteAccess.Select(s => s).Any(s => s.Id == report.Site.Id)))
                {
                    throw new Exception("Permission denied");
                }

                if (!report.SubmissionDate.HasValue)
                {
                    throw new Exception("The requested report is not currently submitted");
                }

                if (!report.UnsubmitRequestedDate.HasValue)
                {
                    throw new Exception("The requested report does not currently have a Unsubmit Request pending");
                }

                if (String.IsNullOrEmpty(Comments))
                {
                    throw new Exception("You must provide comments back to the author in order to dismiss the Unsubmit request");
                }

                report.UnsubmitRequestedDate = null;
                context.SaveChanges();

                var encodedComments = HttpUtility.HtmlEncode(Comments);
                var recipient = report.User.Email;
                string Url = "https://" + Request.Url.Host + "/Reports";
                var nl = "<br />";
                string Body = String.Format("An administrator has dismissed your request to Unsubmit a report." + nl
                    + nl
                    + "Comments: <pre>{6}</pre>" + nl
                    + nl
                    + "Admin: {0} {1} ({2})" + nl
                    + "Report Site: {3}" + nl
                    + "Date Submitted: {4}" + nl
                    + nl
                    + "\r\n<a href=\"{5}\">View Reports</a>"
                    , CurrentUser.FirstName, CurrentUser.LastName, CurrentUser.UserName, report.Site.Name, report.SubmissionDate.HasValue ? report.SubmissionDate.Value.ToShortDateString() : "", Url, encodedComments);

                _emailService.Send(recipient, "Water Treatment: Request to Unsubmit report was dismissed", Body);

                ViewBag.BannerMessage = String.Format("Successfully dismissed Unsubmit request for report ({0} - {1})", report.Site.Name, report.MeasurementDate.Value.ToShortDateString());
                ViewBag.BannerClass = "alert-success";
            }
            catch (Exception e)
            {
                ViewBag.BannerMessage = e.Message;
                ViewBag.BannerClass = "alert-danger";
            }
            return PartialView("_Banner");
        }


        private void ValidateTokens()
        {
            var cookieToken = Request.Cookies["__RequestVerificationToken"].Value;
            var formToken = Request.Headers["__RequestVerificationToken"];
            AntiForgery.Validate(cookieToken, formToken);
        }

        [HttpGet]
        public ActionResult OutOfBounds(string site)
        {

            ViewBag.SystemTypes = context.SystemTypes.Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => new { Name = x.Name, Id = x.Id });
            ViewBag.Sites = _userService.GetActiveSites().OrderBy(x => x.Name).Select(x => new { Name = x.Name, Id = x.Id });

            var initialState = new
            {
                Filters = new
                {
                    Site = string.IsNullOrWhiteSpace(site) ? null : new List<int> { Convert.ToInt32(site) }
                }
            };

            ViewBag.InitialState = JsonConvert.SerializeObject(initialState);

            return View();
        }

        [HttpPost]
        public ActionResult OutOfBoundsSearch(SearchModel Criteria)
        {

            IQueryable<Measurement> filterMeasurements = context.Measurements.Where(x => x.BakedOOB && x.SystemMeasurement.Report.SubmissionDate.HasValue).AsQueryable();

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["SystemTypes"]) && Criteria.Filters["SystemTypes"] != "Any")
            {
                var systemTypes = Criteria.Filters["SystemTypes"].Split(',').Select(x => Convert.ToInt32(x));
                filterMeasurements = filterMeasurements.Where(x => systemTypes.Any(y => x.Parameter.SystemType.Id == y));
            }

            //Site and buildings are against the measurements, not the system types.
            //Must only look at measurements that are OOB
            if (!String.IsNullOrWhiteSpace(Criteria.Filters["Site"]) && Criteria.Filters["Site"] != "Any")
            {
                var site = Convert.ToInt32(Criteria.Filters["Site"]);
                filterMeasurements = filterMeasurements.Where(x => x.SystemMeasurement.BuildingSystem.Building.Site.Id == site);
            }

            if (!String.IsNullOrWhiteSpace(Criteria.Filters["Buildings"]) && Criteria.Filters["Buildings"] != "Any")
            {
                var buildings = Criteria.Filters["Buildings"].Split(',').Select(x => Convert.ToInt32(x));
                filterMeasurements = filterMeasurements.Where(x => buildings.Any(z => z == x.SystemMeasurement.BuildingSystem.Building.Id));
            }

            //Filter only to sites that the user can see
            var allowedSites = _userService.GetActiveSites().Select(x => x.Id).ToList();
            filterMeasurements = filterMeasurements.Where(x => allowedSites.Any(y => y == x.SystemMeasurement.Report.Site.Id));

            var total = filterMeasurements.GroupBy(x => x.Parameter).Count();

            var parameterGroups = filterMeasurements.GroupBy(x => x.Parameter);

            Criteria.SortBy = Criteria.SortBy ?? String.Empty;

            //The nearly duplicated code is a limitation to base Linq to SQL and I don't want to write an extension right now
            switch (Criteria.SortBy)
            {
                case "Name":
                default:
                    if (Criteria.ShouldForwardSearch)
                        parameterGroups = parameterGroups.OrderBy((x) => x.Key.Name);
                    else
                        parameterGroups = parameterGroups.OrderByDescending((x) => x.Key.Name);
                    break;
                case "SystemType":
                    if (Criteria.ShouldForwardSearch)
                        parameterGroups = parameterGroups.OrderBy((x) => x.Key.SystemType.Name);
                    else
                        parameterGroups = parameterGroups.OrderByDescending((x) => x.Key.SystemType.Name);
                    break;
            }

            var groupSets = parameterGroups.Skip(Criteria.Offset).Take(Criteria.MaxResults).ToList();

            var data = new SearchResult<OOBModel>
            {
                Results = groupSets.Select(x => new OOBModel
                {
                    Name = x.Key.Name,
                    SystemName = x.Key.SystemType.Name,
                    Reports = x.Select(y => new OOBReportModel { Id = y.SystemMeasurement.Report.Id, MeasurementDate = y.SystemMeasurement.Report.MeasurementDate.Value })
                }).ToList(),
                Total = total
            };

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ViewResult SiteReport()
        {
            if (_userService.IsAdmin() || _userService.IsSiteAdmin() || _userService.IsExecutiveReportViewer() || _userService.IsReportViewer())
            {
                var sites = _userService.GetActiveSites().OrderBy(x => x.Name).ToList();
                ViewBag.Sites = JsonConvert.SerializeObject(sites);
                var buildings = sites.ToDictionary(s => s.Id, s => s.Buildings.Select(b => new { Id = b.Id, Name = b.Name }));
                ViewBag.BuildingsPerSite = JsonConvert.SerializeObject(buildings);
            }
            else
            {
                ViewBag.Sites = JsonConvert.SerializeObject(new List<Site>());
            }
            return View();
        }

        private const double lateRatio = 1.5;

        [HttpGet]
        public JsonResult GetSiteReport(int SiteId, int? BuildingId, DateTime? From, DateTime? To)
        {
            if (!(_userService.HasFullSiteAccess() || (_userService.IsSiteAdmin() || _userService.IsReportViewer()) && _userService.GetSites().Any(s => s.Id == SiteId)))
            {
                throw new Exception("Permission Denied");
            }

            var site = context.Sites.Where(s => s.Id == SiteId).First();
            var systems = site.Buildings.Where(b => BuildingId == null || b.Id == BuildingId).SelectMany(b => b.Systems).Where(c=>c.IsActive);
            var parameterData = getLatestMeasurementsForSiteOrBuilding(site, BuildingId, null, From, To);

            var ret = new List<SystemTypeRollup>();
            foreach (var system in systems)
            {
                var type = system.SystemType;
                var grouping = new SystemTypeRollup() { SystemType = string.Format("{0} ({1})", type.Name, system.Location), OutOfBoundsCount = 0, LateCount = 0, NeverMeasuredCount = 0, Measurements = new List<MeasurementDatum>() };
                foreach (var parameter in type.Parameters)
                {
                    var bounds = parameter.ParameterBounds;
                    var min = bounds.FirstOrDefault(b => b.MinValue != null);
                    var max = bounds.FirstOrDefault(b => b.MaxValue != null);
                    var latestDatum = parameterData[system.Id][parameter.Id];

                    var frequency = getFrquencyInDays(parameter.Frequency);

                    var measurement = new MeasurementDatum()
                    {
                        ParameterName = parameter.Name,
                        MinBound = (min == null) ? null : min.MinValue, // min.MinValue,
                        MaxBound = (max == null) ? null : max.MaxValue, //  max.MaxValue,
                        Frequency = frequency,
                        Units = parameter.Unit,
                        Value = latestDatum.LatestValue,
                        IsOutOfBounds = latestDatum.IsOutOfBounds,
                        DaysElapsed = latestDatum.DaysElapsed,
                        ElapsedRatio = latestDatum.DaysElapsed.HasValue ? latestDatum.DaysElapsed.Value / frequency : 0,
                        IsLate = latestDatum.IsLate,
                        ReportDate = latestDatum.LatestDate,
                        HasComments = latestDatum.HasComments,
                        ParameterType = latestDatum.ParameterType
                    };

                    #region Rollup Calculations
                    // Now figure out whether to write to the rollup row
                    if (latestDatum.IsOutOfBounds)
                    {
                        grouping.OutOfBoundsCount++;
                    }

                    if (latestDatum.IsLate)
                    {
                        grouping.LateCount++;
                    }

                    if (measurement.ReportDate.HasValue)
                    {
                        if (!grouping.OldestReportDate.HasValue || measurement.ReportDate.Value < grouping.OldestReportDate.Value)
                        {
                            grouping.OldestReportDate = measurement.ReportDate;
                        }

                        if (!grouping.MostRecentReportDate.HasValue || measurement.ReportDate.Value > grouping.MostRecentReportDate.Value)
                        {
                            grouping.MostRecentReportDate = measurement.ReportDate;
                        }
                    }
                    else
                    {
                        grouping.NeverMeasuredCount++;
                    }
                    #endregion

                    grouping.Measurements.Add(measurement);
                }
                ret.Add(grouping);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }



        private int getFrquencyInDays(string frequencyValue)
        {
            int returnValue = 0;
            switch (frequencyValue.ToLower())
            {
                case "quarterly":
                    returnValue = 90;
                    break;
                case "monthly":
                    returnValue = 30;
                    break;
                case "biweekly":
                    returnValue = 14;
                    break;
                case "weekly":
                    returnValue = 7;
                    break;
                case "twice weekly":
                    returnValue = 4;
                    break;
                case "daily":
                    returnValue = 1;
                    break;
                default:
                    int i;
                    if (int.TryParse(frequencyValue, out i)) returnValue = i; ;
                    break;
            }
            return returnValue;
        }



        /// <summary>
        /// Gets the latest measurements per parameter for a Site in the form of (SystemId => (ParameterId => LatestMeasurementDatum))
        /// </summary>
        /// <param name="Site"></param>
        /// <param name="SystemTypeId">Optional filter by System Type</param>
        /// <param name="From">Optional filter by From date</param>
        /// <param name="To">Optional filter by to date</param>
        /// <returns>Dictionary of Dictionaries SystemComparisonModel</returns>
        private void getLatestMeasurementsForSite(Site Site, int SystemTypeId, DateTime? From, DateTime? To,
             SystemComparisonModel retsystemComparision)
        {
            var systemType = context.SystemTypes.First(st => st.IsActive && st.Id == SystemTypeId);

            SqlParameter @siteIdParam = new SqlParameter()
            {
                ParameterName = "@SiteId",
                DbType = DbType.Int32,
                Value = Site.Id
            };

            SqlParameter @systemTypeIdParam = new SqlParameter()
            {
                ParameterName = "@systemTypeId",
                DbType = DbType.Int32,
                Value = SystemTypeId
            };
            SqlParameter @fromDateParam;
            if (From != null)
            {
                @fromDateParam = new SqlParameter()
                {
                    ParameterName = "@FromDate",
                    DbType = DbType.DateTime,
                    Value = From
                };
            }
            else
            {
                @fromDateParam = new SqlParameter()
                {
                    ParameterName = "@FromDate",
                    Value = DBNull.Value
                };
            }

            SqlParameter @toDateParam;
            if (To != null)
            {
                @toDateParam = new SqlParameter()
                {
                    ParameterName = "@ToDate",
                    DbType = DbType.DateTime,
                    Value = To
                };
            }
            else
            {
                @toDateParam = new SqlParameter()
                {
                    ParameterName = "@ToDate",
                    Value = DBNull.Value
                };
            }

            List<SystemComparisonData> resultantRows = null;
            try
            {
                resultantRows = context.Database.SqlQuery<SystemComparisonData>("[dbo].[spGetLatestMeasurementsForSite] @SiteId, @systemTypeId, @FromDate, @ToDate", @siteIdParam, @systemTypeIdParam, @fromDateParam, @toDateParam).ToList();
            }
            catch (Exception ex)
            {
                var Errormsg = ex.Message;
            }

            Dictionary<int, ComparisonDatum> _data = new Dictionary<int, ComparisonDatum>();
            var comparisonRptData = systemType.Parameters.ToDictionary(p => p.Id, p => _data);
            Dictionary<int, SystemMetaDatum> systemLocationData = new Dictionary<int, SystemMetaDatum>();

            if (resultantRows.Count() > 0)
            {
                foreach (SystemComparisonData systemComparision in resultantRows)
                {
                    ComparisonDatum lmd;
                    SystemMetaDatum loc;
                    int parameterId = systemComparision.ParameterId;
                    int buildingSystemId = systemComparision.BuildingSystemId;
                    int reportId = systemComparision.ReportId;
                    // hash is getUniqueKey()


                    var dataForSystem = comparisonRptData[parameterId];    // find correct index
                    if (!dataForSystem.TryGetValue(getUniqueKey(reportId, buildingSystemId), out lmd))
                    {
                        comparisonRptData[parameterId].Add(getUniqueKey(reportId, buildingSystemId), new ComparisonDatum());
                        var lmd2 = comparisonRptData[parameterId][getUniqueKey(reportId, buildingSystemId)];
                        double mv;
                        if (double.TryParse(systemComparision.Value, out mv))
                        {
                            lmd2.Value = systemComparision.Value;
                        }
                        lmd2.ReportDate = systemComparision.SubmissionDate;
                        lmd2.IsOutOfBounds = systemComparision.IsOutofBounds;
                    }

                    if (!systemLocationData.TryGetValue(getUniqueKey(reportId, buildingSystemId), out loc))
                    {
                        systemLocationData.Add(getUniqueKey(reportId, buildingSystemId), new SystemMetaDatum());
                        var locForSystem = systemLocationData[getUniqueKey(reportId, buildingSystemId)];
                        locForSystem.Id = systemComparision.BuildingSystemId;
                        locForSystem.BuildingName = systemComparision.BuildingName;
                        locForSystem.SiteName = systemComparision.SiteName;
                        locForSystem.Location = systemComparision.Location;
                        locForSystem.ReportDate = systemComparision.SubmissionDate.ToShortDateString();
                    }
                }

                //retsystemComparision.SystemMetaData = systemLocationData;
                if (retsystemComparision.SystemMetaData == null)
                {
                    retsystemComparision.SystemMetaData = new Dictionary<int, SystemMetaDatum>();
                }
                foreach (var y in systemLocationData)
                {
                    retsystemComparision.SystemMetaData.Add(y.Key, y.Value);
                }

                //retsystemComparision.ComparisonData = comparisonRptData;
                try
                {
                    if (retsystemComparision.ComparisonData == null)
                    {
                        retsystemComparision.ComparisonData = new Dictionary<int, Dictionary<int, ComparisonDatum>>();
                        foreach (var x in comparisonRptData)
                        {
                            retsystemComparision.ComparisonData.Add(x.Key, x.Value);
                        }
                    }
                    else
                    {
                        ComparisonDatum lmd;
                        foreach (var x in comparisonRptData)
                        {
                            foreach (var y in comparisonRptData[x.Key])
                            {
                                if (retsystemComparision.ComparisonData[x.Key].TryGetValue(y.Key, out lmd))
                                {
                                    lmd.Value = y.Value.Value;
                                    lmd.IsOutOfBounds = y.Value.IsOutOfBounds;
                                    lmd.ReportDate = y.Value.ReportDate;
                                }
                                else
                                {
                                    retsystemComparision.ComparisonData[x.Key].Add(y.Key, y.Value);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errMsg = ex.Message;
                }
            }
            return;   //systemComparisionData
        }





        /// <summary>
        /// Gets the latest measurements per parameter for a Site in the form of (SystemId => (ParameterId => LatestMeasurementDatum))
        /// </summary>
        /// <param name="Site"></param>
        /// <param name="SystemTypeId">Optional filter by System Type</param>
        /// <param name="From">Optional filter by From date</param>
        /// <param name="To">Optional filter by to date</param>
        /// <returns>Dictionary of Dictionaries SystemComparisonModel</returns>
        private void getCollectionDashboardForSite(Site Site, DateTime? From, DateTime? To,
             CollectionsModel retSiteCollectionsCounts)
        {
            int siteId = Site.Id;
            SqlParameter @siteIdParam = new SqlParameter()
            {
                ParameterName = "@SiteId",
                DbType = DbType.Int32,
                Value = siteId
            };

            SqlParameter @fromDateParam;
            if (From != null)
            {
                @fromDateParam = new SqlParameter()
                {
                    ParameterName = "@FromDate",
                    DbType = DbType.DateTime,
                    Value = From
                };
            }
            else
            {
                @fromDateParam = new SqlParameter()
                {
                    ParameterName = "@FromDate",
                    Value = DBNull.Value
                };
            }

            SqlParameter @toDateParam;
            if (To != null)
            {
                @toDateParam = new SqlParameter()
                {
                    ParameterName = "@ToDate",
                    DbType = DbType.DateTime,
                    Value = To
                };
            }
            else
            {
                @toDateParam = new SqlParameter()
                {
                    ParameterName = "@ToDate",
                    Value = DBNull.Value
                };
            }

            List<CollectionDashboardData> resultantRows = null;
            try
            {
                resultantRows = context.Database.SqlQuery<CollectionDashboardData>("[dbo].[spGetCollectionDashboardForSite] @SiteId, @FromDate, @ToDate", @siteIdParam, @fromDateParam, @toDateParam).ToList();
            }
            catch (Exception ex)
            {
                var Errormsg = ex.Message;
            }

            //Dictionary<int, SiteCounts> 

            if (resultantRows.Count() > 0)
            {
                SiteCounts stc;
                try
                {
                    foreach (CollectionDashboardData collectionDashboard in resultantRows)
                    {
                        if (retSiteCollectionsCounts.SiteCollectionData.TryGetValue(siteId, out stc))
                        {
                            stc.Name = Site.Name;
                            stc.InBounds += collectionDashboard.InRange;
                            stc.OutOfBounds += collectionDashboard.InRangeTenPercent;
                            stc.NotCollected += collectionDashboard.OutOfRangeTenPercent;
                            stc.Late += collectionDashboard.NotMeasured;                                                         //collectionDashboard.InRangeTenPercent;

                        }
                        else
                        {
                            stc = new SiteCounts();
                            stc.Name = Site.Name;
                            stc.InBounds = collectionDashboard.InRange;
                            stc.OutOfBounds = collectionDashboard.InRangeTenPercent;
                            stc.NotCollected = collectionDashboard.OutOfRangeTenPercent;
                            stc.Late = collectionDashboard.NotMeasured;

                            retSiteCollectionsCounts.SiteCollectionData.Add(siteId, stc);
                        }
                    }
                    ;
                }

                catch (Exception ex)
                {
                    var errMsg = ex.Message;
                }
            }
            return;   //CollectionsModel
        }


        /// <summary>
        /// Gets the latest measurements per parameter for a Site in the form of (SystemId => (ParameterId => LatestMeasurementDatum))
        /// </summary>
        /// <param name="Site"></param>
        /// <param name="SystemTypeId">Optional filter by System Type</param>
        /// <param name="From">Optional filter by From date</param>
        /// <param name="To">Optional filter by to date</param>
        /// <returns>Dictionary of Dictionaries MeasurementComparisonModel</returns>
        private void getMeasurementComparisionForSite(int ParameterId, Site Site, DateTime? From, DateTime? To,
             MeasurementComparisonModel retMeasurementComparision)
        {

            var reportsForParameter = context.Measurements.Where(m => m.Parameter.Id == ParameterId && m.SystemMeasurement.Report.SubmissionDate!=null).Select(s => s.SystemMeasurement.Report.Id).ToList();
            SqlParameter @siteIdParam = new SqlParameter()
            {
                ParameterName = "@SiteId",
                DbType = DbType.Int32,
                Value = Site.Id
            };

            SqlParameter @parameterIdParam = new SqlParameter()
            {
                ParameterName = "@ParameterId",
                DbType = DbType.Int32,
                Value = ParameterId
            };

            SqlParameter @fromDateParam;
            if (From != null)
            {
                @fromDateParam = new SqlParameter()
                {
                    ParameterName = "@FromDate",
                    DbType = DbType.DateTime,
                    Value = From
                };
            }
            else
            {
                @fromDateParam = new SqlParameter()
                {
                    ParameterName = "@FromDate",
                    Value = DBNull.Value
                };
            }

            SqlParameter @toDateParam;
            if (To != null)
            {
                @toDateParam = new SqlParameter()
                {
                    ParameterName = "@ToDate",
                    DbType = DbType.DateTime,
                    Value = To
                };
            }
            else
            {
                @toDateParam = new SqlParameter()
                {
                    ParameterName = "@ToDate",
                    Value = DBNull.Value
                };
            }

            List<MeasureComparisonData> resultantRows = null;
            try
            {
                resultantRows = context.Database.SqlQuery<MeasureComparisonData>("exec [dbo].[spGetMeasurementComparisonData] @ParameterId, @SiteId, @FromDate, @ToDate", @parameterIdParam, @siteIdParam, @fromDateParam, @toDateParam).ToList();
            }
            catch (Exception ex)
            {
                var Errormsg = ex.Message;
            }

            Dictionary<int, decimal?> reportData = new Dictionary<int, decimal?>();
            Dictionary<int, string> reportHeaders = new Dictionary<int, string>();
            reportHeaders.Add(0, "Site");
            reportHeaders.Add(1, "Min");
            reportHeaders.Add(2, "Max");
            reportData.Add(1, null);
            reportData.Add(2, null);

            if (resultantRows.Count() > 0)
            {
                try
                {
                    foreach(int rptId in reportsForParameter)
                    {
                        decimal? rpt;
                        if (!reportData.TryGetValue(rptId, out rpt))
                        {
                            //initialize
                            reportData.Add(rptId, Convert.ToDecimal(0));
                        }

                    }

                    foreach (int rptId in reportsForParameter)
                    {
                        string hdr;

                        if (!reportHeaders.TryGetValue(rptId, out hdr))
                        {
                            var reportDate = context.Reports.Where(m => m.Id == rptId && m.SubmissionDate != null).Select(s => s.SubmissionDate).SingleOrDefault() ;
                            string reportDatestr = string.Format("{0:M/d/yyyy}", reportDate);
                            string heading = "Rpt " + rptId.ToString() + " " + reportDatestr; 
                            reportHeaders.Add(rptId, heading);
                        }
                    }

                }
                catch (Exception ex)
                {
                    var errMsg = ex.Message;
                }

                Dictionary<int, MeasurementComparisonDatum> MeasureCompData = new Dictionary<int, MeasurementComparisonDatum>();

                foreach (MeasureComparisonData measureComparision in resultantRows)
                {
                    MeasurementComparisonDatum lmd;
                    int siteId = measureComparision.Site_Id;
                    int buildingId = measureComparision.Building_Id;
                    int buildingSystemId = measureComparision.BuildingSystem_Id;
                    int reportId = measureComparision.Report_Id;
                    // hash is getUniqueKey()

                    if (!MeasureCompData.TryGetValue(getUniqueKey(siteId, buildingId, buildingSystemId), out lmd))
                    {
                        MeasureCompData.Add(getUniqueKey(siteId, buildingId, buildingSystemId), new MeasurementComparisonDatum());
                        Dictionary<int, decimal?> reportDataCp = new Dictionary<int, decimal?>(reportData);  // copy
                        //initialize
                        var lmd2 = MeasureCompData[getUniqueKey(siteId, buildingId, buildingSystemId)];
                        lmd2.Values = reportDataCp;  // attach a copy
                        lmd2.SiteName = measureComparision.SiteName;
                        lmd2.BuildingName = measureComparision.BuildingName;
                        lmd2.Location = measureComparision.Location;
                        lmd2.Values[1] = measureComparision.MinValue;
                        lmd2.Values[2] = measureComparision.MaxValue;
                        lmd2.Min = measureComparision.MinValue;
                        lmd2.Max = measureComparision.MaxValue;
                        decimal mv;
                        if (decimal.TryParse(measureComparision.Value, out mv))
                        {
                            lmd2.Values[reportId] = mv;
                        }
                        else
                        {
                            //all report id s should be coverd above
                        }
                    }
                    else
                    {
                        var lmd2 = MeasureCompData[getUniqueKey(siteId, buildingId, buildingSystemId)];
                        decimal mv;
                        if (decimal.TryParse(measureComparision.Value, out mv))
                        {
                            lmd2.Values[reportId] = mv;
                        }
                        else
                        {
                            //all report id s should be coverd above
                        }

                    }

                }

                try
                {
                    if (retMeasurementComparision.MeasurementComparisonData == null)
                    {
                        retMeasurementComparision.MeasurementComparisonData = MeasureCompData;
                    }
                    else
                    {
                        foreach (var element in MeasureCompData)
                        {
                            retMeasurementComparision.MeasurementComparisonData.Add(element.Key, element.Value);
                        }
                    }

                    if (retMeasurementComparision.Reports == null)
                    {
                        retMeasurementComparision.Reports = reportHeaders;
                    }

                }
                catch (Exception ex)
                {
                    var errMsg = ex.Message;
                }
            }
            return;   //measurementComparisionData
        }




        /// <summary>
        /// Gets the latest measurements per parameter for a Site in the form of (SystemId => (ParameterId => LatestMeasurementDatum))
        /// </summary>
        /// <param name="Site"></param>
        /// <param name="BuildingId">Optional filter by Building</param>
        /// <param name="SystemTypeId">Optional filter by System Type</param>
        /// <returns>Dictionary of Dictionaries (SystemId => (ParameterId => LatestMeasurementDatum))</returns>
        private Dictionary<int, Dictionary<int, LatestMeasurementDatum>> getLatestMeasurementsForSiteOrBuilding(Site Site, int? BuildingId, int? SystemTypeId, DateTime? From, DateTime? To)
        {
            var buildings = BuildingId == null ? Site.Buildings : Site.Buildings.Where(b => b.Id == BuildingId);
            var systems = SystemTypeId == null ? buildings.SelectMany(b => b.Systems) : buildings.SelectMany(b => b.Systems).Where(sys => sys.SystemType.Id == SystemTypeId);

            var baseQuery = context.Reports.Where(r => r.Site.Id == Site.Id && r.SubmissionDate.HasValue);

            // Ignore submission time in filtering
            if (From.HasValue)
            {
                baseQuery = baseQuery.Where(r => DbFunctions.TruncateTime(r.SubmissionDate) >= DbFunctions.TruncateTime(From));
            }

            if (To.HasValue)
            {
                baseQuery = baseQuery.Where(r => DbFunctions.TruncateTime(r.SubmissionDate) <= DbFunctions.TruncateTime(To));
            }

            var siteReports = baseQuery.OrderByDescending(r => r.MeasurementDate).ToList();

            var parameters = systems.SelectMany(s => s.SystemType.Parameters);
            var parameterData = systems.ToDictionary(s => s.Id, s => s.SystemType.Parameters.ToDictionary(p => p.Id, p => new LatestMeasurementDatum() { ParameterName = p.Name }));
            var parameterMetaData = new Dictionary<int, ParameterMetaDatum>();
            var toProcess = new List<int>(parameters.Select(p => p.Id));
            var i = 0;
            while (i < siteReports.Count() && toProcess.Count > 0)
            {
                var report = siteReports[i];
                foreach (var systemMeasurement in report.SystemMeasurements.Where(br => br.BuildingSystem.Building.Id == BuildingId && br.BuildingSystem.IsActive).Where(sm => SystemTypeId == null || sm.BuildingSystem.SystemType.Id == SystemTypeId))
                {
                    var system = systemMeasurement.BuildingSystem;
                    var dataForSystem = parameterData[system.Id];
                    foreach (var measurement in systemMeasurement.Measurements)
                    {
                        var parameter = measurement.Parameter;
                        ParameterMetaDatum pmd;
                        if (!parameterMetaData.TryGetValue(parameter.Id, out pmd))
                        {
                            var bounds = parameter.ParameterBounds;
                            var min = (bounds.FirstOrDefault() == null) ? null : bounds.FirstOrDefault().MinValue;
                            var max = (bounds.FirstOrDefault() == null) ? null : bounds.FirstOrDefault().MaxValue;
                            parameterMetaData[parameter.Id] = new ParameterMetaDatum() { Name = parameter.Name, Type = parameter.Type.Name, MinBound = min, MaxBound = max };
                        }

                        LatestMeasurementDatum lmd;
                        if (dataForSystem.TryGetValue(measurement.Parameter.Id, out lmd))
                        {     // Maybe a report in the past had additional parameters/systems that are no longer associated with the Site/Building, so check that this measurement is in the parameterData
                            double mv;
                            var canParseMeasurementVal = double.TryParse(measurement.Value, out mv);
                            var paramMetaData = parameterMetaData[parameter.Id];
                            lmd.ParameterType = paramMetaData.Type;
                            if (measurement.IsApplicable && canParseMeasurementVal)
                            {
                                decimal lv;
                                var canParseLatestVal = decimal.TryParse(lmd.LatestValue, out lv);
                                if (string.IsNullOrEmpty(lmd.LatestValue) || !canParseLatestVal)
                                {
                                    lmd.LatestValue = measurement.Value;
                                    lmd.LatestDate = report.MeasurementDate.Value;
                                    lmd.HasComments = !string.IsNullOrEmpty(measurement.Comment);
                                    lmd.DaysElapsed = (DateTime.UtcNow - report.MeasurementDate.Value).Days;
                                    lmd.IsOutOfBounds = lv < paramMetaData.MinBound || lv > paramMetaData.MaxBound;
                                    lmd.IsLate = (double)lmd.DaysElapsed / Convert.ToDouble(getFrquencyInDays(parameter.Frequency)) > lateRatio;
                                }
                                else
                                {
                                    toProcess.Remove(measurement.Parameter.Id);
                                }
                            }
                        }
                    }
                }
                i++;
            }
            return parameterData;
        }




        private class LatestMeasurementDatum
        {
            public string ParameterName { get; set; }
            public string LatestValue { get; set; }
            public DateTime? LatestDate { get; set; }
            public int? DaysElapsed { get; set; }
            public bool? HasComments { get; set; }
            public bool IsLate { get; set; }
            public bool IsOutOfBounds { get; set; }
            public string ParameterType { get; set; }
        }


        private class SystemComparisonData
        {
            public int ParameterId { get; set; }
            public string SiteName { get; set; }
            public string BuildingName { get; set; }
            public int BuildingSystemId { get; set; }
            public int SystemTypeId { get; set; }
            public DateTime MeasurementDate { get; set; }
            public DateTime SubmissionDate { get; set; }
            public Boolean IsOutofBounds { get; set; }
            public string Value { get; set; }
            public string ParameterName { get; set; }
            public int ReportId { get; set; }
            public string Location { get; set; }
        }

        private class MeasureComparisonData
        {
            public string SiteName { get; set; }
            public int Parameter_Id { get; set; }
            public int SystemType_Id { get; set; }
            public int BuildingSystem_Id { get; set; }
            public int Site_Id { get; set; }
            public int Building_Id { get; set; }
            public string BuildingName { get; set; }
            public string Location { get; set; }
            public int SystemMeasurement_Id { get; set; }
            public int Report_Id { get; set; }
            public DateTime MeasurementDate { get; set; }
            public DateTime SubmissionDate { get; set; }
            public string Value { get; set; }
            public decimal MinValue { get; set; }
            public decimal MaxValue { get; set; }
        }

        private class CollectionDashboardData
        {
            public int InRange { get; set; }
            public int InRangeTenPercent { get; set; }
            public int OutOfRangeTenPercent { get; set; }
            public int NotMeasured { get; set; }
            public int SiteId { get; set; }
            public DateTime SubmissionDate { get; set; }
        }

        private class LeadershipDashboardData
        {
            public int siteId  { get; set; }
            public string siteName { get; set; }
            public int AllInRange { get; set; }
            public int AllMeasured { get; set; }
            public int? SomeMeasured { get; set; }
        }

        [HttpGet]
        public ActionResult SystemComparison()
        {
            if (_userService.IsAdmin() || _userService.IsSiteAdmin() || _userService.IsExecutiveReportViewer() || _userService.IsReportViewer())
            {
                var systemTypes = context.SystemTypes.Where(x => x.IsActive).ToList();
                ViewBag.SystemTypes = JsonConvert.SerializeObject(systemTypes.OrderBy(x => x.Name).Select(x => new { Name = x.Name, Id = x.Id }));

                var siteIds = _userService.GetActiveSites().OrderBy(x => x.Name).Select(s => s.Id).ToList();
                Dictionary<int, List<Site>> sitesBySystemType = new Dictionary<int, List<Site>>();

                foreach (SystemType systype in systemTypes)
                {
                    var SiteRows = systype.BuildingSystems.Select(bs => bs.Building.Site).Where(s => s != null && siteIds.Contains(s.Id)).Distinct();
                    List<Site> sitesBySystemType1 = SiteRows.ToList();
                    sitesBySystemType.Add(systype.Id, sitesBySystemType1);
                }
                ViewBag.SitesBySystemType = JsonConvert.SerializeObject(sitesBySystemType);
            }
            else
            {
                return new RedirectResult("/");
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetSystemComparison(int SystemTypeId, IEnumerable<int> SiteIds, DateTime? From, DateTime? To)
        {
            if (!(_userService.HasFullSiteAccess() || _userService.IsSiteAdmin() || _userService.IsExecutiveReportViewer() || _userService.IsReportViewer()))
            {
                throw new Exception("Permission Denied");
            }
            var filteredSiteIds = SiteIds == null ? new List<int>() : SiteIds;  // Side note: even if we ensure we pass an empty array from the js in the "no filter" case, MVC translates that to null instead of an empty IEnumerable (or array or whatever)
            var noSiteFilter = filteredSiteIds.Count() == 0;
            var myActiveSites = _userService.GetActiveSites().Where(s => noSiteFilter || filteredSiteIds.Contains(s.Id)).ToList();
            var sites = myActiveSites.Where(s => s.Buildings.SelectMany(b => b.Systems).Select(sys => sys.SystemType).Any(st => st.Id == SystemTypeId));
            var systemType = context.SystemTypes.First(st => st.IsActive && st.Id == SystemTypeId);

            var ret = new SystemComparisonModel() { SystemTypeName = systemType.Name, ParameterMetaData = new Dictionary<int, ParameterMetaDatum>(), SystemMetaData = new Dictionary<int, SystemMetaDatum>() };

            foreach (var parameter in systemType.Parameters)
            {
                var bounds = parameter.ParameterBounds;
                var min = bounds.FirstOrDefault() == null ? null : bounds.FirstOrDefault().MinValue;
                var max = bounds.FirstOrDefault() == null ? null : bounds.FirstOrDefault().MaxValue;
                ret.ParameterMetaData[parameter.Id] = new ParameterMetaDatum() { Name = parameter.Name, Type = parameter.Type.Name, MinBound = min, MaxBound = max };
            }
            //int i = 1;
            foreach (Site s in sites)
            {
                getLatestMeasurementsForSite(s, SystemTypeId, From, To, ret);
            }

            var serialized = JsonConvert.SerializeObject(ret);
            return Json(serialized, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CollectionDashboard()
        {
            if (!(_userService.HasFullSiteAccess() || _userService.IsSiteAdmin() || _userService.IsReportViewer()))
            {
                return new RedirectResult("/");
            }
            var sites = _userService.GetActiveSites().OrderBy(s=>s.Name).ToList();
            ViewBag.Sites = JsonConvert.SerializeObject(sites);
            return View();
        }

        [HttpPost]
        public ActionResult GetCollectionData(IEnumerable<int> SiteIds, DateTime? From, DateTime? To)
        {
            List<Site> sites;
            if (_userService.IsAdmin() || _userService.IsSiteAdmin() || _userService.IsExecutiveReportViewer() || _userService.IsReportViewer())
            {
                var filteredSiteIds = SiteIds == null ? new List<int>() : SiteIds;  // Side note: even if we ensure we pass an empty array from the js in the "no filter" case, MVC translates that to null instead of an empty IEnumerable (or array or whatever)
                var noSiteFilter = filteredSiteIds.Count() == 0;
                sites = _userService.GetActiveSites().Where(s => filteredSiteIds.Contains(s.Id)).ToList();
            }
            else
            {
                throw new Exception("Permission Denied");
            }

            var ret = new CollectionsModel();
            foreach (var site in sites)
            {
                getCollectionDashboardForSite(site, From, To, ret);
            }

            var serialized = JsonConvert.SerializeObject(ret);
            return Json(serialized, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MeasurementComparison()
        {
            if (_userService.IsAdmin() || _userService.IsSiteAdmin() || _userService.IsExecutiveReportViewer() || _userService.IsReportViewer())
            {
                var parameters = new List<Parameter>();
                var mySites = _userService.GetActiveSites().OrderBy(x => x.Name).ToList();

                var parametersToSites = new Dictionary<int, List<NamedEntity>>();
                foreach (var site in mySites)
                {
                    foreach (var param in site.Buildings.SelectMany(b => b.Systems.Where(b1=>b1.IsActive && b1.SystemType.IsActive)).Select(s => s.SystemType).SelectMany(st => st.Parameters))
                    {
                        parameters.Add(param);
                        var paramId = param.Id;
                        List<NamedEntity> paramSites;
                        if (!parametersToSites.TryGetValue(paramId, out paramSites))
                        {
                            parametersToSites[paramId] = new List<NamedEntity>();
                        }
                        var sites = parametersToSites[paramId];
                        if (!sites.Any(s => s.Id == site.Id))
                        {
                            sites.Add(new NamedEntity() { Id = site.Id, Name = site.Name });
                        }
                    }
                }

                ViewBag.Parameters = JsonConvert.SerializeObject(parameters.Distinct().Select(p => new { Name = p.Name + " - " + p.SystemType.Name, Id = p.Id }).OrderBy(p => p.Name));
                ViewBag.SitesByParameter = JsonConvert.SerializeObject(parametersToSites);
            }
            else
            {
                return new RedirectResult("/");
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetMeasurementComparison(int ParameterId, IEnumerable<int> SiteIds, DateTime? From, DateTime? To)
        {
            if (!(_userService.HasFullSiteAccess() || _userService.IsSiteAdmin() || _userService.IsExecutiveReportViewer() || _userService.IsReportViewer()))
            {
                throw new Exception("Permission Denied");
            }
            var missingMeasurements = new List<MeasurementComparisonDatum>();

            var filteredSiteIds = SiteIds == null ? new List<int>() : SiteIds;
            var noSiteFilter = filteredSiteIds.Count() == 0;
            var myActiveSites = _userService.GetActiveSites().Where(s => noSiteFilter || filteredSiteIds.Contains(s.Id)).ToList();
            var parameter = context.Parameters.First(p => p.Id == ParameterId);
            var parameterType = parameter.Type.Name;

            var ret = new MeasurementComparisonModel() { SystemType = parameter.SystemType.Name, Units = parameter.Unit, Parameter = parameter.Name, ParameterType = parameterType };
            var measurementData = new Dictionary<int, MeasurementComparisonDatum>();       // systemId => measurementValue (null for never measuremented)

            if (myActiveSites != null)
            {
                try
                {
                    var sites = parameter.SystemType.BuildingSystems.Select(bs => bs.Building).Select(b => b.Site).Distinct().Where(s => myActiveSites.Any(mas => mas.Id == s.Id));
                    foreach (Site s in sites)
                    {
                        getMeasurementComparisionForSite(ParameterId, s, From, To, ret);
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }
            return Json(JsonConvert.SerializeObject(ret));
        }

        [HttpGet]
        public FileContentResult GetReportPDF(int id)
        {
            var report = context.Reports.Find(id);
            var model = _reportService.BuildViewReportModel(report);

            var name = DateTime.UtcNow.ToShortDateString() + " Report For " + model.SiteName;
            var html = string.Empty;

            using (var output = new StringWriter())
            {
                this.ViewData.Model = model;
                var view = ViewEngines.Engines.FindView(this.ControllerContext, "ReportPDF2", "_LayoutPDF").View;
                var viewContext = new ViewContext(this.ControllerContext, view, this.ViewData, this.TempData, output);

                view.Render(viewContext, output);
                html = output.ToString();
            }

            return File(_pdfService.Create(html), _pdfService.ContentType, name + _pdfService.Extension);
        }

        [HttpGet]
        public ActionResult ReportPDF(int id)
        {
            var report = context.Reports.Find(id);
            var model = _reportService.BuildViewReportModel(report);

            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteReport(int ReportId)
        {
            ValidateTokens();
            try
            {
                var report = context.Reports.FirstOrDefault(r => r.Id == ReportId);
                if (report == null)
                {
                    throw new Exception("Invalid report ID");
                }

                var hasPermission = CurrentUser.Id == report.User.Id || _userService.IsAdmin() || (_userService.IsSiteAdmin() && CurrentUser.userSiteAccess.Select(sa => sa).Any(s => s.Id == report.Site.Id));

                if (!hasPermission)
                {
                    throw new Exception("Permission denied");
                }

                if (report.SubmissionDate.HasValue)
                {
                    throw new Exception("Cannot delete a submitted report");
                }

                var siteName = report.Site.Name;
                var measuredOn = string.Empty;
                if (report.MeasurementDate.HasValue)
                {
                    measuredOn = " - " + report.MeasurementDate.Value.ToShortDateString();
                }

                _reportService.Delete(report);

                ViewBag.BannerMessage = String.Format("Successfully deleted report ({0}{1})", siteName, measuredOn);
                ViewBag.BannerClass = "alert-success";
            }
            catch (Exception e)
            {
                ViewBag.BannerMessage = e.Message;
                ViewBag.BannerClass = "alert-danger";
            }
            return PartialView("_Banner");
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
                Stream file = _s3Storage.GetFile(attachment.StorageId);
                if (file != null)
                    return File(file, "application/force-download", attachment.Name);
                else
                    return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
            else
            {
                return File(_storage.Get(attachment.StorageId), "application/force-download", attachment.Name);
            }

        }

        [HttpPost]
        public JsonResult Subscribe(int id)
        {
            ValidateTokens();

            var subs = GetSubscribedSiteIds(_userService.CurrentUser).ToList();
            if (!subs.Contains(id))
            {
                context.ReportSubscriptions.Add(new ReportSubscription
                {
                    User = _userService.CurrentUser,
                    Site = context.Sites.Single(s => s.Id == id)
                });

                context.SaveChanges();

                subs.Add(id);
            }

            return Json(subs);
        }

        [HttpPost]
        public JsonResult Unsubscribe(int id)
        {
            ValidateTokens();

            var subs = GetSubscribedSiteIds(_userService.CurrentUser).ToList();
            if (subs.Contains(id))
            {
                var remove = context.ReportSubscriptions.Single(rs => rs.Site.Id == id && rs.User.Id == _userService.CurrentUser.Id);
                context.ReportSubscriptions.Remove(remove);
                context.SaveChanges();

                subs.Remove(id);
            }

            return Json(subs);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Unsubscribe(int id, string token)
        {
            var subscription = context.ReportSubscriptions.FirstOrDefault(rs => rs.Id == id);
            if (subscription == null)
            {
                TempData["Error"] = "Subscription does not exist.";
                return RedirectToAction("Login", "Account");
            }

            if (subscription.UnsubscribeAuthToken != token)
            {
                TempData["Error"] = "Unsubscribe link has expired.";
                return RedirectToAction("Login", "Account");
            }

            var model = new UnsubscribeModel
            {
                Id = subscription.Id,
                Token = subscription.UnsubscribeAuthToken
            };

            ViewBag.Site = subscription.Site.Name;

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UnsubscribeMail")]
        public ActionResult Unsubscribe(UnsubscribeModel model)
        {
            if (ModelState.IsValid)
            {
                var subscription = context.ReportSubscriptions.FirstOrDefault(rs => rs.Id == model.Id);
                if (subscription == null || (subscription.User.Email != model.Email))
                {
                    TempData["Error"] = "Subscription does not exist.";
                    return RedirectToAction("Login", "Account");
                }

                if (subscription.UnsubscribeAuthToken != model.Token)
                {
                    TempData["Error"] = "Unsubscribe link has expired.";
                    return RedirectToAction("Login", "Account");
                }

                context.ReportSubscriptions.Remove(subscription);
                context.SaveChanges();

                TempData["CreateSuccess"] = string.Format("Unsubscribed {0}.", model.Email);
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        private IEnumerable<int> GetSubscribedSiteIds(User against)
        {
            return context.ReportSubscriptions.Where(rs => rs.User.Id == against.Id).Select(rs => rs.Site.Id).ToArray();
        }

        public ActionResult Frequency()
        {
            return View();
        }

        private int getUniqueKey(int id1, int id2, int id3)
        {
            int interId = Pairing(id1, id2);
            return Pairing(interId, id3);
        }

        private int getUniqueKey(int id1, int id2)
        {
            return Pairing(id1, id2);
        }

        private int Pairing(int k1, int k2)
        {
            var uniqueNum = .5 * (k1 + k2) * (k1 + k2 + 1) + k2;
            return Convert.ToInt32(uniqueNum);
        }

        [HttpGet]
        public ActionResult HighCommandDashboard()
        {
            if (_userService.IsAdmin())
            {
                List<LeadershipDashboardData> siteComplence = getComplientSites();
                int inRange = siteComplence.Where(l => l.AllInRange == 1).Count();
                int allMeasured = siteComplence.Where(l => l.AllInRange == 0 & l.AllMeasured == 1).Count();
                int missedMeasurements = siteComplence.Where(l => l.AllInRange == 0 & l.AllMeasured == 0 & l.SomeMeasured !=null).Count();
                int noMeasurements = siteComplence.Where(l => l.AllInRange == 0 & l.AllMeasured == 0 & l.SomeMeasured == null).Count();

                Dictionary<string, int> complienceData = new Dictionary<string, int>();
                complienceData.Add("InRange", inRange);
                complienceData.Add("SomeOutofRange", allMeasured);
                complienceData.Add("Missedmeasurements", missedMeasurements);
                complienceData.Add("Nomeasurements", noMeasurements);
                ViewBag.dashboard = JsonConvert.SerializeObject(complienceData);

                ViewBag.complience = JsonConvert.SerializeObject(siteComplence);
            }
            else
            {
                return new RedirectResult("/");
            }

            return View();
        }
  
        private List<LeadershipDashboardData> getComplientSites()
        {
            var currentUserId = CurrentUser.Id;
            //only accesible sites are considered
            SqlParameter @userIdParam = new SqlParameter()
            {
                ParameterName = "@userId",
                DbType = DbType.Int32,
                Value = currentUserId
            };

            List<LeadershipDashboardData> resultantRows = null;
            try
            {
                resultantRows = context.Database.SqlQuery<LeadershipDashboardData>("exec [dbo].[spGetLeadershipDashboard] @userId", @userIdParam).ToList();
            }
            catch (Exception ex)
            {
                var Errormsg = ex.Message;
            }
            return resultantRows;
        }

        [HttpGet]
        public ActionResult SRMPMDashboard()
        {
            List<Site> userSites = CurrentUser.userSiteAccess.Select(s => s).OrderBy(s => s.Name).ToList();

            if ( _userService.IsSiteAdmin() && userSites.Count>0)
            {
                List<LeadershipDashboardData> siteComplence = getComplientSites();
                int inRange = siteComplence.Where(l => l.AllInRange == 1).Count();
                int allMeasured = siteComplence.Where(l => l.AllInRange == 0 & l.AllMeasured == 1).Count();
                int missedMeasurements = siteComplence.Where(l => l.AllInRange == 0 & l.AllMeasured == 0 & l.SomeMeasured != null).Count();
                int noMeasurements = siteComplence.Where(l => l.AllInRange == 0 & l.AllMeasured == 0 & l.SomeMeasured == null).Count();

                Dictionary<string, int> complienceData = new Dictionary<string, int>();
                complienceData.Add("InRange", inRange);
                complienceData.Add("SomeOutofRange", allMeasured);
                complienceData.Add("Missedmeasurements", missedMeasurements);
                complienceData.Add("Nomeasurements", noMeasurements);
                ViewBag.dashboard = JsonConvert.SerializeObject(complienceData);

                ViewBag.complience = JsonConvert.SerializeObject(siteComplence);
            }
            else
            {
                return new RedirectResult("/");
            }

            return View();
        }

        [HttpGet]
        public ActionResult SiteDashboard(string site)
        {
            var startDate = DateTime.Now.AddDays(-33).ToShortDateString();
            var endDate = DateTime.Now.ToShortDateString();
            var submitDate= DateTime.Now.AddDays(-30).ToShortDateString();

            var initialState = new
            {
                ShowCreatedBy = _userService.IsAdmin() || _userService.IsSiteAdmin() ,    // These roles can see others' reports
                ShowAdminActions = _userService.IsAdmin() || _userService.IsSiteAdmin(),
                Subscriptions = GetSubscribedSiteIds(_userService.CurrentUser),
                Filters = new
                {
                    MeasurementDateStart = startDate,
                    MeasurementDateEnd = endDate,
                    ReportSubmitDate = submitDate,
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

        [HttpPost]
        public JsonResult SiteDashboard(SearchModel criteria)
        {
            IQueryable<Report> filteredReports = context.Reports.Where(r => r.SubmissionDate.HasValue).AsQueryable();

            var currentUserId = CurrentUser.Id;
            var isSysAdmin = _userService.IsAdmin();
            var isSiteAdmin = _userService.IsSiteAdmin();
            var siteAdminSites = new List<int>();
            if (isSiteAdmin)
            {
                siteAdminSites = CurrentUser.userSiteAccess.Select(s => s.Id).ToList();
                filteredReports = filteredReports.Where(r => r.User.Id == currentUserId || siteAdminSites.Any(s => r.Site.Id == s));
            }


            if (!string.IsNullOrWhiteSpace(criteria.Filters["Sites"]))
            {
                var siteIds = criteria.Filters["Sites"].Split(',').Select(v => Convert.ToInt32(v));
                filteredReports = filteredReports.Where(x => siteIds.Any(s => x.Site.Id == s));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Filters["ReportSubmitDate"]))
            {
                var reportSubmitStr = criteria.Filters["ReportSubmitDate"];
                DateTime reportSubmit;
                if (DateTime.TryParse(reportSubmitStr, out reportSubmit))
                {
                    // The call to ToUniversalTime() is mostly to make it clearer during debugging that we're comparing UTC to UTC (i.e. rather than 19:00ET to 00:00UTC, which are equivalent depending on DST)
                    reportSubmit = reportSubmit.ToUniversalTime();
                    filteredReports = filteredReports.Where(r => r.SubmissionDate >= reportSubmit);
                }
            }

            var now = DateTime.UtcNow;

            switch (criteria.SortBy ?? string.Empty)
            {
                case "Site":
                    filteredReports = filteredReports.SortBy(x => x.Site.Name, criteria.ShouldForwardSearch);
                    break;
                case "Type":
                    filteredReports = filteredReports.SortBy(x => x.ReportType, criteria.ShouldForwardSearch);
                    break;
                case "StartedOn":
                    filteredReports = filteredReports.SortBy(x => x.CreationDate, criteria.ShouldForwardSearch);
                    break;
                case "MeasuredOn":
                    filteredReports = filteredReports.SortBy(x => x.MeasurementDate.Value, criteria.ShouldForwardSearch);
                    break;
                case "SubmittedOn":
                    filteredReports = filteredReports.SortBy(x => x.SubmissionDate.HasValue ? x.SubmissionDate.Value : now, criteria.ShouldForwardSearch);
                    break;
                default:
                    filteredReports = filteredReports.OrderByDescending(x => x.UnsubmitRequestedDate).ThenByDescending(x => x.SubmissionDate).ThenByDescending(x => x.MeasurementDate);
                    break;
            }

            var total = filteredReports.Count();
            var reports = filteredReports.Skip(criteria.Offset).Take(criteria.MaxResults).ToList();
            var data = new SearchResult<ReportSummaryViewModel>
            {
                Results = reports.Select(x => new ReportSummaryViewModel
                {
                    Id = x.Id,
                    Site = x.Site.Name,
                    Type = x.ReportType,
                    StartedOn = DateTime.SpecifyKind(x.CreationDate, DateTimeKind.Utc),
                    SubmittedOn = x.SubmissionDate.HasValue ? DateTime.SpecifyKind(x.SubmissionDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                    MeasuredOn = x.MeasurementDate.HasValue ? x.MeasurementDate.Value.ToShortDateString() : string.Empty,
                    CreatedBy = String.Format("{0} {1} ({2})", x.User.FirstName, x.User.LastName, x.User.UserName),
                    Vendor = x.User.Vendor != null ? x.User.Vendor.Name : null,
                    CanRequestUnsubmit = x.User.Id == currentUserId && x.SubmissionDate.HasValue,
                    HasRequestedUnsubmit = _reportService.HasRequestedUnsubmit(x),
                    CanUnsubmit = x.SubmissionDate.HasValue && (isSysAdmin || isSiteAdmin && siteAdminSites.Any(s => s == x.Site.Id))
                }),
                Total = total
            };

            return Json(data);

        }

        public  ActionResult PowerBiReport()
        {

            return View();
        }

    }
}
