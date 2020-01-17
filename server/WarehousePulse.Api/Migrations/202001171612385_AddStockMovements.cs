namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStockMovements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockMovement",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InventoryItemId = c.Int(nullable: false),
                        Delta = c.Int(nullable: false),
                        QuantityAfter = c.Int(nullable: false),
                        Reason = c.String(maxLength: 200),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InventoryItem", t => t.InventoryItemId, cascadeDelete: true)
                .Index(t => t.InventoryItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockMovement", "InventoryItemId", "dbo.InventoryItem");
            DropIndex("dbo.StockMovement", new[] { "InventoryItemId" });
            DropTable("dbo.StockMovement");
        }
    }
}
