using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{
    public class Vendor : Entity
    {
        public Vendor()
        {
            Members = new HashSet<User>();
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string PointOfContact { get; set; }

        public virtual ICollection<User> Members { get; set; }
    }
}