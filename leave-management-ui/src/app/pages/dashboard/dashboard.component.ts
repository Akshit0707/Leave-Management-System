  get pastRequests() {
    return this.allRequests.filter(req => req.status !== 'Pending');
  }
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeaveService } from '../../services/leave';
import { Auth } from '../../services/auth';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatToolbarModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatGridListModule,
    MatProgressSpinnerModule,
    MatListModule,
    MatDividerModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  summary: any = {
    totalRequests: 0,
    approvedRequests: 0,
    rejectedRequests: 0,
    pendingRequests: 0,
    totalDaysRequested: 0,
    totalDaysApproved: 0
  };

  isLoading: boolean = true;
  isManager: boolean = false;
  userName: string = '';

  pendingRequests: any[] = [];
  allRequests: any[] = [];
  requestsLoading: boolean = true;

  constructor(private leaveService: LeaveService, private authService: Auth) {}

  ngOnInit() {
    this.isManager = this.authService.isManager();
    const user = this.authService.getUser();
    this.userName = user ? `${user.firstName}` : 'User';
    this.loadSummary();
    if (this.isManager) {
      this.loadManagerRequests();
    }
  }

  loadSummary() {
    this.leaveService.getSummary().subscribe({
      next: (data) => {
        this.summary = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  loadManagerRequests() {
    this.requestsLoading = true;
    this.leaveService.getPendingLeaves().subscribe({
      next: (pending) => {
        this.pendingRequests = pending;
        this.leaveService.getAllLeaves().subscribe({
          next: (all) => {
            this.allRequests = all;
            this.requestsLoading = false;
          },
          error: () => {
            this.requestsLoading = false;
          }
        });
      },
      error: () => {
        this.requestsLoading = false;
      }
    });
  }
}