import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PasswordResetService {
  private apiUrl = `${environment.apiUrl}/api/auth`;

  constructor(private http: HttpClient) {}

  getLatestResetRequest(email: string) {
    return this.http.get<any[]>(`${this.apiUrl}/all-password-resets`).pipe(
      // Find the latest request for the given email
      map(requests => requests.find((r: any) => r.email === email && !r.isCompleted && !r.isRejected)),
    );
  }
}
