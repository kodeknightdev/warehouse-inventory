namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSkuUniqueIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.InventoryItem", "Sku", unique: true, name: "IX_InventoryItem_Sku");
        }
        
        public override void Down()
        {
            DropIndex("dbo.InventoryItem", "IX_InventoryItem_Sku");
        }
    }
}
