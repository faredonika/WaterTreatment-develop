namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BuildingSystemsDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BuildingSystem", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BuildingSystem", "Description");
        }
    }
}
