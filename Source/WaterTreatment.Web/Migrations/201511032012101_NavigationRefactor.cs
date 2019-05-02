namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NavigationRefactor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RoleSiteSectionMapping", "RoleId", "dbo.Role");
            DropForeignKey("dbo.OrderedRoleSiteSectionMapping", "SiteSectionId", "dbo.SiteSection");
            DropForeignKey("dbo.OrderedRoleSiteSectionMapping", new[] { "RoleSiteSectionMapping_RoleId", "RoleSiteSectionMapping_LandingController" }, "dbo.RoleSiteSectionMapping");
            DropIndex("dbo.RoleSiteSectionMapping", new[] { "RoleId" });
            DropIndex("dbo.OrderedRoleSiteSectionMapping", new[] { "SiteSectionId" });
            DropIndex("dbo.OrderedRoleSiteSectionMapping", new[] { "RoleSiteSectionMapping_RoleId", "RoleSiteSectionMapping_LandingController" });
            CreateTable(
                "dbo.LandingAction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Controller = c.String(),
                        Action = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleAction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Action_Id = c.Int(),
                        Role_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LandingAction", t => t.Action_Id)
                .ForeignKey("dbo.Role", t => t.Role_Id)
                .Index(t => t.Action_Id)
                .Index(t => t.Role_Id);
            
            CreateTable(
                "dbo.MainSection",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Controller = c.String(),
                        Action = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubSection",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Enabled = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        Controller = c.String(),
                        Action = c.String(),
                        Name = c.String(),
                        MainSection_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MainSection", t => t.MainSection_Id)
                .Index(t => t.MainSection_Id);
            
            DropTable("dbo.RoleSiteSectionMapping");
            DropTable("dbo.OrderedRoleSiteSectionMapping");
            DropTable("dbo.SiteSection");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SiteSection",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Controller = c.String(nullable: false, maxLength: 32),
                        Action = c.String(nullable: false, maxLength: 32),
                        Header = c.String(nullable: false, maxLength: 128),
                        Description = c.String(maxLength: 512),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderedRoleSiteSectionMapping",
                c => new
                    {
                        Order = c.Int(nullable: false),
                        SiteSectionId = c.Int(nullable: false),
                        RoleSiteSectionMapping_RoleId = c.Int(),
                        RoleSiteSectionMapping_LandingController = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => new { t.Order, t.SiteSectionId });
            
            CreateTable(
                "dbo.RoleSiteSectionMapping",
                c => new
                    {
                        RoleId = c.Int(nullable: false),
                        LandingController = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => new { t.RoleId, t.LandingController });
            
            DropForeignKey("dbo.SubSection", "MainSection_Id", "dbo.MainSection");
            DropForeignKey("dbo.RoleAction", "Role_Id", "dbo.Role");
            DropForeignKey("dbo.RoleAction", "Action_Id", "dbo.LandingAction");
            DropIndex("dbo.SubSection", new[] { "MainSection_Id" });
            DropIndex("dbo.RoleAction", new[] { "Role_Id" });
            DropIndex("dbo.RoleAction", new[] { "Action_Id" });
            DropTable("dbo.SubSection");
            DropTable("dbo.MainSection");
            DropTable("dbo.RoleAction");
            DropTable("dbo.LandingAction");
            CreateIndex("dbo.OrderedRoleSiteSectionMapping", new[] { "RoleSiteSectionMapping_RoleId", "RoleSiteSectionMapping_LandingController" });
            CreateIndex("dbo.OrderedRoleSiteSectionMapping", "SiteSectionId");
            CreateIndex("dbo.RoleSiteSectionMapping", "RoleId");
            AddForeignKey("dbo.OrderedRoleSiteSectionMapping", new[] { "RoleSiteSectionMapping_RoleId", "RoleSiteSectionMapping_LandingController" }, "dbo.RoleSiteSectionMapping", new[] { "RoleId", "LandingController" });
            AddForeignKey("dbo.OrderedRoleSiteSectionMapping", "SiteSectionId", "dbo.SiteSection", "Id", cascadeDelete: true);
            AddForeignKey("dbo.RoleSiteSectionMapping", "RoleId", "dbo.Role", "Id", cascadeDelete: true);
        }
    }
}
