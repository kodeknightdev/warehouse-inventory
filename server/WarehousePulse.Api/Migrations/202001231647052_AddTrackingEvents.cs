namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrackingEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrackingEvent",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ShipmentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Location = c.String(maxLength: 100),
                        Note = c.String(maxLength: 300),
                        OccurredAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shipment", t => t.ShipmentId, cascadeDelete: true)
                .Index(t => t.ShipmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrackingEvent", "ShipmentId", "dbo.Shipment");
            DropIndex("dbo.TrackingEvent", new[] { "ShipmentId" });
            DropTable("dbo.TrackingEvent");
        }
    }
}
