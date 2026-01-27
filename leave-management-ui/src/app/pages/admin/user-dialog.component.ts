import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    // 🔴 REQUIRED for mat-dialog-* directives
    MatDialogModule,

    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  templateUrl: './user-dialog.component.html',
  styleUrl: './user-dialog.component.css'
})
export class UserDialogComponent {
  form: FormGroup;
  roles = ['Employee', 'Manager', 'Admin'];
  managers: any[] = [];
  isEdit: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<UserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder
  ) {
    const user = data?.user || {};
    this.managers = data?.managers || [];
    this.isEdit = !!data?.isEdit;
    this.form = this.fb.group({
      firstName: [user.firstName || '', Validators.required],
      lastName: [user.lastName || '', Validators.required],
      email: [user.email || '', [Validators.required, Validators.email]],
      password: ['', this.isEdit ? [] : [Validators.required, Validators.minLength(6)]],
      role: [user.role ? user.role : (user.role === 0 ? 'Employee' : user.role === 1 ? 'Manager' : user.role === 2 ? 'Admin' : 'Employee'), Validators.required],
      managerId: [user.managerId ?? null]
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  update(): void {
    if (this.form.valid) {
      this.dialogRef.close({ ...this.form.value });
    }
  }
}
