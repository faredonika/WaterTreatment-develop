namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Report_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Report", t => t.Report_Id)
                .Index(t => t.Report_Id);
            
            CreateTable(
                "dbo.Report",
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
                "dbo.SystemMeasurement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Building_Id = c.Int(),
                        Report_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Building", t => t.Building_Id)
                .ForeignKey("dbo.Report", t => t.Report_Id)
                .Index(t => t.Building_Id)
                .Index(t => t.Report_Id);
            
            CreateTable(
                "dbo.Measurement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Parameter_Id = c.Int(),
                        SystemMeasurement_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Parameter", t => t.Parameter_Id)
                .ForeignKey("dbo.SystemMeasurement", t => t.SystemMeasurement_Id)
                .Index(t => t.Parameter_Id)
                .Index(t => t.SystemMeasurement_Id);
            
            CreateTable(
                "dbo.Note",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Report_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Report", t => t.Report_Id)
                .Index(t => t.Report_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Report", "User_Id", "dbo.User");
            DropForeignKey("dbo.Report", "Site_Id", "dbo.Site");
            DropForeignKey("dbo.Note", "Report_Id", "dbo.Report");
            DropForeignKey("dbo.SystemMeasurement", "Report_Id", "dbo.Report");
            DropForeignKey("dbo.Measurement", "SystemMeasurement_Id", "dbo.SystemMeasurement");
            DropForeignKey("dbo.Measurement", "Parameter_Id", "dbo.Parameter");
            DropForeignKey("dbo.SystemMeasurement", "Building_Id", "dbo.Building");
            DropForeignKey("dbo.Attachment", "Report_Id", "dbo.Report");
            DropIndex("dbo.Note", new[] { "Report_Id" });
            DropIndex("dbo.Measurement", new[] { "SystemMeasurement_Id" });
            DropIndex("dbo.Measurement", new[] { "Parameter_Id" });
            DropIndex("dbo.SystemMeasurement", new[] { "Report_Id" });
            DropIndex("dbo.SystemMeasurement", new[] { "Building_Id" });
            DropIndex("dbo.Report", new[] { "User_Id" });
            DropIndex("dbo.Report", new[] { "Site_Id" });
            DropIndex("dbo.Attachment", new[] { "Report_Id" });
            DropTable("dbo.Note");
            DropTable("dbo.Measurement");
            DropTable("dbo.SystemMeasurement");
            DropTable("dbo.Report");
            DropTable("dbo.Attachment");
        }
    }
}
