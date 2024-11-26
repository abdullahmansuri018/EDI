import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:5089/api/authenticate';  // Adjust this URL to your backend API

  constructor(private http: HttpClient) {}

  // Login function that stores the token
  login(user: User): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, user);
  }

  // Register function
  register(user: User): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, user, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  // Check if user is logged in by verifying token presence in localStorage
  isAuthenticated(): boolean {
    // Check if running in the browser before accessing localStorage
    if (typeof window !== 'undefined' && window.localStorage) {
      return !!localStorage.getItem('token');
    }
    return false;  // Return false if running in a non-browser environment
  }

  // Get the current JWT token
  getToken(): string | null {
    // Check if running in the browser before accessing localStorage
    if (typeof window !== 'undefined' && window.localStorage) {
      return localStorage.getItem('token');
    }
    return null;  // Return null if running in a non-browser environment
  }

  // Log the user out by removing the token from localStorage
  logout(): void {
    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.removeItem('token');
    }
  }
}
