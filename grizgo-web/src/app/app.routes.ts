import { Routes } from '@angular/router';
import { DeliveryTrackingComponent } from './delivery/delivery-tracking/delivery-tracking.component';
import { RestaurantListComponent } from './restaurants/restaurant-list/restaurant-list.component';
import { CartViewComponent } from './cart/cart-view/cart-view.component';

export const routes: Routes = [
  { path: '', redirectTo: 'restaurants', pathMatch: 'full' },
  { path: 'track', component: DeliveryTrackingComponent },
  { path: 'restaurants', component: RestaurantListComponent },
  { path: 'cart', component: CartViewComponent }
];
