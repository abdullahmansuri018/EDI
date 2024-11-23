import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContainerDataService {
  private apiUrl = 'http://localhost:5089/api/data';  // Adjust the API base URL

  constructor(private http: HttpClient) { }

  // Method to fetch data by containerId
  fetchContainerDataByContainerId(containerId: string): Observable<any> {
    const token = localStorage.getItem('token');  // Get the JWT token from localStorage
    if (!token) {
      throw new Error('User is not authenticated');
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<any>(`${this.apiUrl}/fetch-by-containerId/${containerId}`, { headers });
  }

  // Method to fetch data by userId
  fetchContainerDataByUserId(): Observable<any> {
    const token = localStorage.getItem('token');  // Get the JWT token from localStorage
    if (!token) {
      throw new Error('User is not authenticated');
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<any>(`${this.apiUrl}/fetch-by-userId`, { headers });
  }

  removeContainer(containerId: string): Observable<any> {
    const token = localStorage.getItem('token');  // Get the JWT token from localStorage
    if (!token) {
      throw new Error('User is not authenticated');
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.delete<any>(`${this.apiUrl}/remove-container/${containerId}`, { headers });
  }
}
