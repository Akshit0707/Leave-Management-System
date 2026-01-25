import { HttpClient } from '@angular/common/http';
import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class Auth {

  private apiUrl = `${environment.apiUrl}/api/auth`;

  private tokenSubject = new BehaviorSubject<string | null>(null);
  public token$ = this.tokenSubject.asObservable();

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  register(
    email: string,
    password: string,
    firstName: string,
    lastName: string,
    role: string,
    managerId: number | null = null
  ) {
    return this.http.post<any>(`${this.apiUrl}/register`, {
      email: email,
      password: password,
      firstName: firstName,
      lastName: lastName,
      role: role,
      managerId: managerId
    }).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  login(email: string, password: string) {
    return this.http.post<any>(`${this.apiUrl}/login`, { email, password })
      .pipe(
        tap(response => this.handleAuthSuccess(response))
      );
  }

  logout() {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      this.tokenSubject.next(null);
    }
  }

  private handleAuthSuccess(response: any) {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('token', response.token);
      localStorage.setItem('user', JSON.stringify(response));
      this.tokenSubject.next(response.token);
    }
  }

  getToken(): string | null {
    return isPlatformBrowser(this.platformId)
      ? localStorage.getItem('token')
      : null;
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  isLoggedIn(): boolean {
    return this.isAuthenticated();
  }

  getUser() {
    if (isPlatformBrowser(this.platformId)) {
      const user = localStorage.getItem('user');
      return user ? JSON.parse(user) : null;
    }
    return null;
  }


  isManager(): boolean {
    const user = this.getUser();
    // Accept both string and numeric role for compatibility
    return user?.role === 'Manager' || user?.role === 1;
  }

  isAdmin(): boolean {
    const user = this.getUser();
    // Accept both string and numeric role for compatibility
    return user?.role === 'Admin' || user?.role === 2;
  }

  getUserRole(): string | null {
    return this.getUser()?.role ?? null;
  }

  requestPasswordReset(email: string) {
    // Simulate API call for password reset request (admin approval required)
    // Replace with actual backend call
    return this.http.post<any>(`${this.apiUrl}/request-password-reset`, { email });
  }
}
