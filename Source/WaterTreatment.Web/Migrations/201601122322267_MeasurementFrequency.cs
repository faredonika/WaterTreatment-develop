namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MeasurementFrequency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Measurement", "Frequency", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Measurement", "Frequency");
        }
    }
}
