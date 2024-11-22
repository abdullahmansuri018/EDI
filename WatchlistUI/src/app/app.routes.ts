import { Routes } from '@angular/router';
import { LoginComponent } from '../app/components/login/login.component';
import { RegisterComponent } from '../app/components/register/register.component';
import { AuthGuard } from '../app/Guards/auth.guard';  // Import the AuthGuard
import { ContainerDataComponent } from './components/containerdata/containerdata.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'containerdata', component: ContainerDataComponent, canActivate: [AuthGuard] },
];
