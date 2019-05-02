namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoundDescriptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParameterBound", "MinDescription", c => c.String());
            AddColumn("dbo.ParameterBound", "MaxDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParameterBound", "MaxDescription");
            DropColumn("dbo.ParameterBound", "MinDescription");
        }
    }
}
