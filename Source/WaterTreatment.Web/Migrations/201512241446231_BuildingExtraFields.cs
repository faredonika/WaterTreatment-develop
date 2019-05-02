namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BuildingExtraFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Building", "RPUID", c => c.String());
            AddColumn("dbo.Building", "RPSUID", c => c.String());
            AddColumn("dbo.Building", "BuildingNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Building", "BuildingNumber");
            DropColumn("dbo.Building", "RPSUID");
            DropColumn("dbo.Building", "RPUID");
        }
    }
}
