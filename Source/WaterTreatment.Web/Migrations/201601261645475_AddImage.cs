namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LandingAction", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LandingAction", "Image");
        }
    }
}
