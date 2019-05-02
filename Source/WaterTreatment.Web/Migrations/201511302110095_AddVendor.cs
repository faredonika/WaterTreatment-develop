namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVendor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vendor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        State = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.User", "Vendor_Id", c => c.Int());
            CreateIndex("dbo.User", "Vendor_Id");
            AddForeignKey("dbo.User", "Vendor_Id", "dbo.Vendor", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User", "Vendor_Id", "dbo.Vendor");
            DropIndex("dbo.User", new[] { "Vendor_Id" });
            DropColumn("dbo.User", "Vendor_Id");
            DropTable("dbo.Vendor");
        }
    }
}
