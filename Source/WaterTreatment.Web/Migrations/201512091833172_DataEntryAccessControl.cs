namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataEntryAccessControl : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserSite", "User_Id", "dbo.User");
            DropForeignKey("dbo.UserSite", "Site_Id", "dbo.Site");
            DropIndex("dbo.UserSite", new[] { "User_Id" });
            DropIndex("dbo.UserSite", new[] { "Site_Id" });
            CreateTable(
                "dbo.SiteAccess",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Site_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.Site_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.SystemTypeAccess",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Site_Id = c.Int(),
                        System_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SiteAccess", t => t.Site_Id)
                .ForeignKey("dbo.SystemType", t => t.System_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.System_Id);
            
            AddColumn("dbo.User", "Site_Id", c => c.Int());
            CreateIndex("dbo.User", "Site_Id");
            AddForeignKey("dbo.User", "Site_Id", "dbo.Site", "Id");
            DropTable("dbo.UserSite");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserSite",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Site_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Site_Id });
            
            DropForeignKey("dbo.User", "Site_Id", "dbo.Site");
            DropForeignKey("dbo.SiteAccess", "User_Id", "dbo.User");
            DropForeignKey("dbo.SystemTypeAccess", "System_Id", "dbo.SystemType");
            DropForeignKey("dbo.SystemTypeAccess", "Site_Id", "dbo.SiteAccess");
            DropForeignKey("dbo.SiteAccess", "Site_Id", "dbo.Site");
            DropIndex("dbo.SystemTypeAccess", new[] { "System_Id" });
            DropIndex("dbo.SystemTypeAccess", new[] { "Site_Id" });
            DropIndex("dbo.SiteAccess", new[] { "User_Id" });
            DropIndex("dbo.SiteAccess", new[] { "Site_Id" });
            DropIndex("dbo.User", new[] { "Site_Id" });
            DropColumn("dbo.User", "Site_Id");
            DropTable("dbo.SystemTypeAccess");
            DropTable("dbo.SiteAccess");
            CreateIndex("dbo.UserSite", "Site_Id");
            CreateIndex("dbo.UserSite", "User_Id");
            AddForeignKey("dbo.UserSite", "Site_Id", "dbo.Site", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserSite", "User_Id", "dbo.User", "Id", cascadeDelete: true);
        }
    }
}
