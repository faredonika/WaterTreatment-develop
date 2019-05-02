namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordResetCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "ResetCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "ResetCode");
        }
    }
}
