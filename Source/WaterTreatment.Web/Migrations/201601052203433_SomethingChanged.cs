namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomethingChanged : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserSiteAccessBuildingSystem", newName: "BuildingSystemUserSiteAccess");
            DropPrimaryKey("dbo.BuildingSystemUserSiteAccess");
            AddPrimaryKey("dbo.BuildingSystemUserSiteAccess", new[] { "BuildingSystem_Id", "UserSiteAccess_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.BuildingSystemUserSiteAccess");
            AddPrimaryKey("dbo.BuildingSystemUserSiteAccess", new[] { "UserSiteAccess_Id", "BuildingSystem_Id" });
            RenameTable(name: "dbo.BuildingSystemUserSiteAccess", newName: "UserSiteAccessBuildingSystem");
        }
    }
}
