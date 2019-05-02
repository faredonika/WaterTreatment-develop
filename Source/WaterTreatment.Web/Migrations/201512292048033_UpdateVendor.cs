namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateVendor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vendor", "City", c => c.String());
            AddColumn("dbo.Vendor", "ZipCode", c => c.String());
            AddColumn("dbo.Vendor", "PointOfContact", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vendor", "PointOfContact");
            DropColumn("dbo.Vendor", "ZipCode");
            DropColumn("dbo.Vendor", "City");
        }
    }
}
