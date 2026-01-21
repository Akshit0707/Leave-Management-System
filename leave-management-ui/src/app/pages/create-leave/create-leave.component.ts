import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LeaveService } from '../../services/leave';

@Component({
  selector: 'app-create-leave',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-leave.component.html',
  styleUrl: './create-leave.component.css'
})
export class CreateLeaveComponent {
  startDate: string = '';
  endDate: string = '';
  reason: string = '';
  error: string = '';
  success: string = '';
  isLoading: boolean = false;
  minDate: string = '';

  constructor(private leaveService: LeaveService, private router: Router) {
    // Set minimum date to today
    const today = new Date();
    this.minDate = today.toISOString().split('T')[0];
  }

  submit() {
    // Clear messages
    this.error = '';
    this.success = '';

    if (!this.startDate || !this.endDate || !this.reason) {
      this.error = 'Please fill in all fields';
      return;
    }

    const start = new Date(this.startDate);
    const end = new Date(this.endDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    // Validate start date is not in the past
    if (start < today) {
      this.error = 'Start date cannot be in the past';
      return;
    }

    // Validate end date is not in the past
    if (end < today) {
      this.error = 'End date cannot be in the past';
      return;
    }

    // Validate end date is after start date
    if (end < start) {
      this.error = 'End date must be after start date';
      return;
    }

    this.isLoading = true;
    this.leaveService.createLeave(this.startDate, this.endDate, this.reason).subscribe({
      next: () => {
        this.success = 'Leave request created successfully!';
        setTimeout(() => this.router.navigate(['/my-leaves']), 2000);
      },
      error: () => {
        this.error = 'Failed to create leave request';
        this.isLoading = false;
      }
    });
  }

  reset() {
    this.startDate = '';
    this.endDate = '';
    this.reason = '';
    this.error = '';
    this.success = '';
  }
}