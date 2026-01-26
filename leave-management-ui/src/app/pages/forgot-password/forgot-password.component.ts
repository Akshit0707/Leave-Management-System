
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { PasswordResetService } from '../../services/password-reset.service';


interface PasswordResetRequest {
  id: number;
  email: string;
  isApproved: boolean;
  isCompleted: boolean;
  isRejected?: boolean;
}

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
  resetRequestId: number | null = null;
  newPassword: string = '';
  confirmPassword: string = '';
  resetError: string = '';
  passwordStrength: string = '';
  passwordStrengthLabel: string = '';
  step: number = 1; // 1: request, 2: pending, 3: reset, 4: done

  constructor(private authService: Auth, private router: Router, private passwordResetService: PasswordResetService) {}

  requestReset() {
    if (!this.email) {
      this.message = 'Please enter your email.';
      return;
    }
    this.isLoading = true;
    this.passwordResetService.getLatestResetRequest(this.email).subscribe({
      next: (req: PasswordResetRequest | undefined) => {
        if (req && req.isApproved && !req.isCompleted) {
          this.resetRequestId = req.id;
          this.isLoading = false;
          this.step = 3;
        } else if (req && !req.isApproved && !req.isCompleted && !req.isRejected) {
          // Pending request exists
          this.message = 'Your request is still pending admin approval.';
          this.isLoading = false;
          this.step = 2;
        } else {
          // Allow new request if previous is rejected or completed
          this.authService.requestPasswordReset(this.email).subscribe({
            next: () => {
              this.message = 'Request submitted. Please wait until admin approves your request.';
              this.isLoading = false;
              this.step = 2;
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

  checkApproval() {
    if (!this.email) {
      this.message = 'Please enter your email.';
      return;
    }
    this.isLoading = true;
    this.passwordResetService.getLatestResetRequest(this.email).subscribe({
      next: (req: PasswordResetRequest | undefined) => {
        if (req && req.isApproved && !req.isCompleted) {
          this.resetRequestId = req.id;
          this.step = 3;
        } else if (req && req.isRejected) {
          this.message = 'Your request was rejected. You can submit a new request.';
          this.step = 1;
        } else if (req && !req.isApproved) {
          this.message = 'Your request is still pending admin approval.';
        } else {
          this.message = 'No pending request found. Please submit a new request.';
          this.step = 1;
        }
        this.isLoading = false;
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
        this.step = 4;
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

  checkPasswordStrength() {
    const pwd = this.newPassword || '';
    if (pwd.length < 6) {
      this.passwordStrength = 'weak';
      this.passwordStrengthLabel = 'Weak';
    } else if (pwd.match(/[A-Z]/) && pwd.match(/[0-9]/) && pwd.length >= 8) {
      this.passwordStrength = 'strong';
      this.passwordStrengthLabel = 'Strong';
    } else {
      this.passwordStrength = 'medium';
      this.passwordStrengthLabel = 'Medium';
    }
  }
}
