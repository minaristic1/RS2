import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cart } from '../models/cart';
import { SessionService } from '../../shared/session/session.service';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly baseUrl = 'http://localhost:5029/api/carts';

  constructor(private http: HttpClient, private session: SessionService) {}

  getCart(): Observable<Cart> {
    return this.http.get<Cart>(`${this.baseUrl}/${this.session.getUserId()}`);
  }

  addItem(productId: string, quantity: number): Observable<Cart> {
    return this.http.post<Cart>(`${this.baseUrl}/${this.session.getUserId()}/items`, { productId, quantity });
  }

  updateItemQuantity(productId: string, quantity: number): Observable<Cart> {
    return this.http.put<Cart>(`${this.baseUrl}/${this.session.getUserId()}/items/${productId}`, { quantity });
  }

  removeItem(productId: string): Observable<Cart> {
    return this.http.delete<Cart>(`${this.baseUrl}/${this.session.getUserId()}/items/${productId}`);
  }

  checkout(deliveryAddress: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${this.session.getUserId()}/checkout`, { deliveryAddress });
  }
}
