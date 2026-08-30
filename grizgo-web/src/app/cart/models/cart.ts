export interface CartItem {
  productId: string;
  restaurantId: string;
  productName: string;
  price: number;
  quantity: number;
  totalPrice: number;
}

export interface Cart {
  userId: string;
  items: CartItem[];
  totalPrice: number;
}
