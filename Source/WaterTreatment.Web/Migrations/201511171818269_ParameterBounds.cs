namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParameterBounds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ParameterBound",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        MinValue = c.String(),
                        MaxValue = c.String(),
                        IsEnforced = c.Boolean(nullable: false),
                        Parameter_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Parameter", t => t.Parameter_Id)
                .Index(t => t.Parameter_Id);
            
            AlterColumn("dbo.Parameter", "Unit", c => c.String());
            AlterColumn("dbo.Parameter", "Source", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParameterBound", "Parameter_Id", "dbo.Parameter");
            DropIndex("dbo.ParameterBound", new[] { "Parameter_Id" });
            AlterColumn("dbo.Parameter", "Source", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.Parameter", "Unit", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.ParameterBound");
        }
    }
}
