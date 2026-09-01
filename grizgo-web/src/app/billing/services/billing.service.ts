import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice, Payment, PayInvoiceRequest } from '../models/invoice';

@Injectable({ providedIn: 'root' })
export class BillingService {
  private readonly baseUrl = 'http://localhost:5029/api/invoices';

  constructor(private http: HttpClient) {}

  getCustomerInvoices(customerId: string): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(`${this.baseUrl}/customer/${customerId}`);
  }

  payInvoice(invoiceId: string, request: PayInvoiceRequest): Observable<Payment> {
    return this.http.post<Payment>(`${this.baseUrl}/${invoiceId}/payments`, request);
  }
}
