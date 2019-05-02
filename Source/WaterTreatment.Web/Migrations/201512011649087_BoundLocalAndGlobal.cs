namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoundLocalAndGlobal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParameterBound", "Site_Id", c => c.Int());
            CreateIndex("dbo.ParameterBound", "Site_Id");
            AddForeignKey("dbo.ParameterBound", "Site_Id", "dbo.Site", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParameterBound", "Site_Id", "dbo.Site");
            DropIndex("dbo.ParameterBound", new[] { "Site_Id" });
            DropColumn("dbo.ParameterBound", "Site_Id");
        }
    }
}
