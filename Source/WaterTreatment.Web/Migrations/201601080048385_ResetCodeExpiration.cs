namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResetCodeExpiration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "ResetCodeExpiration", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "ResetCodeExpiration");
        }
    }
}
