export interface InventoryItem {
  id: number;
  sku: string;
  name: string;
  description?: string;
  category?: string;
  quantityOnHand: number;
  reorderLevel: number;
  isLowStock: boolean;
  unitOfMeasure: string;
  unitCost: number;
  locationId?: number;
  locationCode?: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
