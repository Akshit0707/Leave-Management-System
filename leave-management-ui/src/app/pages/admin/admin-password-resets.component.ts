  // ...existing code...
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { environment } from '../../../environments/environment';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-admin-password-resets',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatSnackBarModule, FormsModule, MatProgressSpinnerModule],
  templateUrl: './admin-password-resets.component.html',
  styleUrl: './admin-password-resets.component.css'
})
export class AdminPasswordResetsComponent implements OnInit {
    delete(requestId: number) {
      if (!confirm('Are you sure you want to delete this password reset request?')) return;
      this.http.delete(`${environment.apiUrl}/api/auth/delete-password-reset/${requestId}`).subscribe({
        next: () => {
          this.snackBar.open('Request deleted', 'Close', { duration: 2000 });
          this.loadRequests();
        },
        error: (err) => {
          this.snackBar.open('Delete failed', 'Close', { duration: 3000 });
          this.error = 'Delete failed';
        }
      });
    }
  displayedColumns: string[] = ['email', 'requestedAt', 'status', 'actions'];
  requests: any[] = [];
  isLoading = false;
  error: string = '';

  constructor(private http: HttpClient, private snackBar: MatSnackBar) {}

  ngOnInit() {
    this.loadRequests();
  }

  loadRequests() {
    this.isLoading = true;
    this.http.get<any[]>(`${environment.apiUrl}/api/auth/all-password-resets`).subscribe({
      next: (data) => {
        console.log('Password reset requests from backend:', data);
        this.requests = data.map(r => ({ ...r, status: r.status || 'pending' }));
        console.log('Requests mapped for display:', this.requests);
        this.isLoading = false;
        this.error = '';
      },
      error: (err) => {
        this.error = 'Failed to load requests';
        this.snackBar.open('Failed to load requests', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  approve(requestId: number, comment: string) {
    this.http.post(`${environment.apiUrl}/api/auth/approve-password-reset`, { requestId, comment }).subscribe({
      next: () => {
        this.snackBar.open('Request approved', 'Close', { duration: 2000 });
        this.loadRequests();
      },
      error: (err) => {
        this.snackBar.open('Approval failed', 'Close', { duration: 3000 });
        this.error = 'Approval failed';
      }
    });
  }

  reject(requestId: number, comment: string) {
    this.http.post(`${environment.apiUrl}/api/auth/reject-password-reset`, { requestId, comment }).subscribe({
      next: () => {
        this.snackBar.open('Request rejected', 'Close', { duration: 2000 });
        this.loadRequests();
      },
      error: (err) => {
        this.snackBar.open('Rejection failed', 'Close', { duration: 3000 });
        this.error = 'Rejection failed';
      }
    });
  }
}
