namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MeasurementComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Measurement", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Measurement", "Comment");
        }
    }
}
