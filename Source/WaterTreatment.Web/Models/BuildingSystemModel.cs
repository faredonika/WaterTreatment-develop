using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class BuildingSystemModel
    {

        public int Id { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Location { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int SystemTypeId { get; set; }

    }

}