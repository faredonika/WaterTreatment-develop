using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{

    public class ParameterModel
    {

        public ParameterModel()
        {
            Bounds = new List<BoundModel>();
        }

        public int Id { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Frequency { get; set; }
        public int ParameterTypeId { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Unit { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Source { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Link { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Use { get; set; }

        public List<BoundModel> Bounds { get; set; }

    }

}