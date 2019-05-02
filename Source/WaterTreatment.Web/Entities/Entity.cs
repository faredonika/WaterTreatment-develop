using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities
{

    public interface IEntity
    {
        int Id { get; set; }
    }

    public interface INamedEntity : IEntity
    {
        string Name { get; set; }
    }

    public class Entity : IEntity
    {
        public int Id { get; set; }
    }

    public class NamedEntity : Entity, INamedEntity
    {
        public string Name { get; set; }
    }

    public class NamedAuditedEntity : AuditEntity, INamedEntity
    {
        public string Name { get; set; }
    }

}