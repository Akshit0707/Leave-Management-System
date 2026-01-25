import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-form-field',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule],
  template: `
    <mat-form-field appearance="outline" class="form-field">
      <mat-label>{{ label }}</mat-label>
      <input matInput [type]="type" [formControl]="control" [placeholder]="placeholder" [required]="required" />
      <mat-error *ngIf="control?.invalid && control?.touched">{{ error }}</mat-error>
    </mat-form-field>
  `,
  styleUrls: ['./form-field.component.css']
})
export class FormFieldComponent {
  @Input() label: string = '';
  @Input() type: string = 'text';
  @Input() placeholder: string = '';
  @Input() required: boolean = false;
  @Input() error: string = '';
  @Input() control!: FormControl;
}
