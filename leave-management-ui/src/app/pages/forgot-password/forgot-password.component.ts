import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { PasswordResetService } from '../../services/password-reset.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  email: string = '';
  message: string = '';
  isLoading: boolean = false;
  requestSubmitted: boolean = false;
  showResetPopup: boolean = false;
  resetRequestId: number | null = null;
  newPassword: string = '';
  confirmPassword: string = '';
  resetError: string = '';

  constructor(private authService: Auth, private router: Router, private passwordResetService: PasswordResetService) {}

  requestReset() {
    if (!this.email) {
      this.message = 'Please enter your email.';
      return;
    }
    this.isLoading = true;
    // Check for existing approved request
    this.passwordResetService.getLatestResetRequest(this.email).subscribe({
      next: (req) => {
        if (req && req.isApproved && !req.isCompleted) {
          this.showResetPopup = true;
          this.resetRequestId = req.id;
          this.isLoading = false;
        } else if (req && !req.isApproved) {
          this.message = 'Your request is still pending admin approval.';
          this.isLoading = false;
        } else {
          // No request or completed/rejected, submit new request
          this.authService.requestPasswordReset(this.email).subscribe({
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
      },
      error: () => {
        this.message = 'Failed to check request status. Try again.';
        this.isLoading = false;
      }
    });
  }

  resetApprovedPassword() {
    this.resetError = '';
    if (!this.newPassword || !this.confirmPassword) {
      this.resetError = 'Please fill in all fields.';
      return;
    }
    if (this.newPassword !== this.confirmPassword) {
      this.resetError = 'Passwords do not match.';
      return;
    }
    if (!this.resetRequestId) {
      this.resetError = 'Invalid request.';
      return;
    }
    this.isLoading = true;
    this.authService.completePasswordReset(this.resetRequestId, this.newPassword).subscribe({
      next: () => {
        this.showResetPopup = false;
        this.message = 'Password reset successful. You can now log in with your new password.';
        this.isLoading = false;
        this.newPassword = '';
        this.confirmPassword = '';
      },
      error: () => {
        this.resetError = 'Failed to reset password. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
