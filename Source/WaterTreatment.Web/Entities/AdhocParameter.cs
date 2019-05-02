using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{
    public class AdhocParameter : NamedEntity
    {

        public AdhocParameter()
        {
            AdhocMeasurements = new HashSet<AdhocMeasurement>();
            ParameterBounds = new HashSet<ParameterBound>();
        }

        public string Unit { get; set; }
        public string Source { get; set; }
        public string Link { get; set; }
        public string Use { get; set; }
        public string Frequency { get; set; }

        public virtual ParameterType Type { get; set; }
        public virtual BuildingSystem BuildingSystem { get; set; }

        [JsonIgnore]
        public virtual ICollection<AdhocMeasurement> AdhocMeasurements { get; set; }
        public virtual ICollection<ParameterBound> ParameterBounds { get; set; }

    }
}