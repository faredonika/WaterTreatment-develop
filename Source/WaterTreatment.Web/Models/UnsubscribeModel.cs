using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WaterTreatment.Web.Entities;

namespace WaterTreatment.Web.Models
{
    public class UnsubscribeModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; } 
    }
}