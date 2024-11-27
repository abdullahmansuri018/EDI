import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  private apiUrl = 'http://localhost:5140/api/payment';

  constructor(private http: HttpClient) {}

  // Method to get the token from localStorage (or wherever it's stored)
  private getAuthToken(): string | null {
    const token = localStorage.getItem('token');  // Make sure this matches what AuthService uses
    console.log('Retrieved token:', token);  // Log the token to check
    return token;
  }

  // Method to call the 'mark-as-paid' endpoint
  markAsPaid(containerId: string): Observable<any> {
    const token = this.getAuthToken();

    // Check if the token exists, and if not, reject the request
    if (!token) {
      return throwError('Authentication token is missing');
    }

    // Set the token in the Authorization header
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    // Call the API and handle errors
    return this.http.post(`${this.apiUrl}/service-bus-message-send/${containerId}`, {}, { headers });
  }

  // Method to call the 'process-service-bus-message' endpoint
  processServiceBusMessage(): Observable<any> {
    const token = this.getAuthToken();

    // Check if the token exists, and if not, reject the request
    if (!token) {
      return throwError('Authentication token is missing');
    }

    // Set the token in the Authorization header
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    // Call the API and handle errors
    return this.http.post(`${this.apiUrl}/process-service-bus-message`, {}, { headers });
  }
}
