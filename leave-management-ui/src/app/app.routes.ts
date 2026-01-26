import { Routes } from '@angular/router';
import { AuthGuard } from './services/auth.guard';
import { ManagerGuard } from './services/manager.guard';
import { AdminGuard } from './services/admin.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/landing/landing.component').then(m => m.LandingComponent) },
  { path: 'login', loadComponent: () => import('./pages/login/Login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./pages/register/register.component').then(m => m.RegisterComponent) },
  { path: 'forgot-password', loadComponent: () => import('./pages/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent) },
  { path: 'dashboard', loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent), canActivate: [AuthGuard] },
  { path: 'reset-password', loadComponent: () => import('./pages/reset-password/reset-password.component').then(m => m.ResetPasswordComponent) },
  { path: 'my-leaves', loadComponent: () => import('./pages/my-leaves/my-leaves.component').then(m => m.MyLeavesComponent), canActivate: [AuthGuard] },
  { path: 'create-leave', loadComponent: () => import('./pages/create-leave/create-leave.component').then(m => m.CreateLeaveComponent), canActivate: [AuthGuard] },
  { path: 'pending-leaves', loadComponent: () => import('./pages/pending-leaves/pending-leaves.component').then(m => m.PendingLeavesComponent), canActivate: [AuthGuard, ManagerGuard] },
  { path: 'admin', loadComponent: () => import('./pages/admin/admin.component').then(m => m.AdminComponent), canActivate: [AuthGuard, AdminGuard] },
  { path: '**', redirectTo: '' }
];
