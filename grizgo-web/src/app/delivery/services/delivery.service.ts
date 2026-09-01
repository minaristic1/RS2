import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DeliveryOrder } from '../models/delivery-order';

@Injectable({ providedIn: 'root' })
export class DeliveryService {
  private readonly baseUrl = 'http://localhost:5029/api/delivery';

  constructor(private http: HttpClient) {}

  getByOrderId(orderId: string): Observable<DeliveryOrder> {
    return this.http.get<DeliveryOrder>(`${this.baseUrl}/by-order/${orderId}`);
  }

  cancel(id: string): Observable<DeliveryOrder> {
    return this.http.post<DeliveryOrder>(`${this.baseUrl}/${id}/cancel`, {});
  }
}
