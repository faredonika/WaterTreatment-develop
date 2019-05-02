namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportCreationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Report", "CreationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Report", "CreationDate");
        }
    }
}
