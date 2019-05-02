using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models {
    public class CollectionsModel {
        public Dictionary<int, SiteCounts> SiteCollectionData { get; set; } = new Dictionary<int, SiteCounts>();
    }

    public class SiteCounts {
        public string Name { get; set; }
        public int InBounds { get; set; }
        public int OutOfBounds { get; set; }
        public int NotCollected { get; set; }
        public int Late { get; set; }
    }
}