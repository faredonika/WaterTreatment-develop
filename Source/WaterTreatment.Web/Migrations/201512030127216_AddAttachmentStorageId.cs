namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentStorageId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachment", "StorageId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attachment", "StorageId");
        }
    }
}
