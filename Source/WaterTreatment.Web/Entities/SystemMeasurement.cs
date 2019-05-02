using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public class SystemMeasurement : Entity
    {

        public SystemMeasurement()
        {
            Measurements = new HashSet<Measurement>();
            AdhocMeasurements = new HashSet<AdhocMeasurement>();
        }

        public string ReasonSkipped { get; set; }

        public virtual Report Report { get; set; }
        public virtual BuildingSystem BuildingSystem { get; set; }
        public virtual ICollection<Measurement> Measurements { get; set; }
        public virtual ICollection<AdhocMeasurement> AdhocMeasurements { get; set; }

    }

}