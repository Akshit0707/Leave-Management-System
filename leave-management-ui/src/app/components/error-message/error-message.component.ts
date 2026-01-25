import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-message',
  standalone: true,
  imports: [CommonModule],
  template: `<div *ngIf="error" class="error-message">{{ error }}</div>`,
  styleUrls: ['./error-message.component.css']
})
export class ErrorMessageComponent {
  @Input() error: string = '';
}
