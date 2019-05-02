using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class BoundModel
    {

        public int Id { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Type { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Range { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public decimal? MinValue { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public decimal? MaxValue { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MinDescription { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MaxDescription { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public bool IsEnforced { get; set; }
        public int SiteId { get; set; }

    }

}