namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequireName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LandingAction", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.MainSection", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.SubSection", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.SystemType", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Parameter", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.ParameterType", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ParameterType", "Name", c => c.String());
            AlterColumn("dbo.Parameter", "Name", c => c.String());
            AlterColumn("dbo.SystemType", "Name", c => c.String());
            AlterColumn("dbo.SubSection", "Name", c => c.String());
            AlterColumn("dbo.MainSection", "Name", c => c.String());
            AlterColumn("dbo.LandingAction", "Name", c => c.String());
        }
    }
}
