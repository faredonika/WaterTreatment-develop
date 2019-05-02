using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WaterTreatment.Web.Entities
{

    public class Site : NamedEntity
    {
        public Site()
        {
            Buildings = new HashSet<Building>();
            SiteAccess = new HashSet<User>();
        }

        public string Location { get; set; }
        public bool IsActive { get; set; }
        public Nullable<DateTime> NextDigest { get; set; }

        [JsonIgnore]
        public virtual ICollection<Building> Buildings { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> SiteAccess { get; set; }
    }

}