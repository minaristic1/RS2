import { Routes } from '@angular/router';
import { DeliveryTrackingComponent } from './delivery/delivery-tracking/delivery-tracking.component';

export const routes: Routes = [
  { path: '', redirectTo: 'track', pathMatch: 'full' },
  { path: 'track', component: DeliveryTrackingComponent }
];
