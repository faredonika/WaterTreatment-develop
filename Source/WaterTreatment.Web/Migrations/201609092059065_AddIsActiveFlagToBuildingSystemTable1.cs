namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsActiveFlagToBuildingSystemTable1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BuildingSystem", "CreateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BuildingSystem", "CreateDate", c => c.DateTime(nullable: false));
        }
    }
}
