import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
  email: string = '';
  message: string = '';
  isLoading: boolean = false;

  constructor(private authService: Auth, private router: Router) {}

  requestReset() {
    if (!this.email) {
      this.message = 'Please enter your email.';
      return;
    }
    this.isLoading = true;
    // Simulate API call to request password reset (admin approval required)
    this.authService.requestPasswordReset(this.email).subscribe({
      next: () => {
        this.message = 'Request submitted. Please wait for admin approval.';
        this.isLoading = false;
      },
      error: () => {
        this.message = 'Failed to submit request. Try again.';
        this.isLoading = false;
      }
    });
  }
}
