namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSystemType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Parameter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Unit = c.String(nullable: false, maxLength: 128),
                        Source = c.String(nullable: false, maxLength: 256),
                        Link = c.String(),
                        Name = c.String(),
                        Type_Id = c.Int(),
                        SystemType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ParameterType", t => t.Type_Id)
                .ForeignKey("dbo.SystemType", t => t.SystemType_Id)
                .Index(t => t.Type_Id)
                .Index(t => t.SystemType_Id);
            
            CreateTable(
                "dbo.ParameterType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Parameter", "SystemType_Id", "dbo.SystemType");
            DropForeignKey("dbo.Parameter", "Type_Id", "dbo.ParameterType");
            DropIndex("dbo.Parameter", new[] { "SystemType_Id" });
            DropIndex("dbo.Parameter", new[] { "Type_Id" });
            DropTable("dbo.ParameterType");
            DropTable("dbo.Parameter");
            DropTable("dbo.SystemType");
        }
    }
}
