import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DeliveryOrder } from '../../delivery/models/delivery-order';

@Injectable({ providedIn: 'root' })
export class RestaurantOrdersService {
  private readonly baseUrl = 'http://localhost:5029/api/delivery';

  constructor(private http: HttpClient) {}

  getAll(): Observable<DeliveryOrder[]> {
    return this.http.get<DeliveryOrder[]>(this.baseUrl);
  }

  advanceStatus(id: string): Observable<DeliveryOrder> {
    return this.http.post<DeliveryOrder>(`${this.baseUrl}/${id}/advance-status`, {});
  }

  cancel(id: string): Observable<DeliveryOrder> {
    return this.http.post<DeliveryOrder>(`${this.baseUrl}/${id}/cancel`, {});
  }
}
