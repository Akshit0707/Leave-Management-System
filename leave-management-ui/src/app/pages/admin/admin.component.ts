import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserDialogComponent } from './user-dialog.component';

import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormsModule } from '@angular/forms';
import { AdminPasswordResetsComponent } from './admin-password-resets.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSnackBarModule,
    MatDialogModule,
    UserDialogComponent,
    MatProgressSpinnerModule,
    AdminPasswordResetsComponent,
    RouterModule
  ]
})
export class AdminComponent implements OnInit {
  usersDataSource = new MatTableDataSource<any>([]);
  usersColumns = ['id', 'firstName', 'lastName', 'email', 'role', 'manager', 'actions'];
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  isLoading = true;
  error: string | null = null;

  managers: any[] = [];
  constructor(private userService: UserService, private snackBar: MatSnackBar, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.usersDataSource.data = users.map((u: any) => ({
          ...u,
          manager: u.managerId ? users.find((m: any) => m.id === u.managerId)?.firstName + ' ' + users.find((m: any) => m.id === u.managerId)?.lastName : '-'
        }));
        this.usersDataSource.paginator = this.paginator;
        this.usersDataSource.sort = this.sort;
        this.isLoading = false;
        // Also fetch managers for dialog dropdowns
        this.userService.getManagers().subscribe({
          next: (mgrs) => { this.managers = mgrs; },
          error: () => { this.managers = []; }
        });
      },
      error: () => {
        this.error = 'Failed to load users';
        this.isLoading = false;
      }
    });
  }

  addUser() {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      data: { isEdit: false, managers: this.managers },
      width: '400px'
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.addUser(result).subscribe({
          next: () => {
            this.snackBar.open('User added!', 'Close', { duration: 2000, panelClass: 'snackbar-success' });
            this.loadUsers();
          },
          error: (err) => {
            this.snackBar.open('Add failed: ' + (err.error || err.message), 'Close', { duration: 3000, panelClass: 'snackbar-error' });
          }
        });
      }
    });
  }

  editUser(user: any) {
    const dialogRef = this.dialog.open(UserDialogComponent, {
      data: { isEdit: true, user, managers: this.managers },
      width: '400px'
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.userService.editUser(user.id, result).subscribe({
          next: () => {
            this.snackBar.open('User updated!', 'Close', { duration: 2000, panelClass: 'snackbar-success' });
            this.loadUsers();
          },
          error: (err) => {
            this.snackBar.open('Edit failed: ' + (err.error || err.message), 'Close', { duration: 3000, panelClass: 'snackbar-error' });
          }
        });
      }
    });
  }

  deleteUser(user: any) {
    if (!confirm(`Delete user ${user.firstName} ${user.lastName}?`)) return;
    this.userService.deleteUser(user.id).subscribe({
      next: () => {
        this.snackBar.open('User deleted!', 'Close', { duration: 2000, panelClass: 'snackbar-success' });
        this.loadUsers();
      },
      error: (err) => {
        this.snackBar.open('Delete failed: ' + (err.error || err.message), 'Close', { duration: 3000, panelClass: 'snackbar-error' });
      }
    });
  }
}
