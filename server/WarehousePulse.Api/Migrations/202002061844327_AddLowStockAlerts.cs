namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLowStockAlerts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LowStockAlert",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InventoryItemId = c.Int(nullable: false),
                        QuantityAtAlert = c.Int(nullable: false),
                        ReorderLevel = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        AcknowledgedAt = c.DateTime(),
                        ResolvedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InventoryItem", t => t.InventoryItemId, cascadeDelete: true)
                .Index(t => t.InventoryItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LowStockAlert", "InventoryItemId", "dbo.InventoryItem");
            DropIndex("dbo.LowStockAlert", new[] { "InventoryItemId" });
            DropTable("dbo.LowStockAlert");
        }
    }
}
