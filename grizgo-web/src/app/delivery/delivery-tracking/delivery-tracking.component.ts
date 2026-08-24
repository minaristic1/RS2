import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DeliveryService } from '../services/delivery.service';
import { DeliveryOrder } from '../models/delivery-order';

@Component({
  selector: 'app-delivery-tracking',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './delivery-tracking.component.html',
  styleUrl: './delivery-tracking.component.css'
})
export class DeliveryTrackingComponent {
  orderId = '';
  delivery = signal<DeliveryOrder | null>(null);
  notFound = signal(false);
  loading = signal(false);

  constructor(private deliveryService: DeliveryService) {}

  search() {
    this.loading.set(true);
    this.notFound.set(false);
    this.delivery.set(null);

    this.deliveryService.getByOrderId(this.orderId).subscribe({
      next: (result) => {
        this.delivery.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.notFound.set(true);
        this.loading.set(false);
      }
    });
  }

  statusLabel(status: number): string {
    const labels = ['Kreirana', 'Potvrđena', 'U pripremi', 'Na putu', 'Dostavljena', 'Otkazana'];
    return labels[status] ?? 'Nepoznato';
  }
}
