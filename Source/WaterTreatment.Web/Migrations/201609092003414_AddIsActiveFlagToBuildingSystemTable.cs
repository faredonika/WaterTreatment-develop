namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsActiveFlagToBuildingSystemTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BuildingSystem", "IsActive", c => c.Boolean(nullable: false, defaultValueSql: "1"));
            AddColumn("dbo.BuildingSystem", "CreateDate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("dbo.BuildingSystem", "UpdateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BuildingSystem", "UpdateDate");
            DropColumn("dbo.BuildingSystem", "CreateDate");
            DropColumn("dbo.BuildingSystem", "IsActive");
        }
    }
}
