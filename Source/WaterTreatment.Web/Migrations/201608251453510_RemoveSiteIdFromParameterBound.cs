namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSiteIdFromParameterBound : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ParameterBound", "Site_Id", "dbo.Site");
            DropIndex("dbo.ParameterBound", new[] { "Site_Id" });
            DropColumn("dbo.ParameterBound", "Site_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ParameterBound", "Site_Id", c => c.Int());
            CreateIndex("dbo.ParameterBound", "Site_Id");
            AddForeignKey("dbo.ParameterBound", "Site_Id", "dbo.Site", "Id");
        }
    }
}
