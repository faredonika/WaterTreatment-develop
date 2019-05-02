using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class SearchModel
    {
        public SearchModel()
        {
            Filters = new Dictionary<string, string>();
        }

        public int MaxResults { get; set; }
        public int Offset { get; set; }
        public bool ShouldForwardSearch { get; set; }
        public String SortBy { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public Dictionary<string, string> Filters { get; set; }

    }

}