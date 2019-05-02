namespace WaterTreatment.Web.Migrations {
    using System;
    using System.Data.Entity.Migrations;

    public partial class Sites : DbMigration {
        public override void Up() {
            CreateTable(
                "dbo.Site",
                c => new {
                    Id = c.Int(nullable: false, identity: true),
                    Location = c.String(nullable: false, maxLength: 1024),
                    Name = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Building",
                c => new {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    Site_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.Site_Id)
                .Index(t => t.Site_Id);

            CreateTable(
                "dbo.BuildingSystem",
                c => new {
                    Id = c.Int(nullable: false, identity: true),
                    Location = c.String(nullable: false, maxLength: 1024),
                    Name = c.String(nullable: false),
                    SystemType_Id = c.Int(nullable: false),
                    Building_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemType", t => t.SystemType_Id, cascadeDelete: false)
                .ForeignKey("dbo.Building", t => t.Building_Id)
                .Index(t => t.SystemType_Id)
                .Index(t => t.Building_Id);
        }

        public override void Down() {
            DropForeignKey("dbo.Building", "Site_Id", "dbo.Site");
            DropForeignKey("dbo.BuildingSystem", "Building_Id", "dbo.Building");
            DropForeignKey("dbo.BuildingSystem", "SystemType_Id", "dbo.SystemType");
            DropIndex("dbo.BuildingSystem", new[] { "Building_Id" });
            DropIndex("dbo.BuildingSystem", new[] { "SystemType_Id" });
            DropIndex("dbo.Building", new[] { "Site_Id" });
            DropTable("dbo.BuildingSystem");
            DropTable("dbo.Building");
            DropTable("dbo.Site");
        }
    }
}