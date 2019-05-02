using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public class Attachment : NamedEntity
    {

        public Guid StorageId { get; set; }
        public virtual Report Report { get; set; }

    }

}