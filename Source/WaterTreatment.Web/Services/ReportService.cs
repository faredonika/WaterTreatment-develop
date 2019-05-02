using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Models;
using WaterTreatment.Web.Services.Interface;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WaterTreatment.Web.Services
{

    public interface IReportService
    {
        ReportModel BuildReportModel(Report report);
        ReportModel BuildReportModel2(Report report);
        ViewReportModel BuildViewReportModel(Report report);
        bool HasRequestedUnsubmit(Report report);
        void Delete(Report report);
        void Delete(int reportId);
    }

    public class ReportService : IReportService
    {
        private readonly WTContext Context;
        private readonly IFileStorage _storage;
        private readonly IAWSS3 _AWSS3Storage;

        public ReportService(WTContext context, IFileStorage storage, IAWSS3 s3Storage)
        {
            Context = context;
            string storageType = ConfigurationManager.AppSettings["StorageType"].ToUpper();
            if (storageType == Constants.StorageType_AWSS3)
            {
                _AWSS3Storage = s3Storage;
            }
            else
            {
                _storage = storage;
            }
        }

        public ReportModel BuildReportModel(Report Report)
        {
            var Model = new ViewReportModel();
            Model.Id = Report.Id;
            Model.SiteName = Report.Site.Name;
            Model.MeasurementDate = string.Empty;
            Model.Notes = Report.Notes;

            if (Report.MeasurementDate.HasValue)
                Model.MeasurementDate = Report.MeasurementDate.Value.ToShortDateString();

            foreach (var systemmeasurement in Report.SystemMeasurements)
            {
                var RSM = new ReportSystemMeasurementModel();

                RSM.Id = systemmeasurement.Id;
                RSM.Location = systemmeasurement.BuildingSystem.Location;
                RSM.Description = systemmeasurement.BuildingSystem.Description;
                RSM.SystemName = systemmeasurement.BuildingSystem.SystemType.Name;
                RSM.ReasonSkipped = systemmeasurement.ReasonSkipped;

                foreach (var measurement in systemmeasurement.Measurements)
                {
                    var measurementModel = new MeasurementModel();

                    measurementModel.Id = measurement.Id;
                    measurementModel.Name = measurement.Parameter.Name;
                    measurementModel.Source = measurement.Parameter.Source;
                    measurementModel.HasBounds = false;//Assume it doesn't and update if it does
                    measurementModel.Value = measurement.Value;
                    measurementModel.Unit = measurement.Parameter.Unit;
                    measurementModel.IsApplicable = measurement.IsApplicable;
                    measurementModel.Comment = measurement.Comment;
                    measurementModel.IsAdhoc = false; // this is a regular measurement

                    //Despite being able to enter multiple of these only grab the first
                    //<Todo>: Restrict adding multiple global or local of the same site
                    //Grab the global scoped parameter if it actually exists
                    var PB = measurement.Parameter.ParameterBounds.FirstOrDefault();

                    //If there are no global bounds try and grab the first local bound
                    //if (PB == null) {
                    //    PB = measurement.Parameter.ParameterBounds.FirstOrDefault(x => x.Site == Report.Site);
                    //}

                    if (PB != null)
                    {
                        measurementModel.HasBounds = true;

                        measurementModel.Type = PB.Type;
                        measurementModel.Range = PB.Range;
                        measurementModel.MinValue = PB.MinValue;
                        measurementModel.MinDescription = PB.MinDescription;
                        measurementModel.MaxDescription = PB.MaxDescription;
                        measurementModel.MaxValue = PB.MaxValue;
                        measurementModel.IsEnforced = PB.IsEnforced;
                    }
                    RSM.Measurements.Add(measurementModel);
                }
                // Adhoc measurements go here
                foreach (var AddhocMeasurement in systemmeasurement.AdhocMeasurements)
                {
                    var measurementModel = new MeasurementModel();

                    measurementModel.Id = AddhocMeasurement.Id;
                    measurementModel.Name = AddhocMeasurement.AdhocParameter.Name;
                    measurementModel.Source = AddhocMeasurement.AdhocParameter.Source;
                    measurementModel.HasBounds = false;//Assume it doesn't and update if it does
                    measurementModel.Value = AddhocMeasurement.Value;
                    measurementModel.Unit = AddhocMeasurement.AdhocParameter.Unit;
                    measurementModel.IsApplicable = AddhocMeasurement.IsApplicable;
                    measurementModel.Comment = AddhocMeasurement.Comment;
                    measurementModel.IsAdhoc = true; // this is an adhoc measurement

                    //Despite being able to enter multiple of these only grab the first
                    //<Todo>: Restrict adding multiple global or local of the same site
                    //Grab the global scoped parameter if it actually exists
                    var PB = AddhocMeasurement.AdhocParameter.ParameterBounds.FirstOrDefault();

                    //If there are no global bounds try and grab the first local bound
                    //if (PB == null) {
                    //    PB = measurement.Parameter.ParameterBounds.FirstOrDefault(x => x.Site == Report.Site);
                    //}

                    if (PB != null)
                    {
                        measurementModel.HasBounds = true;

                        measurementModel.Type = PB.Type;
                        measurementModel.Range = PB.Range;
                        measurementModel.MinValue = PB.MinValue;
                        measurementModel.MinDescription = PB.MinDescription;
                        measurementModel.MaxDescription = PB.MaxDescription;
                        measurementModel.MaxValue = PB.MaxValue;
                        measurementModel.IsEnforced = PB.IsEnforced;
                    }
                    RSM.Measurements.Add(measurementModel);
                }


                var buildingModel = new ReportBuildingModel();

                buildingModel.Systems.Add(RSM);
                buildingModel.Id = systemmeasurement.BuildingSystem.Building.Id;
                buildingModel.Name = systemmeasurement.BuildingSystem.Building.Name;
                Model.Buildings.Add(buildingModel);
            }
            return Model;
        }

        public ReportModel BuildReportModel2(Report Report)
        {
            var Model = new ViewReportModel();
            Model.Id = Report.Id;
            Model.SiteName = Report.Site.Name;
            Model.MeasurementDate = string.Empty;
            Model.Notes = Report.Notes;

            if (Report.MeasurementDate.HasValue)
                Model.MeasurementDate = Report.MeasurementDate.Value.ToShortDateString();

            foreach (var building in Report.Site.Buildings)
            {
                var buildingModel = new ReportBuildingModel();

                foreach (var systemmeasurement in Report.SystemMeasurements)
                {
                    if (systemmeasurement.BuildingSystem.Building.Id != building.Id)
                        continue;
                    var RSM = new ReportSystemMeasurementModel();

                    RSM.Id = systemmeasurement.Id;
                    RSM.Location = systemmeasurement.BuildingSystem.Location;
                    RSM.Description = systemmeasurement.BuildingSystem.Description;
                    RSM.SystemName = systemmeasurement.BuildingSystem.SystemType.Name;
                    RSM.ReasonSkipped = systemmeasurement.ReasonSkipped;

                    foreach (var measurement in systemmeasurement.Measurements)
                    {
                        var measurementModel = new MeasurementModel();

                        measurementModel.Id = measurement.Id;
                        measurementModel.Name = measurement.Parameter.Name;
                        measurementModel.Source = measurement.Parameter.Source;
                        measurementModel.HasBounds = false;//Assume it doesn't and update if it does
                        measurementModel.Value = measurement.Value;
                        measurementModel.Unit = measurement.Parameter.Unit;
                        measurementModel.IsApplicable = measurement.IsApplicable;
                        measurementModel.Comment = measurement.Comment;

                        //Despite being able to enter multiple of these only grab the first
                        //<Todo>: Restrict adding multiple global or local of the same site
                        //Grab the global scoped parameter if it actually exists
                        var PB = measurement.Parameter.ParameterBounds.FirstOrDefault();

                        //If there are no global bounds try and grab the first local bound
                        //if (PB == null) {
                        //    PB = measurement.Parameter.ParameterBounds.FirstOrDefault(x => x.Site == Report.Site);
                        //}

                        if (PB != null)
                        {
                            measurementModel.HasBounds = true;

                            measurementModel.Type = PB.Type;
                            measurementModel.Range = PB.Range;
                            measurementModel.MinValue = PB.MinValue;
                            measurementModel.MinDescription = PB.MinDescription;
                            measurementModel.MaxDescription = PB.MaxDescription;
                            measurementModel.MaxValue = PB.MaxValue;
                            measurementModel.IsEnforced = PB.IsEnforced;
                        }
                        RSM.Measurements.Add(measurementModel);
                    }

                    // Adhoc measurements go here
                    foreach (var AddhocMeasurement in systemmeasurement.AdhocMeasurements)
                    {
                        var measurementModel = new MeasurementModel();

                        measurementModel.Id = AddhocMeasurement.Id;
                        measurementModel.Name = AddhocMeasurement.AdhocParameter.Name;
                        measurementModel.Source = AddhocMeasurement.AdhocParameter.Source;
                        measurementModel.HasBounds = false;//Assume it doesn't and update if it does
                        measurementModel.Value = AddhocMeasurement.Value;
                        measurementModel.Unit = AddhocMeasurement.AdhocParameter.Unit;
                        measurementModel.IsApplicable = AddhocMeasurement.IsApplicable;
                        measurementModel.Comment = AddhocMeasurement.Comment;
                        measurementModel.IsAdhoc = true; // this is an adhoc measurement

                        //Despite being able to enter multiple of these only grab the first
                        //<Todo>: Restrict adding multiple global or local of the same site
                        //Grab the global scoped parameter if it actually exists
                        var PB = AddhocMeasurement.AdhocParameter.ParameterBounds.FirstOrDefault();

                        //If there are no global bounds try and grab the first local bound
                        //if (PB == null) {
                        //    PB = measurement.Parameter.ParameterBounds.FirstOrDefault(x => x.Site == Report.Site);
                        //}

                        if (PB != null)
                        {
                            measurementModel.HasBounds = true;

                            measurementModel.Type = PB.Type;
                            measurementModel.Range = PB.Range;
                            measurementModel.MinValue = PB.MinValue;
                            measurementModel.MinDescription = PB.MinDescription;
                            measurementModel.MaxDescription = PB.MaxDescription;
                            measurementModel.MaxValue = PB.MaxValue;
                            measurementModel.IsEnforced = PB.IsEnforced;
                        }
                        RSM.Measurements.Add(measurementModel);
                    }


                    buildingModel.Systems.Add(RSM);
                    buildingModel.Id = systemmeasurement.BuildingSystem.Building.Id;
                    buildingModel.Name = systemmeasurement.BuildingSystem.Building.Name;
                    //Model.Buildings.Add(buildingModel);
                }
                if (buildingModel.Id == building.Id)
                {
                    Model.Buildings.Add(buildingModel);
                }
            }
            return Model;
        }


        public ViewReportModel BuildViewReportModel(Report Report)
        {
            var ret = (ViewReportModel)BuildReportModel(Report);
            ret.Attachments = Report.Attachments.Select(a => new NamedEntity() { Id = a.Id, Name = a.Name }).ToList();
            ret.CreatedDate = Report.CreationDate;
            ret.SubmittedDate = Report.SubmissionDate.HasValue ? Report.SubmissionDate.Value : (DateTime?)null;

            if (string.IsNullOrEmpty(Report.User.FirstName) || string.IsNullOrEmpty(Report.User.LastName))
            {
                ret.Reporter = Report.User.UserName;
            }
            else
            {
                ret.Reporter = Report.User.FirstName + " " + Report.User.LastName;
            }

            if (Report.User.Vendor != null)
            {
                ret.Vendor = Report.User.Vendor.Name;
            }

            return ret;
        }

        public bool HasRequestedUnsubmit(Report report)
        {
            return report.SubmissionDate.HasValue && report.UnsubmitRequestedDate.HasValue && report.UnsubmitRequestedDate.Value > report.SubmissionDate.Value;
        }

        public void Delete(Report report)
        {
            string storageType = ConfigurationManager.AppSettings["StorageType"].ToUpper();

            using (var context = new WTContext())
            {
                var attachmentGuids = report.Attachments.Where(att => att.Report.Id == report.Id).Select(a => a.StorageId);
                foreach (var a in attachmentGuids)
                {
                    if (storageType == Constants.StorageType_AWSS3)
                    {
                        _AWSS3Storage.Remove(a);
                    }
                    else
                    {
                        _storage.Remove(a);
                    }
                }

                SqlParameter reportToDelete = new SqlParameter("@ReportId", SqlDbType.Int);
                reportToDelete.Value = report.Id;
                context.Database.ExecuteSqlCommand("spDeleteWaterTreatmentReport @ReportId", reportToDelete);
            }
        }

        public void Delete(int reportId)
        {
            var report = Context.Reports.Find(reportId);
            Delete(report);
        }
    }
}