namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsApplicable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Measurement", "IsApplicable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Measurement", "IsApplicable");
        }
    }
}
