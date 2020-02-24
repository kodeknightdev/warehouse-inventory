export type ShipmentStatus = 'Pending' | 'Picking' | 'Packed' | 'InTransit' | 'Delivered' | 'Cancelled';
export type ShipmentDirection = 'Outbound' | 'Inbound';

export interface Shipment {
  id: number;
  reference: string;
  direction: ShipmentDirection;
  status: ShipmentStatus;
  carrier?: string;
  trackingNumber?: string;
  destination?: string;
  expectedAt?: string;
  shippedAt?: string;
  deliveredAt?: string;
  createdAt: string;
  totalUnits: number;
  lines?: ShipmentLine[];
  events?: TrackingEvent[];
}

export interface ShipmentLine {
  id: number;
  inventoryItemId: number;
  sku: string;
  name: string;
  quantity: number;
}

export interface TrackingEvent {
  status: ShipmentStatus;
  location?: string;
  note?: string;
  occurredAt: string;
}

export interface CreateShipmentRequest {
  direction: ShipmentDirection;
  carrier?: string;
  trackingNumber?: string;
  destination: string;
  expectedAt?: string;
  lines: { inventoryItemId: number; quantity: number }[];
}
