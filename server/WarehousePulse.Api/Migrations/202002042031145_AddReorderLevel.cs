namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReorderLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryItem", "ReorderLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryItem", "ReorderLevel");
        }
    }
}
