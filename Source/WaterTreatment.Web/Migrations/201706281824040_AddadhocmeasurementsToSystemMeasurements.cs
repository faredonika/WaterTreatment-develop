namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddadhocmeasurementsToSystemMeasurements : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParameterBound", "AdhocParameter_Id", c => c.Int());
            CreateIndex("dbo.ParameterBound", "AdhocParameter_Id");
            AddForeignKey("dbo.ParameterBound", "AdhocParameter_Id", "dbo.AdhocParameter", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParameterBound", "AdhocParameter_Id", "dbo.AdhocParameter");
            DropIndex("dbo.ParameterBound", new[] { "AdhocParameter_Id" });
            DropColumn("dbo.ParameterBound", "AdhocParameter_Id");
        }
    }
}
