import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LeaveService {
  private apiUrl = 'https://leave-management-system-production-0b95.up.railway.app/api/leaves';

  constructor(private http: HttpClient) {}

  createLeave(startDate: string, endDate: string, reason: string): Observable<any> {
    return this.http.post(`${this.apiUrl}`, { startDate, endDate, reason });
  }

  getMyLeaves(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/mine`);
  }

  getPendingLeaves(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/pending`);
  }

  updateLeaveStatus(id: number, status: number, managerComments: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/status`, { status, managerComments });
  }

  deleteLeave(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getSummary(): Observable<any> {
    return this.http.get(`${this.apiUrl}/summary`);
  }
}
