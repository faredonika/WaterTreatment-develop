namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportCopiedFrom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Report", "CopiedFrom", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Report", "CopiedFrom");
        }
    }
}
