import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-admin-password-resets',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatSnackBarModule],
  templateUrl: './admin-password-resets.component.html',
  styleUrl: './admin-password-resets.component.css'
})
export class AdminPasswordResetsComponent implements OnInit {
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
    this.http.get<any[]>('/api/auth/pending-password-resets').subscribe({
      next: (data) => {
        this.requests = data.map(r => ({ ...r, status: r.status || 'pending' }));
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

  approve(requestId: number) {
    this.http.post('/api/auth/approve-password-reset', requestId).subscribe({
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
}
