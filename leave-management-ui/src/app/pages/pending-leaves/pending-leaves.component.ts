import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LeaveService } from '../../services/leave';

@Component({
  selector: 'app-pending-leaves',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pending-leaves.component.html',
  styleUrl: './pending-leaves.component.css'
})
export class PendingLeavesComponent implements OnInit {
  leaves: any[] = [];
  isLoading: boolean = true;
  selectedLeave: any = null;
  actionType: 'approve' | 'reject' | null = null;
  managerComment: string = '';
  isSubmitting: boolean = false;
  successMessage: string = '';

  constructor(private leaveService: LeaveService) {}

  ngOnInit() {
    this.loadPendingLeaves();
  }

  loadPendingLeaves() {
    this.isLoading = true;
    this.leaveService.getPendingLeaves().subscribe({
      next: (data) => {
        this.leaves = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  openApproveDialog(leave: any) {
    this.selectedLeave = leave;
    this.actionType = 'approve';
    this.managerComment = '';
  }

  openRejectDialog(leave: any) {
    this.selectedLeave = leave;
    this.actionType = 'reject';
    this.managerComment = '';
  }

  approve() {
    this.submitAction(1, this.managerComment);
  }

  reject() {
    if (!this.managerComment.trim()) {
      alert('Please provide a reason for rejection');
      return;
    }
    this.submitAction(2, this.managerComment);
  }

  submitAction(status: number, comment: string) {
    if (!this.selectedLeave) return;

    this.isSubmitting = true;
    this.leaveService.updateLeaveStatus(this.selectedLeave.id, status, comment).subscribe({
      next: () => {
        this.successMessage = status === 1 ? 'Leave approved successfully!' : 'Leave rejected successfully!';
        setTimeout(() => {
          this.successMessage = '';
          this.closeDialog();
          this.loadPendingLeaves();
        }, 2000);
      },
      error: () => {
        alert('Failed to update leave status');
        this.isSubmitting = false;
      }
    });
  }

  closeDialog() {
    this.selectedLeave = null;
    this.actionType = null;
    this.managerComment = '';
    this.isSubmitting = false;
  }

  getDaysCount(startDate: string, endDate: string): number {
    const start = new Date(startDate);
    const end = new Date(endDate);
    return Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1;
  }
}