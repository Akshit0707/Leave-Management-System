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
      map(requests => {
        // Find the latest approved, not completed request for the given email
        const approved = requests
          .filter((r: any) => r.email === email && r.isApproved && !r.isCompleted)
          .sort((a: any, b: any) => new Date(b.requestedAt).getTime() - new Date(a.requestedAt).getTime());
        if (approved.length > 0) return approved[0];
        // Otherwise, return the latest pending (not rejected, not completed, not approved)
        const pending = requests
          .filter((r: any) => r.email === email && !r.isCompleted && !r.isRejected && !r.isApproved)
          .sort((a: any, b: any) => new Date(b.requestedAt).getTime() - new Date(a.requestedAt).getTime());
        if (pending.length > 0) return pending[0];
        // Otherwise, return the latest rejected
        const rejected = requests
          .filter((r: any) => r.email === email && r.isRejected)
          .sort((a: any, b: any) => new Date(b.requestedAt).getTime() - new Date(a.requestedAt).getTime());
        if (rejected.length > 0) return rejected[0];
        return undefined;
      })
    );
  }
}
