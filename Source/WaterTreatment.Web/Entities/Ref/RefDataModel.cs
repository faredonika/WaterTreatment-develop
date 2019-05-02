using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Entities.Ref
{

    public class RefDataModel
    {

        public List<ExtensionWhitelist> Whitelist { get; set; }
        public List<LandingAction> LandingActions { get; set; }
        public List<MainSection> MainSections { get; set; }
        public List<ParameterType> ParameterTypes { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<ParameterBound> ParameterBounds { get; set; }
        public List<SubSection> SubSections { get; set; }
        public List<Role> Roles { get; set; }
        public List<SystemType> SystemTypes { get; set; }
        public List<User> Users { get; set; }
        public List<Building> Buildings { get; set; }
        public List<BuildingSystem> BuildingSystems { get; set; }
        public List<Site> Sites { get; set; }
        //public List<UserSiteAccess> UserSiteAccess { get; set; }
        public List<Vendor> Vendors { get; set; }
        public List<Setting> Settings { get; set; }
    }

}