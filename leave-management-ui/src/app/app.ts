import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, Router, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Auth } from './services/auth';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule,
  MatDialogModule,
  MatInputModule,
  MatButtonModule,
  MatSelectModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  exactOptions = { exact: true };

  constructor(public authService: Auth, private router: Router) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  isManager() {
    return this.authService.isManager();
  }
  
  isLandingPage() {
    const url = this.router.url;
    return url === '/' || url.startsWith('/login') || url.startsWith('/register');
  }
}
