using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class SystemCreateModel
    {

        public SystemCreateModel()
        {
            Parameters = new List<ParameterModel>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsActive { get; set; }

        public List<ParameterModel> Parameters { get; set; }

    }

}