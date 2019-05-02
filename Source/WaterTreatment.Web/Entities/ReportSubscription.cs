using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{
    public class ReportSubscription : Entity
    {
        public string UnsubscribeAuthToken { get; set; }
        virtual public Site Site { get; set; }
        virtual public User User { get; set; }
    }
}