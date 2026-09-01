import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CartService } from '../services/cart.service';
import { Cart } from '../models/cart';

@Component({
  selector: 'app-cart-view',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cart-view.component.html',
  styleUrl: './cart-view.component.css'
})
export class CartViewComponent implements OnInit {
  cart = signal<Cart | null>(null);
  loading = signal(true);
  error = signal(false);
  actionError = signal(false);
  deliveryAddress = '';
  addressMissing = signal(false);

  constructor(private cartService: CartService, private router: Router) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading.set(true);
    this.error.set(false);
    this.cartService.getCart().subscribe({
      next: (result) => {
        this.cart.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set(true);
      }
    });
  }

  increaseQuantity(productId: string, currentQuantity: number): void {
    this.actionError.set(false);
    this.cartService.updateItemQuantity(productId, currentQuantity + 1).subscribe({
      next: (result) => this.cart.set(result),
      error: () => this.actionError.set(true)
    });
  }

  decreaseQuantity(productId: string, currentQuantity: number): void {
    if (currentQuantity <= 1) {
      this.removeItem(productId);
      return;
    }
    this.actionError.set(false);
    this.cartService.updateItemQuantity(productId, currentQuantity - 1).subscribe({
      next: (result) => this.cart.set(result),
      error: () => this.actionError.set(true)
    });
  }

  removeItem(productId: string): void {
    this.actionError.set(false);
    this.cartService.removeItem(productId).subscribe({
      next: (result) => this.cart.set(result),
      error: () => this.actionError.set(true)
    });
  }

  checkout(): void {
    if (!this.deliveryAddress.trim()) {
      this.addressMissing.set(true);
      return;
    }

    this.addressMissing.set(false);
    this.actionError.set(false);
    this.cartService.checkout(this.deliveryAddress).subscribe({
      next: () => {
        this.deliveryAddress = '';
        this.router.navigate(['/payment']);
      },
      error: () => this.actionError.set(true)
    });
  }
}
