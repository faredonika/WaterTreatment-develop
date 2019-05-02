namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LandingActionHoverImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LandingAction", "HoverImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LandingAction", "HoverImage");
        }
    }
}
