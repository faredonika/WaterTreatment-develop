using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public class ParameterBound : Entity
    {

        public string Type { get; set; }
        public string Range { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string MinDescription { get; set; }
        public string MaxDescription { get; set; }
        public bool IsEnforced { get; set; }

        public virtual Parameter Parameter { get; set; }
        public virtual AdhocParameter AdhocParameter { get; set; }
        //   public virtual Site Site { get; set; }

    }
}