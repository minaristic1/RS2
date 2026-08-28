import { Routes } from '@angular/router';
import { DeliveryTrackingComponent } from './delivery/delivery-tracking/delivery-tracking.component';
import { RestaurantListComponent } from './restaurants/restaurant-list/restaurant-list.component';
import { RestaurantDetailComponent } from './restaurants/restaurant-detail/restaurant-detail.component';
import { CartViewComponent } from './cart/cart-view/cart-view.component';
import { CourierDashboardComponent } from './courier/courier-dashboard/courier-dashboard.component';
import { RestaurantOrdersComponent } from './restaurant-orders/restaurant-orders/restaurant-orders.component';
import { NotFoundComponent } from './shared/not-found/not-found.component';

export const routes: Routes = [
  { path: '', redirectTo: 'restaurants', pathMatch: 'full' },
  { path: 'track', component: DeliveryTrackingComponent },
  { path: 'restaurants', component: RestaurantListComponent },
  { path: 'restaurants/:id', component: RestaurantDetailComponent },
  { path: 'cart', component: CartViewComponent },
  { path: 'courier', component: CourierDashboardComponent },
  { path: 'restaurant-orders', component: RestaurantOrdersComponent },
  { path: '**', component: NotFoundComponent }
];
