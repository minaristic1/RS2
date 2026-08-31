import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { RestaurantService } from '../services/restaurant.service';
import { Restaurant } from '../models/restaurant';
import { RestaurantMenuList, MenuItemSummary } from '../models/menu';

const DEFAULT_IMAGE_URL = 'https://placehold.co/300x200?text=GrizGo';

const EMPTY_ITEM_FORM = {
  nameSr: '',
  nameEn: '',
  descriptionSr: '',
  descriptionEn: '',
  price: 0,
  imageUrl: '',
  isAvailable: true,
  isFeatured: false,
  preparationTimeMinutes: 10
};

@Component({
  selector: 'app-restaurant-menu-manage',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './restaurant-menu-manage.component.html',
  styleUrl: './restaurant-menu-manage.component.css'
})
export class RestaurantMenuManageComponent implements OnInit {
  restaurantId = '';
  restaurant = signal<Restaurant | null>(null);
  menuList = signal<RestaurantMenuList | null>(null);
  loading = signal(true);
  error = signal(false);

  showAddMenuForm = signal(false);
  newMenuForm = { nameSr: '', nameEn: '', descriptionSr: '', descriptionEn: '', displayOrder: 1 };

  addCategoryForMenuId = signal<string | null>(null);
  newCategoryForm = { nameSr: '', nameEn: '', descriptionSr: '', descriptionEn: '', displayOrder: 1 };

  addItemForCategoryId = signal<string | null>(null);
  newItemForm = { ...EMPTY_ITEM_FORM };
  activeMenuIdForItem = '';

  editingItemId = signal<string | null>(null);
  editItemForm = { ...EMPTY_ITEM_FORM };

  saving = signal(false);

  constructor(
    private route: ActivatedRoute,
    private restaurantService: RestaurantService
  ) {}

  ngOnInit(): void {
    this.restaurantId = this.route.snapshot.paramMap.get('id') ?? '';
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(false);

    this.restaurantService.getById(this.restaurantId).subscribe({
      next: (r) => this.restaurant.set(r),
      error: () => this.error.set(true)
    });

    this.restaurantService.getMenu(this.restaurantId).subscribe({
      next: (m) => {
        this.menuList.set(m);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set(true);
      }
    });
  }

  submitNewMenu(): void {
    this.saving.set(true);
    this.restaurantService.createMenu(this.restaurantId, this.newMenuForm).subscribe({
      next: () => {
        this.saving.set(false);
        this.showAddMenuForm.set(false);
        this.newMenuForm = { nameSr: '', nameEn: '', descriptionSr: '', descriptionEn: '', displayOrder: 1 };
        this.load();
      },
      error: () => {
        this.saving.set(false);
        this.error.set(true);
      }
    });
  }

  openAddCategory(menuId: string): void {
    this.addCategoryForMenuId.set(menuId);
    this.newCategoryForm = { nameSr: '', nameEn: '', descriptionSr: '', descriptionEn: '', displayOrder: 1 };
  }

  submitNewCategory(menuId: string): void {
    this.saving.set(true);
    this.restaurantService.createCategory(this.restaurantId, menuId, this.newCategoryForm).subscribe({
      next: () => {
        this.saving.set(false);
        this.addCategoryForMenuId.set(null);
        this.load();
      },
      error: () => {
        this.saving.set(false);
        this.error.set(true);
      }
    });
  }

  openAddItem(menuId: string, categoryId: string): void {
    this.activeMenuIdForItem = menuId;
    this.addItemForCategoryId.set(categoryId);
    this.newItemForm = { ...EMPTY_ITEM_FORM };
  }

  submitNewItem(categoryId: string): void {
    this.saving.set(true);
    const payload = { ...this.newItemForm, imageUrl: this.newItemForm.imageUrl.trim() || DEFAULT_IMAGE_URL };
    this.restaurantService.createItem(this.restaurantId, this.activeMenuIdForItem, categoryId, payload).subscribe({
      next: () => {
        this.saving.set(false);
        this.addItemForCategoryId.set(null);
        this.load();
      },
      error: () => {
        this.saving.set(false);
        this.error.set(true);
      }
    });
  }

  openEditItem(item: MenuItemSummary): void {
    this.editingItemId.set(item.id);
    this.editItemForm = {
      nameSr: item.nameSr,
      nameEn: item.nameSr,
      descriptionSr: item.descriptionSr,
      descriptionEn: '',
      price: item.price,
      imageUrl: item.imageUrl,
      isAvailable: item.isAvailable,
      isFeatured: false,
      preparationTimeMinutes: 10
    };
  }

  submitEditItem(itemId: string): void {
    this.saving.set(true);
    const payload = { ...this.editItemForm, imageUrl: this.editItemForm.imageUrl.trim() || DEFAULT_IMAGE_URL };
    this.restaurantService.updateItem(itemId, payload).subscribe({
      next: () => {
        this.saving.set(false);
        this.editingItemId.set(null);
        this.load();
      },
      error: () => {
        this.saving.set(false);
        this.error.set(true);
      }
    });
  }

  toggleAvailability(item: MenuItemSummary): void {
    this.restaurantService
      .updateItem(item.id, {
        nameSr: item.nameSr,
        nameEn: item.nameSr,
        descriptionSr: item.descriptionSr,
        descriptionEn: '',
        price: item.price,
        imageUrl: item.imageUrl,
        isAvailable: !item.isAvailable,
        isFeatured: false,
        preparationTimeMinutes: 10
      })
      .subscribe({
        next: () => this.load(),
        error: () => this.error.set(true)
      });
  }

  deleteItem(itemId: string): void {
    this.restaurantService.deleteItem(itemId).subscribe({
      next: () => this.load(),
      error: () => this.error.set(true)
    });
  }
}
