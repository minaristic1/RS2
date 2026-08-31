import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';
import { RestaurantService } from '../../restaurants/services/restaurant.service';
import { Restaurant } from '../../restaurants/models/restaurant';

const ROLES = [
  { value: 'RestaurantOwner', label: 'Vlasnik restorana' },
  { value: 'RestaurantEmployee', label: 'Zaposleni u restoranu' }
];

@Component({
  selector: 'app-create-staff',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './create-staff.component.html',
  styleUrl: './create-staff.component.css'
})
export class CreateStaffComponent implements OnInit {
  roles = ROLES;
  restaurants = signal<Restaurant[]>([]);
  saving = signal(false);
  error = signal(false);
  success = signal(false);

  form = {
    email: '',
    password: '',
    fullName: '',
    role: 'RestaurantOwner',
    restaurantId: ''
  };

  constructor(
    private authService: AuthService,
    private restaurantService: RestaurantService
  ) {}

  ngOnInit(): void {
    this.restaurantService.getAll().subscribe({
      next: (result) => this.restaurants.set(result)
    });
  }

  submit(): void {
    this.saving.set(true);
    this.error.set(false);
    this.success.set(false);

    this.authService.createStaff(this.form).subscribe({
      next: () => {
        this.saving.set(false);
        this.success.set(true);
      },
      error: () => {
        this.saving.set(false);
        this.error.set(true);
      }
    });
  }
}
