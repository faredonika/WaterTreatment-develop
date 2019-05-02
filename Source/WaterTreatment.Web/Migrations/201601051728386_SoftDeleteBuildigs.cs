namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SoftDeleteBuildigs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Building", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Building", "IsActive");
        }
    }
}
