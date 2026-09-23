import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { AdminListComponent } from './admin/admin-list/admin-list.component';
import { AdminEditComponent } from './admin/admin-edit/admin-edit.component';
import { ProfileComponent } from './profile/profile.component';
import { CartComponent } from './cart/cart.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'admin', component: AdminListComponent },
    { path: 'admin/new', component: AdminEditComponent }, 
    { path: 'admin/:id', component: AdminEditComponent },
    { path: 'profile', component: ProfileComponent },
    { path: 'cart', component: CartComponent }
];