import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  imports:[CommonModule,RouterModule,FormsModule],
  styleUrls: ['./register.component.css'],
  standalone: true,
})
export class RegisterComponent {
  user: User = { email: '', password: '' };
  errorMessage: string = '';

  constructor(private authService: AuthService) {}

  register() {
    this.authService.register(this.user).subscribe({
      next: () => {
        this.errorMessage = '';
        alert('Registration successful! You can now log in.');
      },
      error: () => {
        this.errorMessage = 'Registration failed. Please try again.';
      }
    });
  }
}
