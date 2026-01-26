
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { PasswordResetService } from '../../services/password-reset.service';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';


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
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule
  ],
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

  constructor(
    private authService: Auth,
    private passwordResetService: PasswordResetService,
    private snackBar: MatSnackBar
  ) {}

  requestReset() {
    if (!this.email) {
      this.message = 'Please enter your email.';
      return;
    }
    this.isLoading = true;
    // Check for existing pending request first
    this.passwordResetService.getLatestResetRequest(this.email).subscribe({
      next: (req: PasswordResetRequest | undefined) => {
        if (req && !req.isCompleted && !req.isRejected && !req.isApproved) {
          // Pending request exists
          this.isLoading = false;
          this.showPopup('Request already in review. Please wait for admin approval.');
          return;
        }
        // No pending request, proceed
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
      },
      error: () => {
        this.message = 'Failed to check existing requests. Try again.';
        this.isLoading = false;
      }
    });
  }

  showPopup(msg: string) {
    this.snackBar.open(msg, 'Close', {
      duration: 4000,
      verticalPosition: 'top',
      panelClass: ['snackbar-warning']
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
          this.message = 'approved';
          setTimeout(() => { this.step = 3; }, 1000);
        } else if (req && req.isRejected) {
          this.message = 'rejected';
        } else if (req && !req.isApproved) {
          this.message = '';
        } else {
          this.message = '';
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
