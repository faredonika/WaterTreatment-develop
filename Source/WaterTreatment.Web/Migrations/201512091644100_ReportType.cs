namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Report", "ReportType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Report", "ReportType");
        }
    }
}
