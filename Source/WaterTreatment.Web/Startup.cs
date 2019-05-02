using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WaterTreatment.Web.Startup))]
namespace WaterTreatment.Web
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureAuth(app);
        }
    }
}