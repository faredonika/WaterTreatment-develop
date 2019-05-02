namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OnlyLocationIsNecessary : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Site", "Location", c => c.String());
            DropColumn("dbo.BuildingSystem", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BuildingSystem", "Name", c => c.String());
            AlterColumn("dbo.Site", "Location", c => c.String(nullable: false, maxLength: 1024));
        }
    }
}
