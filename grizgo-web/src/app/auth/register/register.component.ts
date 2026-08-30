import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

const ROLES = [
  { value: 'Customer', label: 'Kupac' },
  { value: 'RestaurantOwner', label: 'Vlasnik restorana' },
  { value: 'RestaurantEmployee', label: 'Zaposleni u restoranu' },
  { value: 'Driver', label: 'Dostavljač' },
  { value: 'Admin', label: 'Admin' }
];

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  roles = ROLES;
  saving = signal(false);
  error = signal(false);
  emailTaken = signal(false);
  success = signal(false);

  form = {
    email: '',
    password: '',
    fullName: '',
    role: 'Customer'
  };

  constructor(private authService: AuthService) {}

  submit(): void {
    this.saving.set(true);
    this.error.set(false);
    this.emailTaken.set(false);

    this.authService.register(this.form).subscribe({
      next: () => {
        this.saving.set(false);
        this.success.set(true);
      },
      error: (err) => {
        this.saving.set(false);
        if (err.status === 409) {
          this.emailTaken.set(true);
        } else {
          this.error.set(true);
        }
      }
    });
  }
}
