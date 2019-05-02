namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ASPNETIdentitytables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User", "Role_Id", "dbo.Role");
            DropIndex("dbo.User", new[] { "Role_Id" });
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.User", "Email", c => c.String(maxLength: 256));
            AddColumn("dbo.User", "EmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "PasswordHash", c => c.String());
            AddColumn("dbo.User", "SecurityStamp", c => c.String());
            AddColumn("dbo.User", "PhoneNumber", c => c.String());
            AddColumn("dbo.User", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.User", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "AccessFailedCount", c => c.Int(nullable: false));
            AlterColumn("dbo.Role", "Name", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.User", "UserName", c => c.String(nullable: false, maxLength: 256));
            CreateIndex("dbo.Role", "Name", unique: true, name: "RoleNameIndex");
            CreateIndex("dbo.User", "UserName", unique: true, name: "UserNameIndex");
            DropColumn("dbo.User", "Role_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "Role_Id", c => c.Int());
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserLogin", "UserId", "dbo.User");
            DropForeignKey("dbo.UserClaim", "UserId", "dbo.User");
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.UserClaim", new[] { "UserId" });
            DropIndex("dbo.User", "UserNameIndex");
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.Role", "RoleNameIndex");
            AlterColumn("dbo.User", "UserName", c => c.String());
            AlterColumn("dbo.Role", "Name", c => c.String());
            DropColumn("dbo.User", "AccessFailedCount");
            DropColumn("dbo.User", "LockoutEnabled");
            DropColumn("dbo.User", "LockoutEndDateUtc");
            DropColumn("dbo.User", "TwoFactorEnabled");
            DropColumn("dbo.User", "PhoneNumberConfirmed");
            DropColumn("dbo.User", "PhoneNumber");
            DropColumn("dbo.User", "SecurityStamp");
            DropColumn("dbo.User", "PasswordHash");
            DropColumn("dbo.User", "EmailConfirmed");
            DropColumn("dbo.User", "Email");
            DropTable("dbo.UserLogin");
            DropTable("dbo.UserClaim");
            DropTable("dbo.UserRole");
            CreateIndex("dbo.User", "Role_Id");
            AddForeignKey("dbo.User", "Role_Id", "dbo.Role", "Id");
        }
    }
}
