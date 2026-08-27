import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Restaurant } from '../models/restaurant';
import { RestaurantMenuList } from '../models/menu';

@Injectable({ providedIn: 'root' })
export class RestaurantService {
  private readonly baseUrl = 'http://localhost:5029/api/restaurants';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Restaurant[]> {
    return this.http.get<Restaurant[]>(this.baseUrl);
  }

  getById(id: string): Observable<Restaurant> {
    return this.http.get<Restaurant>(`${this.baseUrl}/${id}`);
  }

  getMenu(id: string): Observable<RestaurantMenuList> {
    return this.http.get<RestaurantMenuList>(`${this.baseUrl}/${id}/menu`);
  }
}
