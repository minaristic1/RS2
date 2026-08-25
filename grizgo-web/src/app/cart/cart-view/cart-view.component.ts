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

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading.set(true);
    this.cartService.getCart().subscribe({
      next: (result) => {
        this.cart.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  increaseQuantity(productId: string, currentQuantity: number): void {
    this.cartService.updateItemQuantity(productId, currentQuantity + 1).subscribe({
      next: (result) => this.cart.set(result)
    });
  }

  decreaseQuantity(productId: string, currentQuantity: number): void {
    if (currentQuantity <= 1) {
      this.removeItem(productId);
      return;
    }
    this.cartService.updateItemQuantity(productId, currentQuantity - 1).subscribe({
      next: (result) => this.cart.set(result)
    });
  }

  removeItem(productId: string): void {
    this.cartService.removeItem(productId).subscribe({
      next: (result) => this.cart.set(result)
    });
  }

  checkout(): void {
    this.cartService.checkout().subscribe({
      next: () => this.loadCart()
    });
  }
}
