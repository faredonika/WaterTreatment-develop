namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreUserSiteRefactors : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SiteAccess", newName: "UserSiteAccess");
            DropForeignKey("dbo.SystemTypeAccess", "Site_Id", "dbo.SiteAccess");
            DropForeignKey("dbo.SystemTypeAccess", "System_Id", "dbo.SystemType");
            DropIndex("dbo.SystemTypeAccess", new[] { "Site_Id" });
            DropIndex("dbo.SystemTypeAccess", new[] { "System_Id" });
            CreateTable(
                "dbo.UserSiteAccessBuildingSystem",
                c => new
                    {
                        UserSiteAccess_Id = c.Int(nullable: false),
                        BuildingSystem_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserSiteAccess_Id, t.BuildingSystem_Id })
                .ForeignKey("dbo.UserSiteAccess", t => t.UserSiteAccess_Id, cascadeDelete: true)
                .ForeignKey("dbo.BuildingSystem", t => t.BuildingSystem_Id, cascadeDelete: true)
                .Index(t => t.UserSiteAccess_Id)
                .Index(t => t.BuildingSystem_Id);
            
            DropTable("dbo.SystemTypeAccess");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SystemTypeAccess",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Site_Id = c.Int(),
                        System_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.UserSiteAccessBuildingSystem", "BuildingSystem_Id", "dbo.BuildingSystem");
            DropForeignKey("dbo.UserSiteAccessBuildingSystem", "UserSiteAccess_Id", "dbo.UserSiteAccess");
            DropIndex("dbo.UserSiteAccessBuildingSystem", new[] { "BuildingSystem_Id" });
            DropIndex("dbo.UserSiteAccessBuildingSystem", new[] { "UserSiteAccess_Id" });
            DropTable("dbo.UserSiteAccessBuildingSystem");
            CreateIndex("dbo.SystemTypeAccess", "System_Id");
            CreateIndex("dbo.SystemTypeAccess", "Site_Id");
            AddForeignKey("dbo.SystemTypeAccess", "System_Id", "dbo.SystemType", "Id");
            AddForeignKey("dbo.SystemTypeAccess", "Site_Id", "dbo.SiteAccess", "Id");
            RenameTable(name: "dbo.UserSiteAccess", newName: "SiteAccess");
        }
    }
}
