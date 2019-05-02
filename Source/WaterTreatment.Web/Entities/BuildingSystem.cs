using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterTreatment.Web.Entities
{

    public class BuildingSystem : Entity
    {
        public BuildingSystem()
        {
            SystemMeasurements = new HashSet<SystemMeasurement>();
            Users = new HashSet<User>();
        }

        public string Location { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual Building Building { get; set; }
        public virtual SystemType SystemType { get; set; }

        public virtual ICollection<SystemMeasurement> SystemMeasurements { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }

}