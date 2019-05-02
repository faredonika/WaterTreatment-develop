namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissingChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LandingAction", "Name", c => c.String());
            AlterColumn("dbo.MainSection", "Name", c => c.String());
            AlterColumn("dbo.SubSection", "Name", c => c.String());
            AlterColumn("dbo.Parameter", "Name", c => c.String());
            AlterColumn("dbo.ParameterType", "Name", c => c.String());
            AlterColumn("dbo.Site", "Name", c => c.String());
            AlterColumn("dbo.Building", "Name", c => c.String());
            AlterColumn("dbo.BuildingSystem", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BuildingSystem", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Building", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Site", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.ParameterType", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Parameter", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.SubSection", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.MainSection", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.LandingAction", "Name", c => c.String(nullable: false));
        }
    }
}
