import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class LeaveService {

  private apiUrl = `${environment.apiUrl}/api/leaves`;

  constructor(private http: HttpClient) {}

  // CREATE LEAVE (Employee)
  createLeave(
    startDate: string,
    endDate: string,
    reason: string
  ): Observable<any> {
    return this.http.post<any>(this.apiUrl, {
      startDate,
      endDate,
      reason
    });
  }

  // EMPLOYEE LEAVES
  getMyLeaves(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/mine`);
  }

  // MANAGER - ALL LEAVES
  getAllLeaves(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // MANAGER - PENDING ONLY
  getPendingLeaves(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/pending`);
  }

  // MANAGER - UPDATE STATUS
  updateLeaveStatus(
    id: number,
    status: string,
    managerComments: string
  ): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/status`, {
      status,
      managerComments
    });
  }

  // DELETE LEAVE
  deleteLeave(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  // DASHBOARD SUMMARY
  getSummary(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/summary`);
  }
}
