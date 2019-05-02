namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "IsActive");
        }
    }
}
