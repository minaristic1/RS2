import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
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
  error = signal(false);
  loading = signal(false);

  cancelling = signal(false);

  constructor(private deliveryService: DeliveryService) {}

  search() {
    this.loading.set(true);
    this.notFound.set(false);
    this.error.set(false);
    this.delivery.set(null);

    this.deliveryService.getByOrderId(this.orderId).subscribe({
      next: (result) => {
        this.delivery.set(result);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === 404) {
          this.notFound.set(true);
        } else {
          this.error.set(true);
        }
        this.loading.set(false);
      }
    });
  }

  statusLabel(status: number): string {
    const labels = ['Kreirana', 'Potvrđena', 'U pripremi', 'Spremna za preuzimanje', 'Na putu', 'Dostavljena', 'Otkazana'];
    return labels[status] ?? 'Nepoznato';
  }

  canCancel(status: number): boolean {
    return status < 5;
  }

  cancel(): void {
    const d = this.delivery();
    if (!d) {
      return;
    }

    this.cancelling.set(true);
    this.deliveryService.cancel(d.id).subscribe({
      next: (result) => {
        this.delivery.set(result);
        this.cancelling.set(false);
      },
      error: () => {
        this.cancelling.set(false);
      }
    });
  }
}
