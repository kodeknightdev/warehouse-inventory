namespace WarehousePulse.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemRowVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryItem", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryItem", "RowVersion");
        }
    }
}
