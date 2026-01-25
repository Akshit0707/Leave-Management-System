import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,

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

  constructor(
    public dialogRef: MatDialogRef<UserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public user: any
  ) {}

  close(): void {
    this.dialogRef.close();
  }

  update(): void {
    this.dialogRef.close(this.user);
  }
}
