namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoolNotString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Measurement", "BakedOOB", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Measurement", "BakedOOB", c => c.String());
        }
    }
}
