// Helper to robustly check if a value is truthy for 'rejected' (handles boolean, number, string)
function isRejected(val: any): boolean {
  return val === true || val === 'true' || val === 1 || val === '1';
}
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PasswordResetService {
  private apiUrl = 'https://leavemgmt-hqgybuaccpf6f6an.eastasia-01.azurewebsites.net/api/password-resets';

  constructor(private http: HttpClient) {}

  getLatestResetRequest(email: string) {
    return this.http.get<any[]>(`${this.apiUrl}/all-password-resets`).pipe(
      map(requests => {
        // Always select the most recent request for the email
        const all = requests
          .filter((r: any) => r.email === email)
          .sort((a: any, b: any) => new Date(b.requestedAt).getTime() - new Date(a.requestedAt).getTime());
        if (all.length > 0) return all[0];
        return undefined;
      })
    );
  }
}
