using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class SystemModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string HasParameters { get; set; }
        public string InUse { get; set; }

    }

}