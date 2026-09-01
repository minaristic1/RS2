import { Routes } from '@angular/router';
import { DeliveryTrackingComponent } from './delivery/delivery-tracking/delivery-tracking.component';
import { RestaurantListComponent } from './restaurants/restaurant-list/restaurant-list.component';
import { RestaurantDetailComponent } from './restaurants/restaurant-detail/restaurant-detail.component';
import { RestaurantCreateComponent } from './restaurants/restaurant-create/restaurant-create.component';
import { RestaurantMenuManageComponent } from './restaurants/restaurant-menu-manage/restaurant-menu-manage.component';
import { CartViewComponent } from './cart/cart-view/cart-view.component';
import { CourierDashboardComponent } from './courier/courier-dashboard/courier-dashboard.component';
import { RestaurantOrdersComponent } from './restaurant-orders/restaurant-orders/restaurant-orders.component';
import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { CreateStaffComponent } from './admin/create-staff/create-staff.component';
import { ContactComponent } from './contact/contact.component';
import { PaymentComponent } from './billing/payment/payment.component';
import { NotFoundComponent } from './shared/not-found/not-found.component';
import { adminGuard } from './auth/guards/admin.guard';
import { roleGuard } from './auth/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'restaurants', pathMatch: 'full' },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'track', component: DeliveryTrackingComponent },
  { path: 'restaurants', component: RestaurantListComponent },
  { path: 'restaurants/new', component: RestaurantCreateComponent, canActivate: [adminGuard] },
  { path: 'restaurants/:id/edit', component: RestaurantCreateComponent },
  { path: 'restaurants/:id/menu-manage', component: RestaurantMenuManageComponent },
  { path: 'restaurants/:id', component: RestaurantDetailComponent },
  { path: 'cart', component: CartViewComponent },
  { path: 'courier', component: CourierDashboardComponent, canActivate: [roleGuard(['Driver', 'Admin'])] },
  { path: 'restaurant-orders', component: RestaurantOrdersComponent, canActivate: [roleGuard(['RestaurantOwner', 'RestaurantEmployee', 'Admin'])] },
  { path: 'admin/create-staff', component: CreateStaffComponent, canActivate: [adminGuard] },
  { path: 'contact', component: ContactComponent },
  { path: 'payment', component: PaymentComponent },
  { path: '**', component: NotFoundComponent }
];
