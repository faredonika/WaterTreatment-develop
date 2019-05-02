namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMeasurementReasonSkipped : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Note", "Report_Id", "dbo.Report");
            DropIndex("dbo.Note", new[] { "Report_Id" });
            AddColumn("dbo.SystemMeasurement", "ReasonSkipped", c => c.String());
            DropColumn("dbo.SystemMeasurement", "Skipped");
            DropTable("dbo.Note");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Note",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Report_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.SystemMeasurement", "Skipped", c => c.Boolean(nullable: false));
            DropColumn("dbo.SystemMeasurement", "ReasonSkipped");
            CreateIndex("dbo.Note", "Report_Id");
            AddForeignKey("dbo.Note", "Report_Id", "dbo.Report", "Id");
        }
    }
}
