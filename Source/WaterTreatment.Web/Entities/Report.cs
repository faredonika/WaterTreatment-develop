using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public class Report : Entity
    {

        public Report()
        {
            Attachments = new HashSet<Attachment>();
            SystemMeasurements = new HashSet<SystemMeasurement>();
        }

        public DateTime CreationDate { get; set; }
        public DateTime? MeasurementDate { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public string ReportType { get; set; }


        public string Notes { get; set; }

        public DateTime? UnsubmitRequestedDate { get; set; }

        public int? CopiedFrom { get; set; }

        public virtual User User { get; set; }

        public virtual Site Site { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<SystemMeasurement> SystemMeasurements { get; set; }

    }

}