export interface OrderItem {
  id: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  deliveryOrderId: string;
}

export interface DeliveryOrder {
  id: string;
  orderId: string;
  customerName: string;
  customerPhone: string;
  restaurantId: string;
  restaurantName: string;
  pickupAddress: string;
  deliveryAddress: string;
  totalPrice: number;
  status: number;
  createdAt: string;
  deliveredAt: string | null;
  cancelledAt: string | null;
  courierId: string | null;
  estimatedDeliveryTime: string | null;
  items: OrderItem[];
}
