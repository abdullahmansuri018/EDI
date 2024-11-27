import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone:true,
  imports:[CommonModule,FormsModule,RouterModule]
})
export class RegisterComponent {
  user: User = { email: '', password: '' };
  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  register() {
    this.authService.register(this.user).subscribe({
      next: (response) => {
        this.errorMessage = ''; // Clear any previous error messages
        if (response.message === "User registered successfully") {
          alert('Registration successful! You can now log in.');
          this.router.navigate(['/login']);
        } else {
          this.errorMessage = 'Unexpected response: ' + response.message;
        }
      },
      error: (err) => {
        console.error('Registration error:', err);
        if (err.status === 400) {
          this.errorMessage = `Registration failed: ${err.error.message || 'Invalid input'}`;
        } else {
          this.errorMessage = 'Registration failed. Please try again later.';
        }
      }
    });
  }

  goToLogin(){
    this.router.navigate(['/login']);
  }
}
