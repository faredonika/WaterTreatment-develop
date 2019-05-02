using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public class Measurement : Entity
    {

        public string Value { get; set; }
        public bool IsApplicable { get; set; }
        //Signifies if the value is Out of Bounds. Only calculated on edit post
        public bool BakedOOB { get; set; }
        public string Comment { get; set; }

        public virtual Parameter Parameter { get; set; }
        public virtual SystemMeasurement SystemMeasurement { get; set; }

    }

}