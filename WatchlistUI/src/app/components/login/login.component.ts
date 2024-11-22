import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';  // Import the User model
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  imports: [CommonModule, RouterModule, FormsModule],
  styleUrls: ['./login.component.css'],
  standalone: true,
})
export class LoginComponent {
  user: User = { email: '', password: '' };
  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.authService.login(this.user).subscribe({
      next: (response: any) => {
        localStorage.setItem('token', response.token);  // Store JWT token in localStorage
        this.errorMessage = '';  // Clear any previous error messages
        this.router.navigate(['/containerdata']);  // Navigate to a protected route
      },
      error: () => {
        this.errorMessage = 'Invalid email or password.';  // Show error message if login fails
      }
    });
  }
}
