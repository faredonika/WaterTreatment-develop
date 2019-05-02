using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models {
    public class MeasurementComparisonModel {
        public string SystemType { get; set; }
        public string Parameter { get; set; }
        public string ParameterType { get; set; }
        public string Units { get; set; }
        public Dictionary<int, string> Reports { get; set; }  // list of headers Ex: report dates
        public Dictionary<int,  MeasurementComparisonDatum> MeasurementComparisonData { get; set; }
    }

    public class MeasurementComparisonDatum {
        public string SiteName { get; set; }
        public string BuildingName { get; set; }
        public string Location { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public Dictionary<int,decimal?> Values { get; set; }
    }
}




//public List<MeasurementComparisonDatum> MeasurementComparisonData = new List<MeasurementComparisonDatum>();
//public List<MeasurementComparisonDatum> MissingMeasurements = new List<MeasurementComparisonDatum>();