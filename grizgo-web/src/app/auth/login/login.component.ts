import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  form = {
    email: '',
    password: ''
  };

  saving = signal(false);
  error = signal(false);

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  submit(): void {
    this.saving.set(true);
    this.error.set(false);

    this.authService.login(this.form).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/restaurants']);
      },
      error: () => {
        this.saving.set(false);
        this.error.set(true);
      }
    });
  }
}
