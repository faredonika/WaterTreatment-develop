using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public class AuditEntity : Entity
    {

        public DateTime ModifiedOn { get; set; }
        public EntityState EventType { get; set; }

        //Used to signify atomic alterations 
        //Since altering multiple fields on the same row produces multiples entries in this table
        public Guid EventInstance { get; set; }

        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int EntityId { get; set; }
        public string OriginalValue { get; set; }
        public string CurrentValue { get; set; }
        
        public virtual User ModifiedBy { get; set; }

    }

}