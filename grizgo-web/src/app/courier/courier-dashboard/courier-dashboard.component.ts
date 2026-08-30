import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourierService } from '../services/courier.service';
import { DeliveryOrder } from '../../delivery/models/delivery-order';
import { SessionService } from '../../shared/session/session.service';

const CONFIRMED = 1;
const READY_FOR_PICKUP = 3;
const OUT_FOR_DELIVERY = 4;
const DELIVERED = 5;
const CANCELLED = 6;

const STATUS_LABELS = ['Kreirana', 'Potvrđena', 'U pripremi', 'Spremna za preuzimanje', 'Na putu', 'Dostavljena', 'Otkazana'];
const STATUS_STEPS = ['Kreirana', 'Potvrđena', 'U pripremi', 'Spremna', 'Na putu', 'Dostavljena'];
const STATUS_BADGE_CLASSES = ['bg-secondary', 'bg-info', 'bg-warning text-dark', 'bg-warning text-dark', 'bg-primary', 'bg-success', 'bg-danger'];

@Component({
  selector: 'app-courier-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './courier-dashboard.component.html',
  styleUrl: './courier-dashboard.component.css'
})
export class CourierDashboardComponent implements OnInit {
  deliveries = signal<DeliveryOrder[]>([]);
  loading = signal(true);
  error = signal(false);
  actionError = signal(false);
  courierId: string;
  statusSteps = STATUS_STEPS;

  availableDeliveries = computed(() =>
    this.deliveries().filter((d) => !d.courierId && d.status >= CONFIRMED && d.status <= READY_FOR_PICKUP)
  );

  myDeliveries = computed(() =>
    this.deliveries().filter((d) => d.courierId === this.courierId && d.status !== DELIVERED && d.status !== CANCELLED)
  );

  constructor(
    private courierService: CourierService,
    private session: SessionService
  ) {
    this.courierId = this.session.getUserId();
  }

  ngOnInit(): void {
    this.loadDeliveries();
  }

  loadDeliveries(): void {
    this.loading.set(true);
    this.error.set(false);
    this.courierService.getAll().subscribe({
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

  takeDelivery(id: string): void {
    this.actionError.set(false);
    this.courierService.assignCourier(id, this.courierId).subscribe({
      next: () => this.loadDeliveries(),
      error: () => this.actionError.set(true)
    });
  }

  advanceStatus(id: string): void {
    this.actionError.set(false);
    this.courierService.advanceStatus(id).subscribe({
      next: () => this.loadDeliveries(),
      error: () => this.actionError.set(true)
    });
  }

  actionLabel(status: number): string {
    return status === READY_FOR_PICKUP ? 'Krenuo sam' : 'Dostavljeno';
  }

  canAct(status: number): boolean {
    return status >= READY_FOR_PICKUP;
  }

  statusLabel(status: number): string {
    return STATUS_LABELS[status] ?? 'Nepoznato';
  }

  statusBadgeClass(status: number): string {
    return STATUS_BADGE_CLASSES[status] ?? 'bg-secondary';
  }

  formatTime(dateStr: string): string {
    return new Date(dateStr).toLocaleString('sr-RS', { dateStyle: 'short', timeStyle: 'short' });
  }
}
