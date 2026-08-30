import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Restaurant } from '../models/restaurant';
import {
  RestaurantMenuList,
  Menu,
  MenuCategory,
  MenuItemSummary,
  CreateMenuRequest,
  CreateMenuCategoryRequest,
  CreateMenuItemRequest
} from '../models/menu';

@Injectable({ providedIn: 'root' })
export class RestaurantService {
  private readonly baseUrl = 'http://localhost:5029/api/restaurants';
  private readonly menuItemsUrl = 'http://localhost:5029/api/menu-items';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Restaurant[]> {
    return this.http.get<Restaurant[]>(this.baseUrl);
  }

  search(term: string): Observable<Restaurant[]> {
    return this.http.get<Restaurant[]>(`${this.baseUrl}/search`, { params: { term } });
  }

  create(restaurant: Partial<Restaurant>): Observable<Restaurant> {
    return this.http.post<Restaurant>(this.baseUrl, restaurant);
  }

  update(id: string, restaurant: Partial<Restaurant>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, restaurant);
  }

  getById(id: string): Observable<Restaurant> {
    return this.http.get<Restaurant>(`${this.baseUrl}/${id}`);
  }

  getMenu(id: string): Observable<RestaurantMenuList> {
    return this.http.get<RestaurantMenuList>(`${this.baseUrl}/${id}/menu`);
  }

  createMenu(restaurantId: string, request: CreateMenuRequest): Observable<Menu> {
    return this.http.post<Menu>(`${this.baseUrl}/${restaurantId}/menus`, request);
  }

  createCategory(restaurantId: string, menuId: string, request: CreateMenuCategoryRequest): Observable<MenuCategory> {
    return this.http.post<MenuCategory>(`${this.baseUrl}/${restaurantId}/menus/${menuId}/categories`, request);
  }

  createItem(
    restaurantId: string,
    menuId: string,
    categoryId: string,
    request: CreateMenuItemRequest
  ): Observable<MenuItemSummary> {
    return this.http.post<MenuItemSummary>(
      `${this.baseUrl}/${restaurantId}/menus/${menuId}/categories/${categoryId}/items`,
      request
    );
  }

  updateItem(itemId: string, request: CreateMenuItemRequest): Observable<void> {
    return this.http.put<void>(`${this.menuItemsUrl}/${itemId}`, request);
  }

  deleteItem(itemId: string): Observable<void> {
    return this.http.delete<void>(`${this.menuItemsUrl}/${itemId}`);
  }
}
