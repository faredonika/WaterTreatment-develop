namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropOldMinAndMaxColumns : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ParameterBound", "MinValue");
            DropColumn("dbo.ParameterBound", "MaxValue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ParameterBound", "MaxValue", c => c.String());
            AddColumn("dbo.ParameterBound", "MinValue", c => c.String());
            Sql(
            @"
            UPDATE dbo.ParameterBound
            SET MinValue = CAST(MinValueNew AS NVARCHAR(MAX))

            UPDATE dbo.ParameterBound
            SET MaxValue = CAST(MaxValueNew AS NVARCHAR(MAX))");
        }
    }
}
