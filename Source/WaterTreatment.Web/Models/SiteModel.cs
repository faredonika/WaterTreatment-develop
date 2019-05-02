using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WaterTreatment.Web.Entities;

namespace WaterTreatment.Web.Models
{

    public class SiteModel
    {

        public SiteModel()
        {
            Buildings = new List<BuildingModel>();
        }

        public int Id { get; set; }

        [Required]
        public String Name { get; set; }
        [Required]
        public String Location { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public List<BuildingModel> Buildings { get; set; }

    }

}