using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities.Ref
{
    public class EntityMetaRefData
    {

        public EntityMetaRefData()
        {
            NamedReferences = new List<Tuple<string, int>>();
        }

        public string EntityName { get; set; }
        public string ListName { get; set; }
        public List<Tuple<string, int>> NamedReferences { get; set; }

        public void ExtractReference<T>(T Entity) where T : INamedEntity
        {

            try
            {
                var stripped = Entity.Name.Replace(" ", "").Replace(".", "_").Replace("/", "_").Replace("(", "").Replace(")", "").Replace("-", "_").Replace(",", "_");

                NamedReferences.Add(new Tuple<string, int>(stripped, Entity.Id));
            }

            catch
            {
                throw new Exception("The inside fucking blew up");
            }

            
        }

    }
}