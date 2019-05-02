using System;

namespace WaterTreatment.Web.Models
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Site { get; set; }
        public DateTime StartedOn { get; set; }
        public int DaysElapsed { get; set; }
    }

    public class ReportSummaryViewModel : ReportViewModel
    {
        public DateTime? SubmittedOn { get; set; }
        public string MeasuredOn { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string Vendor { get; set; }
        public bool CanRequestUnsubmit { get; set; }
        public bool HasRequestedUnsubmit { get; set; }
        public bool CanUnsubmit { get; set; }
    }


    public class ReportCopiedViewModel : ReportViewModel
    {
        public int? Copied { get; set; }
    }
}