// Helper to robustly check if a value is truthy for 'rejected' (handles boolean, number, string)
function isRejected(val: any): boolean {
  return val === true || val === 'true' || val === 1 || val === '1';
}

function isRequestRejected(req: any): boolean {
  // Prefer status field if present, fallback to isRejected
  if (req && typeof req.status === 'string') {
    return req.status.toLowerCase() === 'rejected';
  }
  return isRejected(req?.isRejected);
}

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
    this.passwordResetService.getLatestResetRequest(this.email).subscribe({
      next: (req: PasswordResetRequest | undefined) => {
        if (req && req.isApproved && !req.isCompleted) {
          // Already approved, go to set password
          this.resetRequestId = req.id;
          this.message = 'approved';
          this.step = 3;
          this.isLoading = false;
          return;
        } else if (isRequestRejected(req)) {
          // Already rejected: show rejection in status section with admin comment
          this.snackBar.open('Your previous request was rejected. Please try again or contact admin.', 'Close', {
            duration: 6000,
            verticalPosition: 'top',
            panelClass: ['snackbar-warning']
          });
          this.message = req && (req as any).comment ? `Rejected: ${(req as any).comment}` : 'Your request was rejected. Please try again or contact admin.';
          this.step = 2;
          this.isLoading = false;
          return;
        } else if (req && !req.isCompleted && !req.isRejected && !req.isApproved) {
          // Pending request exists
          this.isLoading = false;
          this.showPopup('Request already in review. Please wait for admin approval.');
          return;
        }
        // No pending/approved/rejected request, proceed
        this.authService.requestPasswordReset(this.email).subscribe({
          next: () => {
            // Immediately show Await Approval step
            this.message = 'Request submitted. Please wait until admin approves your request.';
            this.step = 2;
            this.isLoading = false;
            // After submitting, check approval status in background
            setTimeout(() => {
              this.isLoading = true;
              this.passwordResetService.getLatestResetRequest(this.email).subscribe({
                next: (latestReq: PasswordResetRequest | undefined) => {
                  if (latestReq && latestReq.isApproved && !latestReq.isCompleted) {
                    this.resetRequestId = latestReq.id;
                    this.message = 'approved';
                    this.step = 3;
                  } else if (isRequestRejected(latestReq)) {
                    this.snackBar.open('Your request was rejected. Please try again or contact admin.', 'Close', {
                      duration: 6000,
                      verticalPosition: 'top',
                      panelClass: ['snackbar-warning']
                    });
                    this.message = latestReq && (latestReq as any).comment ? `Rejected: ${(latestReq as any).comment}` : 'Your request was rejected. Please try again or contact admin.';
                    this.step = 2;
                  } else if (latestReq && !latestReq.isCompleted && !latestReq.isRejected && !latestReq.isApproved) {
                    this.message = 'Request already in review. Please wait for admin approval.';
                    this.step = 2;
                  } else {
                    this.message = 'Request submitted. Please wait until admin approves your request.';
                    this.step = 2;
                  }
                  this.isLoading = false;
                },
                error: () => {
                  this.message = 'Request submitted, but failed to check approval status.';
                  this.step = 2;
                  this.isLoading = false;
                }
              });
            }, 1000); // Check status after 1 second
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
        } else if (isRequestRejected(req)) {
          this.snackBar.open('Your request was rejected. Please try again or contact admin.', 'Close', {
            duration: 6000,
            verticalPosition: 'top',
            panelClass: ['snackbar-warning']
          });
          this.message = req && (req as any).comment ? `Rejected: ${(req as any).comment}` : 'Your request was rejected. Please try again or contact admin.';
          this.step = 2;
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
