import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { RestaurantService } from '../services/restaurant.service';
import { Restaurant } from '../models/restaurant';
import { RestaurantMenuList, MenuItemSummary } from '../models/menu';
import { CartService } from '../../cart/services/cart.service';

@Component({
  selector: 'app-restaurant-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './restaurant-detail.component.html',
  styleUrl: './restaurant-detail.component.css'
})
export class RestaurantDetailComponent implements OnInit {
  restaurant = signal<Restaurant | null>(null);
  loading = signal(true);
  notFound = signal(false);

  menuList = signal<RestaurantMenuList | null>(null);
  menuLoading = signal(true);
  activeMenuIndex = signal(0);

  selectedItem = signal<MenuItemSummary | null>(null);
  selectedQuantity = signal(1);
  addingToCart = signal(false);
  addedConfirmation = signal(false);

  constructor(
    private route: ActivatedRoute,
    private restaurantService: RestaurantService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.notFound.set(true);
      this.loading.set(false);
      this.menuLoading.set(false);
      return;
    }

    this.restaurantService.getById(id).subscribe({
      next: (result) => {
        this.restaurant.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.notFound.set(true);
        this.loading.set(false);
      }
    });

    this.restaurantService.getMenu(id).subscribe({
      next: (result) => {
        this.menuList.set(result);
        this.menuLoading.set(false);
      },
      error: () => {
        this.menuLoading.set(false);
      }
    });
  }

  openItem(item: MenuItemSummary): void {
    this.selectedItem.set(item);
    this.selectedQuantity.set(1);
    this.addedConfirmation.set(false);
  }

  closeItem(): void {
    this.selectedItem.set(null);
  }

  increaseQuantity(): void {
    this.selectedQuantity.update((q) => q + 1);
  }

  decreaseQuantity(): void {
    this.selectedQuantity.update((q) => Math.max(1, q - 1));
  }

  addToCart(): void {
    const item = this.selectedItem();
    if (!item) {
      return;
    }

    this.addingToCart.set(true);
    this.cartService.addItem(item.id, this.selectedQuantity()).subscribe({
      next: () => {
        this.addingToCart.set(false);
        this.addedConfirmation.set(true);
      },
      error: () => {
        this.addingToCart.set(false);
      }
    });
  }
}
