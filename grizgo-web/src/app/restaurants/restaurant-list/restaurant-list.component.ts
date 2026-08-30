import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RestaurantService } from '../services/restaurant.service';
import { Restaurant } from '../models/restaurant';

@Component({
  selector: 'app-restaurant-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './restaurant-list.component.html',
  styleUrl: './restaurant-list.component.css'
})
export class RestaurantListComponent implements OnInit {
  restaurants = signal<Restaurant[]>([]);
  loading = signal(true);
  error = signal(false);
  searchTerm = '';

  constructor(private restaurantService: RestaurantService) {}

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.loading.set(true);
    this.error.set(false);
    this.restaurantService.getAll().subscribe({
      next: (result) => {
        this.restaurants.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set(true);
      }
    });
  }

  search(): void {
    if (!this.searchTerm.trim()) {
      this.loadAll();
      return;
    }

    this.loading.set(true);
    this.error.set(false);
    this.restaurantService.search(this.searchTerm).subscribe({
      next: (result) => {
        this.restaurants.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set(true);
      }
    });
  }
}
