export interface DashboardSummary {
  totalSkus: number;
  totalUnits: number;
  stockValue: number;
  lowStockItems: number;
  openAlerts: number;
  shipments: {
    pending: number;
    inTransit: number;
    delivered: number;
  };
  stockByCategory: CategoryStock[];
}

export interface CategoryStock {
  category: string;
  units: number;
  value: number;
}
