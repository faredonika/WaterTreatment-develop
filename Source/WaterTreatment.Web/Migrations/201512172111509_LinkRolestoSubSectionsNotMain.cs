namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkRolestoSubSectionsNotMain : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MainSectionRole", "MainSection_Id", "dbo.MainSection");
            DropForeignKey("dbo.MainSectionRole", "Role_Id", "dbo.Role");
            DropIndex("dbo.MainSectionRole", new[] { "MainSection_Id" });
            DropIndex("dbo.MainSectionRole", new[] { "Role_Id" });
            CreateTable(
                "dbo.SubSectionRole",
                c => new
                    {
                        SubSection_Id = c.Int(nullable: false),
                        Role_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubSection_Id, t.Role_Id })
                .ForeignKey("dbo.SubSection", t => t.SubSection_Id, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.Role_Id, cascadeDelete: true)
                .Index(t => t.SubSection_Id)
                .Index(t => t.Role_Id);
            
            DropTable("dbo.MainSectionRole");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MainSectionRole",
                c => new
                    {
                        MainSection_Id = c.Int(nullable: false),
                        Role_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MainSection_Id, t.Role_Id });
            
            DropForeignKey("dbo.SubSectionRole", "Role_Id", "dbo.Role");
            DropForeignKey("dbo.SubSectionRole", "SubSection_Id", "dbo.SubSection");
            DropIndex("dbo.SubSectionRole", new[] { "Role_Id" });
            DropIndex("dbo.SubSectionRole", new[] { "SubSection_Id" });
            DropTable("dbo.SubSectionRole");
            CreateIndex("dbo.MainSectionRole", "Role_Id");
            CreateIndex("dbo.MainSectionRole", "MainSection_Id");
            AddForeignKey("dbo.MainSectionRole", "Role_Id", "dbo.Role", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MainSectionRole", "MainSection_Id", "dbo.MainSection", "Id", cascadeDelete: true);
        }
    }
}
