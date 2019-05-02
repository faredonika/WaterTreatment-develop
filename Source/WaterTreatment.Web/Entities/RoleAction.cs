using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public class RoleAction : Entity
    {

        public int Order { get; set; }

        public virtual Role Role { get; set; }
        public virtual LandingAction Action { get; set; }

    }

}