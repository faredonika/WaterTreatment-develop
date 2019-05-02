using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class OOBModel
    {

        public OOBModel()
        {
        }
                
        public String Name { get; set; }
        public String SystemName { get; set; }
        public IEnumerable<OOBReportModel> Reports { get; set; }

    }

    public class OOBReportModel
    {

        public int Id { get; set; }
        public DateTime MeasurementDate { get; set;  }

    }

}