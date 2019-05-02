using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WaterTreatment.Web.Entities;

namespace WaterTreatment.Web.Models
{

	public class ReportModel
	{

        public ReportModel()
        {
            Buildings = new List<ReportBuildingModel>();
        }

        public int Id { get; set; }
        public string SiteName { get; set; }
        public string MeasurementDate { get; set; }
        public string Notes { get; set; }

        public List<ReportBuildingModel> Buildings { get; set; }
	}

    public class ViewReportModel : ReportModel {    // We use ReportModel in many views without actually needing the Attachments, so this class exists for when we explicitly need them
        public ViewReportModel() : base() {
            Attachments = new List<NamedEntity>();
        }
        public string Reporter { get; set; }
        public string Vendor { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public List<NamedEntity> Attachments { get; set; }

    }

    public class ReportBuildingModel
    {

        public ReportBuildingModel()
        {
            Systems = new List<ReportSystemMeasurementModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public List<ReportSystemMeasurementModel> Systems { get; set; }

    }

    public class ReportSystemMeasurementModel
    {

        public ReportSystemMeasurementModel()
        {
            Measurements = new List<MeasurementModel>();
        }

        public int Id { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string SystemName { get; set; }
        public string ReasonSkipped { get; set; }

        public List<MeasurementModel> Measurements { get; set; }

    }

    public class MeasurementModel
    {

        public int Id { get; set; }

        public string Name { get; set; }
        public string Source { get; set; }
        public bool HasBounds { get; set; }
        public string Unit { get; set; }

        public string Type { get; set; }
        public string Range { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string MinDescription { get; set; }
        public string MaxDescription { get; set; }
        public bool IsEnforced { get; set; }
        public bool IsApplicable { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        public string Value { get; set; }
        public bool IsAdhoc { get; set; }
        public bool Delete { get; set; }
    }
}