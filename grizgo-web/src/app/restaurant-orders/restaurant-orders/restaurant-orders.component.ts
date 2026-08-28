import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RestaurantOrdersService } from '../services/restaurant-orders.service';
import { RestaurantService } from '../../restaurants/services/restaurant.service';
import { DeliveryOrder } from '../../delivery/models/delivery-order';
import { Restaurant } from '../../restaurants/models/restaurant';

const CREATED = 0;
const CONFIRMED = 1;
const PREPARING = 2;

const STORAGE_KEY = 'grizgo-selected-restaurant-id';

@Component({
  selector: 'app-restaurant-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './restaurant-orders.component.html',
  styleUrl: './restaurant-orders.component.css'
})
export class RestaurantOrdersComponent implements OnInit {
  restaurants = signal<Restaurant[]>([]);
  deliveries = signal<DeliveryOrder[]>([]);
  loading = signal(true);
  selectedRestaurantId = signal<string>(localStorage.getItem(STORAGE_KEY) ?? '');

  incomingOrders = computed(() =>
    this.deliveries().filter(
      (d) =>
        d.restaurantId === this.selectedRestaurantId() &&
        (d.status === CREATED || d.status === CONFIRMED || d.status === PREPARING)
    )
  );

  constructor(
    private restaurantOrdersService: RestaurantOrdersService,
    private restaurantService: RestaurantService
  ) {}

  ngOnInit(): void {
    this.restaurantService.getAll().subscribe({
      next: (result) => this.restaurants.set(result)
    });

    this.loadDeliveries();
  }

  loadDeliveries(): void {
    this.loading.set(true);
    this.restaurantOrdersService.getAll().subscribe({
      next: (result) => {
        this.deliveries.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  onRestaurantChange(id: string): void {
    this.selectedRestaurantId.set(id);
    localStorage.setItem(STORAGE_KEY, id);
  }

  advanceStatus(id: string): void {
    this.restaurantOrdersService.advanceStatus(id).subscribe({
      next: () => this.loadDeliveries()
    });
  }

  cancel(id: string): void {
    this.restaurantOrdersService.cancel(id).subscribe({
      next: () => this.loadDeliveries()
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
