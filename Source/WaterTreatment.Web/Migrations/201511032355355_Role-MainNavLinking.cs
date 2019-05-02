namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleMainNavLinking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MainSectionRole",
                c => new
                    {
                        MainSection_Id = c.Int(nullable: false),
                        Role_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MainSection_Id, t.Role_Id })
                .ForeignKey("dbo.MainSection", t => t.MainSection_Id, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.Role_Id, cascadeDelete: true)
                .Index(t => t.MainSection_Id)
                .Index(t => t.Role_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MainSectionRole", "Role_Id", "dbo.Role");
            DropForeignKey("dbo.MainSectionRole", "MainSection_Id", "dbo.MainSection");
            DropIndex("dbo.MainSectionRole", new[] { "Role_Id" });
            DropIndex("dbo.MainSectionRole", new[] { "MainSection_Id" });
            DropTable("dbo.MainSectionRole");
        }
    }
}
