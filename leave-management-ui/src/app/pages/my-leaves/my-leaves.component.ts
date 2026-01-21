import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeaveService } from '../../services/leave';

@Component({
  selector: 'app-my-leaves',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-leaves.component.html',
  styleUrl: './my-leaves.component.css'
})
export class MyLeavesComponent implements OnInit {
  leaves: any[] = [];
  isLoading: boolean = true;
  deleteConfirmId: number | null = null;
  isDeletingId: number | null = null;

  constructor(private leaveService: LeaveService) {}

  ngOnInit() {
    this.loadLeaves();
  }

  loadLeaves() {
    this.isLoading = true;
    this.leaveService.getMyLeaves().subscribe({
      next: (data) => {
        this.leaves = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  getStatusClass(status: string): string {
    return status.toLowerCase();
  }

  canDelete(leave: any): boolean {
    // Only allow deletion if status is Pending
    return leave.status.toLowerCase() === 'pending';
  }

  confirmDelete(leaveId: number) {
    this.deleteConfirmId = leaveId;
  }

  cancelDelete() {
    this.deleteConfirmId = null;
  }

  deleteLeave(leaveId: number) {
    this.isDeletingId = leaveId;
    this.leaveService.deleteLeave(leaveId).subscribe({
      next: () => {
        this.leaves = this.leaves.filter(l => l.id !== leaveId);
        this.deleteConfirmId = null;
        this.isDeletingId = null;
      },
      error: () => {
        alert('Failed to delete leave request');
        this.isDeletingId = null;
      }
    });
  }
}