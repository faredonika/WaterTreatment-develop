namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BakedOOBField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Measurement", "BakedOOB", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Measurement", "BakedOOB");
        }
    }
}
