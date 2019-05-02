using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WaterTreatment.Web.Entities.Ref
{
    public static class RefDataAnalyzer
    {

        public static List<EntityMetaRefData> Analyze(RefDataModel Model)
        {

            MethodInfo method = typeof(EntityMetaRefData).GetMethod("ExtractReference");

            var built = new List<EntityMetaRefData>();

            foreach (var property in Model.GetType().GetProperties())
            {
                var entityType = property.PropertyType.GenericTypeArguments.First();

                //For now I've made an executive decision that only Named Entities can be referenced strongly
                if (entityType.GetInterfaces().Contains(typeof(INamedEntity)))
                {

                    var metaData = new EntityMetaRefData();

                    metaData.EntityName = entityType.Name;
                    metaData.ListName = property.Name;

                    MethodInfo generic = method.MakeGenericMethod(entityType);
                    var entities = property.GetValue(Model) as IList;

                    if (entities == null)
                        continue;

                    try
                    {
                        foreach (var entity in entities)
                        {
                            if (entity == null)
                                throw new Exception("entity is null");
                            if(generic == null)
                                throw new Exception("generic is null");
                            if (metaData == null)
                                throw new Exception("metaData is null");

                            generic.Invoke(metaData, new object[] { entity });
                        }

                    }
                    catch (Exception e)
                    {
                        throw new Exception(entityType.Name + " ::: " + e.Message);
                    }

                    built.Add(metaData);
                }

            }

            return built;
        }

    }
}