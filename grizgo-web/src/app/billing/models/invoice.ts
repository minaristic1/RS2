export interface InvoiceItem {
  productId: string;
  name: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

export interface Payment {
  id: string;
  amount: number;
  currency: string;
  method: string;
  status: string;
  provider: string;
  transactionReference: string;
  processedAt: string;
}

export interface Invoice {
  id: string;
  orderId: string;
  customerId: string;
  currency: string;
  totalAmount: number;
  status: string;
  createdAt: string;
  paidAt: string | null;
  items: InvoiceItem[];
  payments: Payment[];
}

export interface PayInvoiceRequest {
  method: number;
  provider: string;
  transactionReference: string;
}
