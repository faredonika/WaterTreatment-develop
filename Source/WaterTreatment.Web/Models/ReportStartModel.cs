using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class ReportStartModel
    {

        [Required]
        public int SiteId { get; set; }
        public string Use { get; set; }

    }
}