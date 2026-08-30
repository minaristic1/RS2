import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DeliveryOrder } from '../../delivery/models/delivery-order';

@Injectable({ providedIn: 'root' })
export class CourierService {
  private readonly baseUrl = 'http://localhost:5029/api/delivery';

  constructor(private http: HttpClient) {}

  getAll(): Observable<DeliveryOrder[]> {
    return this.http.get<DeliveryOrder[]>(this.baseUrl);
  }

  assignCourier(id: string, courierId: string): Observable<DeliveryOrder> {
    return this.http.post<DeliveryOrder>(`${this.baseUrl}/${id}/assign-courier?courierId=${courierId}`, {});
  }

  advanceStatus(id: string): Observable<DeliveryOrder> {
    return this.http.post<DeliveryOrder>(`${this.baseUrl}/${id}/advance-status`, {});
  }
}
