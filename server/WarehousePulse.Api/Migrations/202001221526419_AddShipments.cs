namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShipments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShipmentLine",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShipmentId = c.Int(nullable: false),
                        InventoryItemId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InventoryItem", t => t.InventoryItemId)
                .ForeignKey("dbo.Shipment", t => t.ShipmentId, cascadeDelete: true)
                .Index(t => t.ShipmentId)
                .Index(t => t.InventoryItemId);
            
            CreateTable(
                "dbo.Shipment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reference = c.String(nullable: false, maxLength: 20),
                        Direction = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Carrier = c.String(maxLength: 50),
                        TrackingNumber = c.String(maxLength: 64),
                        Destination = c.String(maxLength: 500),
                        ExpectedAt = c.DateTime(),
                        ShippedAt = c.DateTime(),
                        DeliveredAt = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShipmentLine", "ShipmentId", "dbo.Shipment");
            DropForeignKey("dbo.ShipmentLine", "InventoryItemId", "dbo.InventoryItem");
            DropIndex("dbo.ShipmentLine", new[] { "InventoryItemId" });
            DropIndex("dbo.ShipmentLine", new[] { "ShipmentId" });
            DropTable("dbo.Shipment");
            DropTable("dbo.ShipmentLine");
        }
    }
}
