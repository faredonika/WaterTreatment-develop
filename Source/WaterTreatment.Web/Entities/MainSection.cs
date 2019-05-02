using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    [Table("MainSection")]
    public class MainSection : NavSection
    {

        public MainSection()
        {
            SubSections = new HashSet<SubSection>();
        }

        public bool Clickable { get; set; }

        public virtual ICollection<SubSection> SubSections { get; set; }

    }

}