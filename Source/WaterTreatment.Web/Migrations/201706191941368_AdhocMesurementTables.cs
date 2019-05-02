namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdhocMesurementTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdhocMeasurement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        IsApplicable = c.Boolean(nullable: false),
                        BakedOOB = c.Boolean(nullable: false),
                        Comment = c.String(),
                        AdhocParameter_Id = c.Int(),
                        SystemMeasurement_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdhocParameter", t => t.AdhocParameter_Id)
                .ForeignKey("dbo.SystemMeasurement", t => t.SystemMeasurement_Id)
                .Index(t => t.AdhocParameter_Id)
                .Index(t => t.SystemMeasurement_Id);
            
            CreateTable(
                "dbo.AdhocParameter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Unit = c.String(),
                        Source = c.String(),
                        Link = c.String(),
                        Use = c.String(),
                        Frequency = c.String(),
                        Name = c.String(),
                        BuildingSystem_Id = c.Int(),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BuildingSystem", t => t.BuildingSystem_Id)
                .ForeignKey("dbo.ParameterType", t => t.Type_Id)
                .Index(t => t.BuildingSystem_Id)
                .Index(t => t.Type_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdhocMeasurement", "SystemMeasurement_Id", "dbo.SystemMeasurement");
            DropForeignKey("dbo.AdhocParameter", "Type_Id", "dbo.ParameterType");
            DropForeignKey("dbo.AdhocParameter", "BuildingSystem_Id", "dbo.BuildingSystem");
            DropForeignKey("dbo.AdhocMeasurement", "AdhocParameter_Id", "dbo.AdhocParameter");
            DropIndex("dbo.AdhocParameter", new[] { "Type_Id" });
            DropIndex("dbo.AdhocParameter", new[] { "BuildingSystem_Id" });
            DropIndex("dbo.AdhocMeasurement", new[] { "SystemMeasurement_Id" });
            DropIndex("dbo.AdhocMeasurement", new[] { "AdhocParameter_Id" });
            DropTable("dbo.AdhocParameter");
            DropTable("dbo.AdhocMeasurement");
        }
    }
}
