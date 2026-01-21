import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeaveService } from '../../services/leave';
import { Auth } from '../../services/auth';
import { PendingLeavesComponent } from '../pending-leaves/pending-leaves.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, PendingLeavesComponent],
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

  constructor(private leaveService: LeaveService, private authService: Auth) {}

  ngOnInit() {
    this.isManager = this.authService.isManager();
    const user = this.authService.getUser();
    this.userName = user ? `${user.firstName}` : 'User';
    this.loadSummary();
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
}