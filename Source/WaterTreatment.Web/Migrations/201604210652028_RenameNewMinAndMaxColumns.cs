namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameNewMinAndMaxColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParameterBound", "MinValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ParameterBound", "MaxValue", c => c.Decimal(precision: 18, scale: 2));
            Sql(
            @"
            UPDATE dbo.ParameterBound
            SET MinValue = MinValueNew

            UPDATE dbo.ParameterBound
            SET MaxValue = MaxValueNew");
            DropColumn("dbo.ParameterBound", "MinValueNew");
            DropColumn("dbo.ParameterBound", "MaxValueNew");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ParameterBound", "MaxValueNew", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ParameterBound", "MinValueNew", c => c.Decimal(precision: 18, scale: 2));
            Sql(
            @"
            UPDATE dbo.ParameterBound
            SET MinValueNew = MinValue

            UPDATE dbo.ParameterBound
            SET MaxValueNew = MaxValue");
            DropColumn("dbo.ParameterBound", "MaxValue");
            DropColumn("dbo.ParameterBound", "MinValue");
        }
    }
}
