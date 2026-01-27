import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { UserService } from '../../services/user';
import { NavbarComponent } from '../../components/navbar.component';
import { FormFieldComponent } from '../../components/form-field/form-field.component';
import { ErrorMessageComponent } from '../../components/error-message/error-message.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    NavbarComponent,
    FormFieldComponent,
    ErrorMessageComponent,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  firstNameControl = new FormControl('', [Validators.required]);
  lastNameControl = new FormControl('', [Validators.required]);
  emailControl = new FormControl('', [Validators.required, Validators.email]);
  passwordControl = new FormControl('', [Validators.required, Validators.minLength(6)]);
  confirmPasswordControl = new FormControl('', [Validators.required]);
  role: string = 'Employee';
  managerId: number | null = null;
  error: string = '';
  isLoading: boolean = false;
  managers: any[] = [];

  get firstNameError() {
    if (this.firstNameControl.hasError('required')) return 'First name is required';
    return '';
  }
  get lastNameError() {
    if (this.lastNameControl.hasError('required')) return 'Last name is required';
    return '';
  }
  get emailError() {
    if (this.emailControl.hasError('required')) return 'Email is required';
    if (this.emailControl.hasError('email')) return 'Invalid email address';
    return '';
  }
  get passwordError() {
    if (this.passwordControl.hasError('required')) return 'Password is required';
    if (this.passwordControl.hasError('minlength')) return 'Password must be at least 6 characters';
    return '';
  }
  get confirmPasswordError() {
    if (this.confirmPasswordControl.hasError('required')) return 'Please confirm your password';
    if (this.passwordControl.value !== this.confirmPasswordControl.value) return 'Passwords do not match';
    return '';
  }

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
    if (
      this.firstNameControl.invalid ||
      this.lastNameControl.invalid ||
      this.emailControl.invalid ||
      this.passwordControl.invalid ||
      this.confirmPasswordControl.invalid ||
      this.passwordControl.value !== this.confirmPasswordControl.value
    ) {
      this.firstNameControl.markAsTouched();
      this.lastNameControl.markAsTouched();
      this.emailControl.markAsTouched();
      this.passwordControl.markAsTouched();
      this.confirmPasswordControl.markAsTouched();
      this.error = 'Please fill in all fields correctly';
      return;
    }
    this.isLoading = true;
    this.error = '';
    const managerId = this.role === 'Manager' ? null : this.managerId;
    this.authService.register(
      this.emailControl.value!,
      this.passwordControl.value!,
      this.firstNameControl.value!,
      this.lastNameControl.value!,
      this.role,
      managerId
    ).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: (err) => {
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

  onRoleChange() {
    if (this.role === 'Employee') {
      this.fetchManagers();
    } else {
      this.managerId = null;
    }
  }
}