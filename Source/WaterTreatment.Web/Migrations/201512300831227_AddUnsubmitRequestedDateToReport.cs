namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnsubmitRequestedDateToReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Report", "UnsubmitRequestedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Report", "UnsubmitRequestedDate");
        }
    }
}
