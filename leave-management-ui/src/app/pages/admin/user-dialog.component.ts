import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-user-dialog',
  templateUrl: './user-dialog.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatSelectModule]
})
export class UserDialogComponent {
  form: FormGroup;
  roles = ['Employee', 'Manager'];
  managers: any[] = [];

  constructor(
    public dialogRef: MatDialogRef<UserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder
  ) {
    this.managers = data.managers || [];
    this.form = this.fb.group({
      firstName: [data.user?.firstName || '', Validators.required],
      lastName: [data.user?.lastName || '', Validators.required],
      email: [data.user?.email || '', [Validators.required, Validators.email]],
      role: [data.user?.role || 'Employee', Validators.required],
      managerId: [data.user?.managerId || null]
    });
  }

  save() {
    if (this.form.valid) {
      // If role is not Employee, clear managerId
      const value = { ...this.form.value };
      if (value.role !== 'Employee') {
        value.managerId = null;
      }
      this.dialogRef.close(value);
    }
  }

  close() {
    this.dialogRef.close();
  }
}
