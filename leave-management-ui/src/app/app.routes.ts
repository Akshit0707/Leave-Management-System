import { Routes } from '@angular/router';
import { authGuard } from './services/auth.guard';
import { managerGuard } from './services/manager.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/landing/landing.component').then(m => m.LandingComponent) },
  { path: 'login', loadComponent: () => import('./pages/login/Login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./pages/register/register.component').then(m => m.RegisterComponent) },
  { path: 'forgot-password', loadComponent: () => import('./pages/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent) },
  { path: 'dashboard', loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent), canActivate: [authGuard] },
  { path: 'reset-password', loadComponent: () => import('./pages/reset-password/reset-password.component').then(m => m.ResetPasswordComponent) },
  { path: 'my-leaves', loadComponent: () => import('./pages/my-leaves/my-leaves.component').then(m => m.MyLeavesComponent), canActivate: [authGuard] },
  { path: 'create-leave', loadComponent: () => import('./pages/create-leave/create-leave.component').then(m => m.CreateLeaveComponent), canActivate: [authGuard] },
  { path: 'pending-leaves', loadComponent: () => import('./pages/pending-leaves/pending-leaves.component').then(m => m.PendingLeavesComponent), canActivate: [authGuard, managerGuard] },
  { path: 'admin', loadComponent: () => import('./pages/admin/admin.component').then(m => m.AdminComponent), canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];
