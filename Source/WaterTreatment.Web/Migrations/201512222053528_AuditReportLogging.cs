namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuditReportLogging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditEntity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModifiedOn = c.DateTime(nullable: false),
                        EventType = c.Int(nullable: false),
                        EventInstance = c.Guid(nullable: false),
                        TableName = c.String(),
                        ColumnName = c.String(),
                        EntityId = c.Int(nullable: false),
                        OriginalValue = c.String(),
                        CurrentValue = c.String(),
                        Name = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        ModifiedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.ModifiedBy_Id)
                .Index(t => t.ModifiedBy_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuditEntity", "ModifiedBy_Id", "dbo.User");
            DropIndex("dbo.AuditEntity", new[] { "ModifiedBy_Id" });
            DropTable("dbo.AuditEntity");
        }
    }
}
