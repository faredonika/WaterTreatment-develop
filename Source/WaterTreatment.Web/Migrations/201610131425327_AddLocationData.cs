namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocationData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        State = c.String(maxLength: 50),
                        International = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.User", "Location_Id", c => c.Int());
            CreateIndex("dbo.User", "Location_Id");
            AddForeignKey("dbo.User", "Location_Id", "dbo.Location", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User", "Location_Id", "dbo.Location");
            DropIndex("dbo.User", new[] { "Location_Id" });
            DropColumn("dbo.User", "Location_Id");
            DropTable("dbo.Location");
        }
    }
}
