import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartService } from '../services/cart.service';
import { Cart } from '../models/cart';

@Component({
  selector: 'app-cart-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart-view.component.html',
  styleUrl: './cart-view.component.css'
})
export class CartViewComponent implements OnInit {
  cart = signal<Cart | null>(null);
  loading = signal(true);
  error = signal(false);
  actionError = signal(false);

  constructor(private cartService: CartService) {}

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
    this.actionError.set(false);
    this.cartService.checkout().subscribe({
      next: () => this.loadCart(),
      error: () => this.actionError.set(true)
    });
  }
}
