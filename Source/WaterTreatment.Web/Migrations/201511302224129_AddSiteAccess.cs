namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteAccess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSite",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Site_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Site_Id })
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Site", t => t.Site_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Site_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserSite", "Site_Id", "dbo.Site");
            DropForeignKey("dbo.UserSite", "User_Id", "dbo.User");
            DropIndex("dbo.UserSite", new[] { "Site_Id" });
            DropIndex("dbo.UserSite", new[] { "User_Id" });
            DropTable("dbo.UserSite");
        }
    }
}
