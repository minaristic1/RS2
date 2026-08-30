import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RestaurantService } from '../services/restaurant.service';

const CUISINE_TYPES = [
  'Italijanska',
  'Srpska',
  'Kineska',
  'Meksicka',
  'Japanska',
  'FastFood',
  'Rostilj',
  'Zdrava',
  'Vegetarijanska',
  'Deserti'
];

@Component({
  selector: 'app-restaurant-create',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './restaurant-create.component.html',
  styleUrl: './restaurant-create.component.css'
})
export class RestaurantCreateComponent implements OnInit {
  cuisineTypes = CUISINE_TYPES;
  saving = signal(false);
  error = signal(false);
  loading = signal(false);
  editId: string | null = null;

  form = {
    nameSr: '',
    nameEn: '',
    descriptionSr: '',
    descriptionEn: '',
    address: '',
    imageUrl: '',
    cuisineType: 'Srpska',
    isActive: true,
    isFeatured: false
  };

  constructor(
    private restaurantService: RestaurantService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.editId = this.route.snapshot.paramMap.get('id');

    if (this.editId) {
      this.loading.set(true);
      this.restaurantService.getById(this.editId).subscribe({
        next: (result) => {
          this.form = {
            nameSr: result.nameSr,
            nameEn: result.nameEn,
            descriptionSr: result.descriptionSr,
            descriptionEn: result.descriptionEn,
            address: result.address,
            imageUrl: result.imageUrl,
            cuisineType: result.cuisineType,
            isActive: result.isActive,
            isFeatured: result.isFeatured
          };
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    }
  }

  submit(): void {
    this.saving.set(true);
    this.error.set(false);

    if (this.editId) {
      this.restaurantService.update(this.editId, this.form).subscribe({
        next: () => {
          this.saving.set(false);
          this.router.navigate(['/restaurants', this.editId]);
        },
        error: () => {
          this.saving.set(false);
          this.error.set(true);
        }
      });
      return;
    }

    this.restaurantService.create(this.form).subscribe({
      next: (result) => {
        this.saving.set(false);
        this.router.navigate(['/restaurants', result.id]);
      },
      error: () => {
        this.saving.set(false);
        this.error.set(true);
      }
    });
  }
}
