namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NextDigestField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Site", "NextDigest", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Site", "NextDigest");
        }
    }
}
