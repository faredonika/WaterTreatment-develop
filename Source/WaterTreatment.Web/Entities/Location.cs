using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WaterTreatment.Web.Entities
{
    public class Location: Entity
    {
        public Location()
        {
            Users = new HashSet<User>();
        }

        [StringLength(50)]
        public string State { get; set; }
        public bool International { get; set; } 

        public virtual ICollection<User> Users { get; set; }
    }
}