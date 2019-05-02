namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SoftSiteAndSystemDeletes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemType", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Site", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Site", "IsActive");
            DropColumn("dbo.SystemType", "IsActive");
        }
    }
}
