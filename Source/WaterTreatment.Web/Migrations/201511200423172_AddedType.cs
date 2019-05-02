namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParameterBound", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParameterBound", "Type");
        }
    }
}
