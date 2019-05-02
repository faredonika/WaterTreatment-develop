using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Text;
using WaterTreatment.Web.Entities;

namespace WaterTreatment.Web.Migrations
{   

    internal sealed partial class Configuration : DbMigrationsConfiguration<WaterTreatment.Web.WTContext>
    {

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WaterTreatment.Web.WTContext context)
        {

            try
            {
                //RefDataSeed(context);
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
        }

    }

}
