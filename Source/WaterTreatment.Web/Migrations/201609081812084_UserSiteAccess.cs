namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSiteAccess : DbMigration
    {
        public override void Up()
        {
            string sqlStr = "Insert into [dbo].[UserSiteAccess] SELECT [Site_Id] ,[User_Id] FROM [dbo].[SiteAccess2]";
            DropForeignKey("dbo.UserSiteAccess", "User_Id", "dbo.User");
            DropForeignKey("dbo.UserSiteAccess", "Site_Id", "dbo.Site");

            DropIndex("dbo.BuildingSystem", new[] { "UserSiteAccess_Id" });
            DropForeignKey("dbo.BuildingSystem", "UserSiteAccess_Id", "dbo.UserSiteAccess");
            DropColumn("dbo.BuildingSystem", "UserSiteAccess_Id");
            DropPrimaryKey("dbo.UserSiteAccess", "PK_dbo.UserSiteAccess");

            RenameTable("[UserSiteAccess]", "SiteAccess2");


            CreateTable(
                "dbo.UserSiteAccess",
                c => new
                    {
                        Site_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Site_Id, t.User_Id })
                .ForeignKey("dbo.Site", t => t.Site_Id, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true);

            Sql(sqlStr);
            DropTable("SiteAccess2");

        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserSiteAccess",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BuildingSystem", "UserSiteAccess_Id", c => c.Int());
            DropForeignKey("dbo.UserSiteAccess", "User_Id", "dbo.User");
            DropForeignKey("dbo.UserSiteAccess", "Site_Id", "dbo.Site");
            DropTable("dbo.UserSiteAccess");
            CreateIndex("dbo.BuildingSystem", "UserSiteAccess_Id");
            AddForeignKey("dbo.UserSiteAccess", "Site_Id", "dbo.Site", "Id");
            AddForeignKey("dbo.BuildingSystem", "UserSiteAccess_Id", "dbo.UserSiteAccess", "Id");
            AddForeignKey("dbo.UserSiteAccess", "User_Id", "dbo.User", "Id");
        }
    }
}
