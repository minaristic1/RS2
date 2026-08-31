import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { RestaurantOrdersService } from '../services/restaurant-orders.service';
import { RestaurantService } from '../../restaurants/services/restaurant.service';
import { AuthService } from '../../auth/services/auth.service';
import { DeliveryOrder } from '../../delivery/models/delivery-order';
import { Restaurant } from '../../restaurants/models/restaurant';

const CREATED = 0;
const CONFIRMED = 1;
const PREPARING = 2;

const STORAGE_KEY = 'grizgo-selected-restaurant-id';

@Component({
  selector: 'app-restaurant-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './restaurant-orders.component.html',
  styleUrl: './restaurant-orders.component.css'
})
export class RestaurantOrdersComponent implements OnInit {
  restaurants = signal<Restaurant[]>([]);
  deliveries = signal<DeliveryOrder[]>([]);
  loading = signal(true);
  error = signal(false);
  actionError = signal(false);
  selectedRestaurantId = signal<string>('');
  lockedToOwnRestaurant = false;

  incomingOrders = computed(() =>
    this.deliveries().filter(
      (d) =>
        d.restaurantId === this.selectedRestaurantId() &&
        (d.status === CREATED || d.status === CONFIRMED || d.status === PREPARING)
    )
  );

  selectedRestaurantName = computed(
    () => this.restaurants().find((r) => r.id === this.selectedRestaurantId())?.nameSr
  );

  constructor(
    private restaurantOrdersService: RestaurantOrdersService,
    private restaurantService: RestaurantService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const ownRestaurantId = this.authService.currentUser()?.restaurantId;

    if (ownRestaurantId) {
      this.lockedToOwnRestaurant = true;
      this.selectedRestaurantId.set(ownRestaurantId);
    } else {
      this.selectedRestaurantId.set(localStorage.getItem(STORAGE_KEY) ?? '');
    }

    this.restaurantService.getAll().subscribe({
      next: (result) => this.restaurants.set(result),
      error: () => this.error.set(true)
    });

    this.loadDeliveries();
  }

  loadDeliveries(): void {
    this.loading.set(true);
    this.error.set(false);
    this.restaurantOrdersService.getAll().subscribe({
      next: (result) => {
        this.deliveries.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set(true);
      }
    });
  }

  onRestaurantChange(id: string): void {
    this.selectedRestaurantId.set(id);
    localStorage.setItem(STORAGE_KEY, id);
  }

  advanceStatus(id: string): void {
    this.actionError.set(false);
    this.restaurantOrdersService.advanceStatus(id).subscribe({
      next: () => this.loadDeliveries(),
      error: () => this.actionError.set(true)
    });
  }

  cancel(id: string): void {
    this.actionError.set(false);
    this.restaurantOrdersService.cancel(id).subscribe({
      next: () => this.loadDeliveries(),
      error: () => this.actionError.set(true)
    });
  }

  actionLabel(status: number): string {
    if (status === CREATED) return 'Prihvati porudžbinu';
    if (status === CONFIRMED) return 'Počni pripremu';
    return 'Gotova je';
  }

  statusLabel(status: number): string {
    const labels = ['Kreirana', 'Potvrđena', 'U pripremi', 'Spremna za preuzimanje', 'Na putu', 'Dostavljena', 'Otkazana'];
    return labels[status] ?? 'Nepoznato';
  }

  formatTime(dateStr: string): string {
    return new Date(dateStr).toLocaleString('sr-RS', { dateStyle: 'short', timeStyle: 'short' });
  }
}
