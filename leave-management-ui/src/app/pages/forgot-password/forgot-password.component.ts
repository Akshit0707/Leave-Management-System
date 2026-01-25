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
  newPassword: string = '';
  confirmPassword: string = '';
  message: string = '';
  isLoading: boolean = false;
  requestSubmitted: boolean = false;

  constructor(private authService: Auth, private router: Router) {}

  requestReset() {
    if (!this.email || !this.newPassword || !this.confirmPassword) {
      this.message = 'Please fill in all fields.';
      return;
    }
    if (this.newPassword !== this.confirmPassword) {
      this.message = 'Passwords do not match.';
      return;
    }
    this.isLoading = true;
    this.authService.requestPasswordResetWithNewPassword(this.email, this.newPassword).subscribe({
      next: () => {
        this.message = 'Request submitted. Please wait until admin approves your request.';
        this.isLoading = false;
        this.requestSubmitted = true;
      },
      error: () => {
        this.message = 'Failed to submit request. Try again.';
        this.isLoading = false;
      }
    });
  }
}
