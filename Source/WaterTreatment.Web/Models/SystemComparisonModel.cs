using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models {
    public class SystemComparisonModel {
        public string SystemTypeName { get; set; }
        public Dictionary<int, ParameterMetaDatum> ParameterMetaData { get; set; }
        public Dictionary<int, SystemMetaDatum> SystemMetaData { get; set; }
        public Dictionary<int, Dictionary<int, ComparisonDatum>> ComparisonData { get; set; }
    }

    public class SystemMetaDatum {
        public int Id { get; set; }
        public string Location { get; set; }
        public string SiteName { get; set; }
        public string BuildingName { get; set; }
        public string ReportDate { get; set; }
    }

    public class ComparisonDatum {
        public string Value { get; set; }
        public bool IsOutOfBounds { get; set; }
        public DateTime? ReportDate { get; set; }
    }

    public class ParameterMetaDatum {
        public string Name { get; set; }
        public decimal? MinBound { get; set; }
        public decimal? MaxBound { get; set; }
        public string Type { get; set; }
    }

}