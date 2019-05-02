namespace WaterTreatment.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InviteCode : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.BuildingReport", name: "Buiding_Id", newName: "Building_Id");
            RenameIndex(table: "dbo.BuildingReport", name: "IX_Buiding_Id", newName: "IX_Building_Id");
            AddColumn("dbo.User", "InviteCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "InviteCode");
            RenameIndex(table: "dbo.BuildingReport", name: "IX_Building_Id", newName: "IX_Buiding_Id");
            RenameColumn(table: "dbo.BuildingReport", name: "Building_Id", newName: "Buiding_Id");
        }
    }
}
