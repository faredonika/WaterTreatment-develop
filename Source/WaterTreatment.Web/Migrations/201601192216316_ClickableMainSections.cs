namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClickableMainSections : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MainSection", "Clickable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MainSection", "Clickable");
        }
    }
}
