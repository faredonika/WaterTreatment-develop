using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public class SystemType : NamedEntity
    {

        public SystemType()
        {
            Parameters = new HashSet<Parameter>();
            BuildingSystems = new HashSet<BuildingSystem>();
        }

        [DisplayName("System Type Name")]
        [Required]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        public bool IsActive { get; set; }

        public virtual ICollection<Parameter> Parameters { get; set; }
        public virtual ICollection<BuildingSystem> BuildingSystems { get; set; }

    }
}