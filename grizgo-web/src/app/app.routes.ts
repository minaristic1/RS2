import { Routes } from '@angular/router';
import { DeliveryTrackingComponent } from './delivery/delivery-tracking/delivery-tracking.component';
import { RestaurantListComponent } from './restaurants/restaurant-list/restaurant-list.component';
import { RestaurantDetailComponent } from './restaurants/restaurant-detail/restaurant-detail.component';
import { RestaurantCreateComponent } from './restaurants/restaurant-create/restaurant-create.component';
import { CartViewComponent } from './cart/cart-view/cart-view.component';
import { CourierDashboardComponent } from './courier/courier-dashboard/courier-dashboard.component';
import { RestaurantOrdersComponent } from './restaurant-orders/restaurant-orders/restaurant-orders.component';
import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { NotFoundComponent } from './shared/not-found/not-found.component';

export const routes: Routes = [
  { path: '', redirectTo: 'restaurants', pathMatch: 'full' },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'track', component: DeliveryTrackingComponent },
  { path: 'restaurants', component: RestaurantListComponent },
  { path: 'restaurants/new', component: RestaurantCreateComponent },
  { path: 'restaurants/:id/edit', component: RestaurantCreateComponent },
  { path: 'restaurants/:id', component: RestaurantDetailComponent },
  { path: 'cart', component: CartViewComponent },
  { path: 'courier', component: CourierDashboardComponent },
  { path: 'restaurant-orders', component: RestaurantOrdersComponent },
  { path: '**', component: NotFoundComponent }
];
