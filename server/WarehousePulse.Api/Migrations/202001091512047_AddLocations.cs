namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Zone = c.String(maxLength: 50),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.InventoryItem", "LocationId", c => c.Int());
            CreateIndex("dbo.InventoryItem", "LocationId");
            AddForeignKey("dbo.InventoryItem", "LocationId", "dbo.Location", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryItem", "LocationId", "dbo.Location");
            DropIndex("dbo.InventoryItem", new[] { "LocationId" });
            DropColumn("dbo.InventoryItem", "LocationId");
            DropTable("dbo.Location");
        }
    }
}
