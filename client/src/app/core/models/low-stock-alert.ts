export interface LowStockAlert {
  id: number;
  inventoryItemId: number;
  sku: string;
  name: string;
  currentQuantity: number;
  quantityAtAlert: number;
  reorderLevel: number;
  createdAt: string;
  acknowledgedAt?: string;
}
