namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FrequencyMOve : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parameter", "Frequency", c => c.String());
            DropColumn("dbo.Measurement", "Frequency");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Measurement", "Frequency", c => c.String());
            DropColumn("dbo.Parameter", "Frequency");
        }
    }
}
