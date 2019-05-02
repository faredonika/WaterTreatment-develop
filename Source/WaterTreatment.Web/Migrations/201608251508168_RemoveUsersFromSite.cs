namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUsersFromSite : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User", "Site_Id", "dbo.Site");
            DropIndex("dbo.User", new[] { "Site_Id" });
            DropColumn("dbo.User", "Site_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "Site_Id", c => c.Int());
            CreateIndex("dbo.User", "Site_Id");
            AddForeignKey("dbo.User", "Site_Id", "dbo.Site", "Id");
        }
    }
}
