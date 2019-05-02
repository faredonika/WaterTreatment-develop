namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParameterUseField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parameter", "Use", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Parameter", "Use");
        }
    }
}
