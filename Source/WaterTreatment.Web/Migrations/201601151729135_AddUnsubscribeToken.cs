namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnsubscribeToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportSubscription", "UnsubscribeAuthToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportSubscription", "UnsubscribeAuthToken");
        }
    }
}
