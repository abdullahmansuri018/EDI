import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from '../app/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports:[RouterOutlet],
  styleUrls: ['./app.component.css'],
  standalone: true,
})
export class AppComponent {
  title = 'Authentication System';

  constructor(private authService: AuthService, private router: Router) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);  // Redirect to login after logout
  }
}
