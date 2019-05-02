namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BuildingSystem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SystemMeasurement", "Building_Id", "dbo.Building");
            DropIndex("dbo.SystemMeasurement", new[] { "Building_Id" });
            AddColumn("dbo.SystemMeasurement", "BuildingSystem_Id", c => c.Int());
            CreateIndex("dbo.SystemMeasurement", "BuildingSystem_Id");
            AddForeignKey("dbo.SystemMeasurement", "BuildingSystem_Id", "dbo.BuildingSystem", "Id");
            DropColumn("dbo.SystemMeasurement", "Building_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SystemMeasurement", "Building_Id", c => c.Int());
            DropForeignKey("dbo.SystemMeasurement", "BuildingSystem_Id", "dbo.BuildingSystem");
            DropIndex("dbo.SystemMeasurement", new[] { "BuildingSystem_Id" });
            DropColumn("dbo.SystemMeasurement", "BuildingSystem_Id");
            CreateIndex("dbo.SystemMeasurement", "Building_Id");
            AddForeignKey("dbo.SystemMeasurement", "Building_Id", "dbo.Building", "Id");
        }
    }
}
