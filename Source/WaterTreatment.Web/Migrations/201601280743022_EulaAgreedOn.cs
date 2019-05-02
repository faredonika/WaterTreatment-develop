namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EulaAgreedOn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "EulaAgreedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "EulaAgreedOn");
        }
    }
}
