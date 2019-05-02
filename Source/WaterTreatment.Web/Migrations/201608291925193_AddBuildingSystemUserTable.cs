namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Text;
    
    public partial class AddBuildingSystemUserTable : DbMigration
    {
        public override void Up()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO [dbo].[UserBuildingSystem] ");
            builder.Append("SELECT dbo.UserSiteAccess.User_Id as [User_Id], dbo.BuildingSystemUserSiteAccess.BuildingSystem_Id as [BuildingSystem_Id] ");
            builder.Append("FROM dbo.BuildingSystemUserSiteAccess INNER JOIN ");
            builder.Append(" dbo.UserSiteAccess ON dbo.BuildingSystemUserSiteAccess.UserSiteAccess_Id = dbo.UserSiteAccess.Id ");

            DropForeignKey("dbo.BuildingSystemUserSiteAccess", "BuildingSystem_Id", "dbo.BuildingSystem");
            DropForeignKey("dbo.BuildingSystemUserSiteAccess", "UserSiteAccess_Id", "dbo.UserSiteAccess");
            DropIndex("dbo.BuildingSystemUserSiteAccess", new[] { "BuildingSystem_Id" });
            DropIndex("dbo.BuildingSystemUserSiteAccess", new[] { "UserSiteAccess_Id" });
            CreateTable(
                "dbo.UserBuildingSystem",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        BuildingSystem_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.BuildingSystem_Id })
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.BuildingSystem", t => t.BuildingSystem_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.BuildingSystem_Id);
            
            AddColumn("dbo.BuildingSystem", "UserSiteAccess_Id", c => c.Int());
            //run custom sql - update new table
            Sql(builder.ToString());
            CreateIndex("dbo.BuildingSystem", "UserSiteAccess_Id");
            AddForeignKey("dbo.BuildingSystem", "UserSiteAccess_Id", "dbo.UserSiteAccess", "Id");
            DropTable("dbo.BuildingSystemUserSiteAccess");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BuildingSystemUserSiteAccess",
                c => new
                    {
                        BuildingSystem_Id = c.Int(nullable: false),
                        UserSiteAccess_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BuildingSystem_Id, t.UserSiteAccess_Id });
            
            DropForeignKey("dbo.BuildingSystem", "UserSiteAccess_Id", "dbo.UserSiteAccess");
            DropForeignKey("dbo.UserBuildingSystem", "BuildingSystem_Id", "dbo.BuildingSystem");
            DropForeignKey("dbo.UserBuildingSystem", "User_Id", "dbo.User");
            DropIndex("dbo.UserBuildingSystem", new[] { "BuildingSystem_Id" });
            DropIndex("dbo.UserBuildingSystem", new[] { "User_Id" });
            DropIndex("dbo.BuildingSystem", new[] { "UserSiteAccess_Id" });
            DropColumn("dbo.BuildingSystem", "UserSiteAccess_Id");
            DropTable("dbo.UserBuildingSystem");
            CreateIndex("dbo.BuildingSystemUserSiteAccess", "UserSiteAccess_Id");
            CreateIndex("dbo.BuildingSystemUserSiteAccess", "BuildingSystem_Id");
            AddForeignKey("dbo.BuildingSystemUserSiteAccess", "UserSiteAccess_Id", "dbo.UserSiteAccess", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BuildingSystemUserSiteAccess", "BuildingSystem_Id", "dbo.BuildingSystem", "Id", cascadeDelete: true);
        }
    }
}
