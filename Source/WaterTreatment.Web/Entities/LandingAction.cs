using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    [Table("LandingAction")]
    public class LandingAction : SiteSection
    {

        public string Image { get; set; }
        public string HoverImage { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RoleAction> RoleActions { get; set; }

    }

}