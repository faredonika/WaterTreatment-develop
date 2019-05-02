using Newtonsoft.Json;
using System.Collections.Generic;

namespace WaterTreatment.Web.Entities
{

    public class Building : NamedEntity
    {

        public Building()
        {
            Systems = new HashSet<BuildingSystem>();
        }

        public string RPUID { get; set; }
        public string RPSUID { get; set; }
        public string BuildingNumber { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore]
        public virtual Site Site { get; set; }
        public virtual ICollection<BuildingSystem> Systems { get; set; }

    }

}