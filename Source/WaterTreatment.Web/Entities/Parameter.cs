using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{
    public class Parameter : NamedEntity
    {

        public Parameter()
        {
            Measurements = new HashSet<Measurement>();
            ParameterBounds = new HashSet<ParameterBound>();
        }
        public int Id { get; set; }
        public string Unit { get; set; }
        public string Source { get; set; }
        public string Link { get; set; }
        public string Use { get; set; }
        public string Frequency { get; set; }
        //AltParameter has been added to link with alternative paramater id 
        public int? AltParameter { get; set; }

        public virtual ParameterType Type { get; set; }
        public virtual SystemType SystemType { get; set; }

        [JsonIgnore]
        public virtual ICollection<Measurement> Measurements { get; set; }
        public virtual ICollection<ParameterBound> ParameterBounds { get; set; }

    }
}