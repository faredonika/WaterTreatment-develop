namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CopyMinAndMaxValuesToNewDecimalColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParameterBound", "MinValueNew", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ParameterBound", "MaxValueNew", c => c.Decimal(precision: 18, scale: 2));
            // Clean up the messed up data copy it to the new column
            Sql(
            @"
            UPDATE dbo.ParameterBound
            SET MinValue = NULL
            WHERE MinValue = 'null' OR MinValue = ''


            UPDATE dbo.ParameterBound
            SET MaxValue = NULL
            WHERE MaxValue = 'null' OR MaxValue = ''

            UPDATE dbo.ParameterBound
            SET MinValueNew = CAST(MinValue AS DECIMAL(18, 2))

            UPDATE dbo.ParameterBound
            SET MaxValueNew = CAST(MaxValue AS DECIMAL(18, 2))");
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParameterBound", "MaxValueNew");
            DropColumn("dbo.ParameterBound", "MinValueNew");
        }
    }
}
