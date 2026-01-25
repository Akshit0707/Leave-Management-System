import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {
  requestId: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  message: string = '';
  isLoading: boolean = false;
  isSuccess: boolean = false;

  constructor(private route: ActivatedRoute, private authService: Auth, private router: Router) {
    this.route.queryParams.subscribe(params => {
      this.requestId = params['requestId'] || '';
    });
  }

  resetPassword() {
    if (!this.newPassword || !this.confirmPassword) {
      this.message = 'Please fill in all fields.';
      return;
    }
    if (this.newPassword !== this.confirmPassword) {
      this.message = 'Passwords do not match.';
      return;
    }
    if (!this.requestId) {
      this.message = 'Invalid or missing request ID.';
      return;
    }
    this.isLoading = true;
    this.authService.completePasswordReset(+this.requestId, this.newPassword).subscribe({
      next: () => {
        this.message = 'Password reset successful. You can now log in with your new password.';
        this.isSuccess = true;
        this.isLoading = false;
      },
      error: () => {
        this.message = 'Failed to reset password. Please try again or contact support.';
        this.isLoading = false;
      }
    });
  }
}
