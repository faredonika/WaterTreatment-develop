using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{
    [Table("SubSection")]
    public class SubSection : NavSection
    {

        public bool Enabled { get; set; }

        public virtual MainSection MainSection { get; set; }
        public virtual ICollection<Role> Roles { get; set; }

    }

}