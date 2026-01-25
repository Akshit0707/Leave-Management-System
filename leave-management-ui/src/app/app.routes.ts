import { Routes } from '@angular/router';

import { RegisterComponent } from './pages/register/register.component';
import { AdminComponent } from './pages/admin/admin.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { MyLeavesComponent } from './pages/my-leaves/my-leaves.component';
import { CreateLeaveComponent } from './pages/create-leave/create-leave.component';
import { PendingLeavesComponent } from './pages/pending-leaves/pending-leaves.component';
import { authGuard } from './services/auth.guard';
import { managerGuard } from './services/manager.guard';
import { LoginComponent } from './pages/login/Login.component';


export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/landing/landing.component').then(m => m.LandingComponent) },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'my-leaves', component: MyLeavesComponent, canActivate: [authGuard] },
  { path: 'create-leave', component: CreateLeaveComponent, canActivate: [authGuard] },
  { path: 'pending-leaves', component: PendingLeavesComponent, canActivate: [authGuard, managerGuard] },
  { path: 'admin', component: AdminComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];
