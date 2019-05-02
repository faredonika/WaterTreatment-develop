using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{
    public class VendorModel
    {
        public VendorModel()
        {
            Members = new HashSet<UserEditModel>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string PointOfContact { get; set; }
        public ICollection<UserEditModel> Members { get; set; }
    }
}