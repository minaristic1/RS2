import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BillingService } from '../services/billing.service';
import { AuthService } from '../../auth/services/auth.service';
import { Invoice } from '../models/invoice';

const POLL_INTERVAL_MS = 1500;
const MAX_ATTEMPTS = 10;

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.css'
})
export class PaymentComponent implements OnInit, OnDestroy {
  invoice = signal<Invoice | null>(null);
  waiting = signal(true);
  notFound = signal(false);
  paying = signal(false);
  paid = signal(false);
  error = signal(false);

  private attempts = 0;
  private timeoutId: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private billingService: BillingService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.pollForInvoice();
  }

  ngOnDestroy(): void {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
  }

  private pollForInvoice(): void {
    const customerId = this.authService.currentUser()?.id;
    if (!customerId) {
      this.waiting.set(false);
      this.error.set(true);
      return;
    }

    this.billingService.getCustomerInvoices(customerId).subscribe({
      next: (invoices) => {
        const pending = invoices
          .filter((i) => i.status === 'AwaitingPayment')
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

        if (pending.length > 0) {
          this.invoice.set(pending[0]);
          this.waiting.set(false);
          return;
        }

        this.attempts++;
        if (this.attempts >= MAX_ATTEMPTS) {
          this.waiting.set(false);
          this.notFound.set(true);
          return;
        }

        this.timeoutId = setTimeout(() => this.pollForInvoice(), POLL_INTERVAL_MS);
      },
      error: () => {
        this.waiting.set(false);
        this.error.set(true);
      }
    });
  }

  pay(): void {
    const invoice = this.invoice();
    if (!invoice) {
      return;
    }

    this.paying.set(true);
    this.error.set(false);

    this.billingService
      .payInvoice(invoice.id, {
        method: 1,
        provider: 'GrizGo Simulacija',
        transactionReference: crypto.randomUUID()
      })
      .subscribe({
        next: () => {
          this.paying.set(false);
          this.paid.set(true);
        },
        error: () => {
          this.paying.set(false);
          this.error.set(true);
        }
      });
  }
}
