using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models {
    public class SystemTypeRollup {
        public string SystemType { get; set; }
        public int OutOfBoundsCount { get; set; }
        public int LateCount { get; set; }
        public int NeverMeasuredCount { get; set; }
        public DateTime? OldestReportDate { get; set; }
        public DateTime? MostRecentReportDate { get; set; }
        public List<MeasurementDatum> Measurements { get; set; }
    }

    public class MeasurementDatum {
        public string ParameterName { get; set; }
        public decimal? MinBound { get; set; }
        public decimal? MaxBound { get; set; }
        public string Value { get; set; }
        public string Units { get; set; }
        public bool IsOutOfBounds { get; set; }
        public int? DaysElapsed { get; set; }
        public DateTime? ReportDate { get; set; }
        public int Frequency { get; set; }
        public bool? HasComments { get; set; }
        public double ElapsedRatio { get; set; }
        public bool IsLate { get; set; }
        public string ParameterType { get; set; }
    }
}