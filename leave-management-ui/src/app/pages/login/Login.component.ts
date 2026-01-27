import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { NavbarComponent } from '../../components/navbar.component';
import { FormFieldComponent } from '../../components/form-field/form-field.component';
import { ErrorMessageComponent } from '../../components/error-message/error-message.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    NavbarComponent,
    FormFieldComponent,
    ErrorMessageComponent
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  emailControl = new FormControl('', [Validators.required, Validators.email]);
  passwordControl = new FormControl('', [Validators.required]);
  error: string = '';
  isLoading: boolean = false;

  get emailError() {
    if (this.emailControl.hasError('required')) return 'Email is required';
    if (this.emailControl.hasError('email')) return 'Invalid email address';
    return '';
  }
  get passwordError() {
    if (this.passwordControl.hasError('required')) return 'Password is required';
    return '';
  }

  constructor(private authService: Auth, private router: Router) {}

  login() {
    if (this.emailControl.invalid || this.passwordControl.invalid) {
      this.emailControl.markAsTouched();
      this.passwordControl.markAsTouched();
      this.error = 'Please fill in all fields correctly';
      return;
    }
    this.isLoading = true;
    this.error = '';
    this.authService.login(this.emailControl.value!, this.passwordControl.value!).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.error = 'Invalid email or password';
        this.isLoading = false;
      }
    });
  }
}