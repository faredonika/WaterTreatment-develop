namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeparationOfConcernsFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BuildingSystem", "SystemType_Id", "dbo.SystemType");
            DropIndex("dbo.BuildingSystem", new[] { "SystemType_Id" });
            AlterColumn("dbo.BuildingSystem", "SystemType_Id", c => c.Int());
            AlterColumn("dbo.BuildingSystem", "Location", c => c.String());
            CreateIndex("dbo.BuildingSystem", "SystemType_Id");
            AddForeignKey("dbo.BuildingSystem", "SystemType_Id", "dbo.SystemType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BuildingSystem", "SystemType_Id", "dbo.SystemType");
            DropIndex("dbo.BuildingSystem", new[] { "SystemType_Id" });
            AlterColumn("dbo.BuildingSystem", "Location", c => c.String(nullable: false, maxLength: 1024));
            AlterColumn("dbo.BuildingSystem", "SystemType_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.BuildingSystem", "SystemType_Id");
            AddForeignKey("dbo.BuildingSystem", "SystemType_Id", "dbo.SystemType", "Id", cascadeDelete: true);
        }
    }
}
