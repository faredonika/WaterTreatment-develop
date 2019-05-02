using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class SiteSearchModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int BuildingCount { get; set; }

    }

}