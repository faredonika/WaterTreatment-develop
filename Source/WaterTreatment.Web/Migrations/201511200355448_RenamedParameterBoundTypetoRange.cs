namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedParameterBoundTypetoRange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParameterBound", "Range", c => c.String());
            DropColumn("dbo.ParameterBound", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ParameterBound", "Type", c => c.String());
            DropColumn("dbo.ParameterBound", "Range");
        }
    }
}
