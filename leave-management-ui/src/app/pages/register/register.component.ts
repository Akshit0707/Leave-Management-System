import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { UserService } from '../../services/user';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  firstName: string = '';
  lastName: string = '';
  role: string = 'Employee'; // 'Employee' or 'Manager'
  managerId: number | null = null;
  error: string = '';
  isLoading: boolean = false;
  managers: any[] = [];


  constructor(private authService: Auth, private userService: UserService, private router: Router) {}

  ngOnInit() {
    if (this.role === 'Employee') {
      this.fetchManagers();
    }
  }

  fetchManagers() {
    this.userService.getManagers().subscribe({
      next: (data) => {
        this.managers = data;
      },
      error: (err) => {
        this.managers = [];
      }
    });
  }

  register() {
    // Validation
    if (!this.email || !this.password || !this.firstName || !this.lastName) {
      this.error = 'Please fill in all required fields';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    if (this.password.length < 6) {
      this.error = 'Password must be at least 6 characters';
      return;
    }

    // Email validation
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(this.email)) {
      this.error = 'Please enter a valid email address';
      return;
    }

    this.isLoading = true;
    this.error = '';


    // Set managerId to null if Manager is selected
    const managerId = this.role === 'Manager' ? null : this.managerId;

    this.authService.register(
      this.email,
      this.password,
      this.firstName,
      this.lastName,
      this.role,
      managerId
    ).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Registration error:', err);
        if (err.error) {
          if (typeof err.error === 'string') {
            this.error = err.error;
          } else if (typeof err.error === 'object') {
            this.error = err.error.error || err.error.message || JSON.stringify(err.error);
          } else {
            this.error = JSON.stringify(err.error);
          }
        } else {
          this.error = err.message || 'Registration failed. Please try again.';
        }
        this.isLoading = false;
      }
    });
  }

  // When role changes, update manager list
  onRoleChange() {
    if (this.role === 'Employee') {
      this.fetchManagers();
    } else {
      this.managerId = null;
    }
  }
}