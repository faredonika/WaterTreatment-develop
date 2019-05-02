namespace WaterTreatment.Web.Migrations {
    using System;
    using System.Data.Entity.Migrations;

    public partial class SiteSection : DbMigration {
        public override void Up() {
            CreateTable(
                "dbo.RoleSiteSectionMapping",
                c => new {
                    RoleId = c.Int(nullable: false),
                    LandingController = c.String(nullable: false, maxLength: 32),
                })
                .PrimaryKey(t => new { t.RoleId, t.LandingController })
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.OrderedRoleSiteSectionMapping",
                c => new {
                    Order = c.Int(nullable: false),
                    SiteSectionId = c.Int(nullable: false),
                    RoleSiteSectionMapping_RoleId = c.Int(),
                    RoleSiteSectionMapping_LandingController = c.String(maxLength: 32),
                })
                .PrimaryKey(t => new { t.Order, t.SiteSectionId })
                .ForeignKey("dbo.SiteSection", t => t.SiteSectionId, cascadeDelete: true)
                .ForeignKey("dbo.RoleSiteSectionMapping", t => new { t.RoleSiteSectionMapping_RoleId, t.RoleSiteSectionMapping_LandingController })
                .Index(t => t.SiteSectionId)
                .Index(t => new { t.RoleSiteSectionMapping_RoleId, t.RoleSiteSectionMapping_LandingController });

            CreateTable(
                "dbo.SiteSection",
                c => new {
                    Id = c.Int(nullable: false, identity: true),
                    Controller = c.String(nullable: false, maxLength: 32),
                    Action = c.String(nullable: false, maxLength: 32),
                    Header = c.String(nullable: false, maxLength: 128),
                    Description = c.String(maxLength: 512),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down() {
            DropForeignKey("dbo.OrderedRoleSiteSectionMapping", new[] { "RoleSiteSectionMapping_RoleId", "RoleSiteSectionMapping_LandingController" }, "dbo.RoleSiteSectionMapping");
            DropForeignKey("dbo.OrderedRoleSiteSectionMapping", "SiteSectionId", "dbo.SiteSection");
            DropForeignKey("dbo.RoleSiteSectionMapping", "RoleId", "dbo.Role");
            DropIndex("dbo.OrderedRoleSiteSectionMapping", new[] { "RoleSiteSectionMapping_RoleId", "RoleSiteSectionMapping_LandingController" });
            DropIndex("dbo.OrderedRoleSiteSectionMapping", new[] { "SiteSectionId" });
            DropIndex("dbo.RoleSiteSectionMapping", new[] { "RoleId" });
            DropTable("dbo.SiteSection");
            DropTable("dbo.OrderedRoleSiteSectionMapping");
            DropTable("dbo.RoleSiteSectionMapping");
        }
    }
}
