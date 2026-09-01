export interface MenuItemSummary {
  id: string;
  nameSr: string;
  descriptionSr: string;
  price: number;
  imageUrl: string;
  isAvailable: boolean;
}

export interface MenuCategory {
  id: string;
  nameSr: string;
  displayOrder: number;
  items: MenuItemSummary[];
}

export interface Menu {
  menuId: string;
  nameSr: string;
  categories: MenuCategory[];
}

export interface RestaurantMenuList {
  restaurantId: string;
  menus: Menu[];
}

export interface CreateMenuRequest {
  nameSr: string;
  nameEn: string;
  descriptionSr: string;
  descriptionEn: string;
  displayOrder: number;
}

export interface CreateMenuCategoryRequest {
  nameSr: string;
  nameEn: string;
  descriptionSr: string;
  descriptionEn: string;
  displayOrder: number;
}

export interface CreateMenuItemRequest {
  nameSr: string;
  nameEn: string;
  descriptionSr: string;
  descriptionEn: string;
  price: number;
  imageUrl: string;
  isAvailable: boolean;
  isFeatured: boolean;
  preparationTimeMinutes: number;
}
