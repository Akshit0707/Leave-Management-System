
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Auth } from './auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(Auth);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/login']);
    return false;
  }

  // If admin, always redirect to admin dashboard
  if (authService.isAdmin() && state.url !== '/admin') {
    router.navigate(['/admin']);
    return false;
  }

  return true;
};