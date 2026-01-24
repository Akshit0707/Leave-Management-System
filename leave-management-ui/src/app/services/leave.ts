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

  createLeave(
    startDate: string,
    endDate: string,
    reason: string
  ): Observable<any> {
    return this.http.post(this.apiUrl, {
      StartDate: startDate,
      EndDate: endDate,
      Reason: reason
    });
  }

  getMyLeaves(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/mine`);
  }


  getPendingLeaves(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/pending`);
  }

  getAllLeaves(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`);
  }

  updateLeaveStatus(
    id: number,
    status: number,
    managerComments: string
  ): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/status`, {
      Status: status,
      ManagerComments: managerComments
    });
  }

  deleteLeave(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getSummary(): Observable<any> {
    return this.http.get(`${this.apiUrl}/summary`);
  }
}
