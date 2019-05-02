using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class BuildingModel
    {

        public BuildingModel()
        {

            RPUID = String.Empty;
            RPSUID = String.Empty;

            Systems = new List<BuildingSystemModel>();
        }
        
        public int Id { get; set; }
        public String Name { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RPUID { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RPSUID { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string BuildingNumber { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public List<BuildingSystemModel> Systems { get; set; }

    }

}