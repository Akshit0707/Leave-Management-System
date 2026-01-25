import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeaveService } from '../../services/leave';
import { Auth } from '../../services/auth';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatToolbarModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSnackBarModule,
    MatDialogModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})

export class DashboardComponent implements OnInit {
  summary: any = {};
  isLoading = true;
  isManager = false;
  isAdmin = false;
  userName = '';
  managerId: number | null = null;

  // Angular Material DataSources
  pendingDataSource = new MatTableDataSource<any>([]);
  pastDataSource = new MatTableDataSource<any>([]);

  pendingColumns = [
    'id', 'userName', 'dates', 'days', 'reason', 'status', 'comment', 'actions'
  ];

  pastColumns = [
    'id', 'userName', 'dates', 'days', 'reason', 'status',
    'managerComments', 'createdAt', 'reviewedAt'
  ];

  @ViewChild('pendingPaginator') pendingPaginator!: MatPaginator;
  @ViewChild('pastPaginator') pastPaginator!: MatPaginator;
  @ViewChild('pendingSort') pendingSort!: MatSort;
  @ViewChild('pastSort') pastSort!: MatSort;

  requestsLoading = true;
  requestsError: string | null = null;

  constructor(
    private leaveService: LeaveService,
    private authService: Auth,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private router: import('@angular/router').Router
  ) {}

  ngOnInit(): void {
    this.isManager = this.authService.isManager();
    this.isAdmin = this.authService.isAdmin();
    const user = this.authService.getUser();
    this.userName = user?.firstName ?? 'User';
    this.managerId = (this.isManager && user?.userId) ? user.userId : null;

    // If admin, redirect to admin dashboard
    if (this.isAdmin) {
      this.router.navigate(['/admin']);
      return;
    }

    this.loadSummary();

    if (this.isManager) {
      this.loadManagerRequests();
    } else {
      // Employee-specific columns (no userName)
      this.pastColumns = [
        'id', 'dates', 'days', 'reason', 'status',
        'managerComments', 'createdAt', 'reviewedAt'
      ];
      this.loadEmployeeRequests();
    }
  }

  loadEmployeeRequests(): void {
    this.requestsLoading = true;
    this.leaveService.getMyLeaves().subscribe({
      next: (leaves: any[]) => {
        this.pastDataSource.data = leaves;
        this.pastDataSource.paginator = this.pastPaginator;
        this.pastDataSource.sort = this.pastSort;
        this.requestsLoading = false;
      },
      error: () => {
        this.requestsError = 'Failed to load your leave requests';
        this.requestsLoading = false;
      }
    });
  }

  loadSummary(): void {
    this.leaveService.getSummary().subscribe({
      next: (data) => {
        this.summary = data;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  loadManagerRequests(): void {
    this.requestsLoading = true;

    this.leaveService.getAllLeaves().subscribe({
      next: (allLeaves) => {
        const pending = allLeaves.filter(l => l.status === 'Pending');
        const past = allLeaves.filter(l => l.status !== 'Pending');

        this.pendingDataSource.data = pending;
        this.pastDataSource.data = past;

        this.pendingDataSource.paginator = this.pendingPaginator;
        this.pastDataSource.paginator = this.pastPaginator;

        this.pendingDataSource.sort = this.pendingSort;
        this.pastDataSource.sort = this.pastSort;

        this.requestsLoading = false;
      },
      error: () => {
        this.requestsError = 'Failed to load leave requests';
        this.requestsLoading = false;
      }
    });
  }

  approve(req: any): void {
    this.confirmAction('Approve Leave', 'Approve this leave request?', () => {
      this.updateStatus(req, 'Approved');
    });
  }

  reject(req: any): void {
    if (!req._comment?.trim()) {
      this.snackBar.open('Comment is required to reject', 'Close', { duration: 3000 });
      return;
    }

    this.confirmAction('Reject Leave', 'Reject this leave request?', () => {
      this.updateStatus(req, 'Rejected');
    });
  }

  private updateStatus(req: any, status: 'Approved' | 'Rejected'): void {
    this.leaveService.updateLeaveStatus(req.id, status, req._comment || '').subscribe({
      next: () => {
        this.snackBar.open(
          `Leave ${status}`,
          'OK',
          {
            duration: 3000,
            panelClass: status === 'Approved' ? 'snackbar-success' : 'snackbar-error'
          }
        );
        this.loadManagerRequests();
        this.loadSummary();
      },
      error: () => {
        this.snackBar.open('Action failed', 'Close', { duration: 3000, panelClass: 'snackbar-error' });
      }
    });
  }

  private confirmAction(title: string, message: string, onConfirm: () => void): void {
    const confirmed = confirm(`${title}\n\n${message}`);
    if (confirmed) onConfirm();
  }
}
