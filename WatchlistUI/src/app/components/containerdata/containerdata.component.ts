import { Component, OnInit } from '@angular/core';
import { ContainerDataService } from '../../services/containerdata.service';
import { PaymentService } from '../../services/payment.service';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-containerdata',
  templateUrl: './containerdata.component.html',
  styleUrls: ['./containerdata.component.css'],
  imports: [FormsModule, RouterModule, CommonModule],
  standalone: true
})
export class ContainerDataComponent implements OnInit {
  containerId: string = '';
  fetchedData: any[] = [];
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private containerDataService: ContainerDataService,
    private paymentService: PaymentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchDataByUserId();
  }

  // Fetch initial data by user ID
  fetchDataByUserId() {
    this.containerDataService.fetchContainerDataByUserId().subscribe({
      next: (data) => {
        this.fetchedData = data && data.length > 0 ? data : [];
        this.errorMessage = data.length === 0 ? 'No containers found' : '';
      },
      error: (err) => this.errorMessage = err.error?.message || 'Failed to fetch data'
    });
  }

  // Fetch data for a specific container
  onFetchData() {
    if (!this.containerId.trim()) {
      this.errorMessage = 'Container ID is required';
      return;
    }
    if (this.fetchedData.some(container => container.containerId === this.containerId)) {
      this.errorMessage = 'This container ID has already been entered';
      alert(this.errorMessage);
    }

    this.containerDataService.fetchContainerDataByContainerId(this.containerId).subscribe({
      next: (data) => {
        if (data) {
          this.fetchedData.push(data);
          this.successMessage = `Container ${this.containerId} fetched successfully.`;
        } else {
          this.errorMessage = 'No data returned for this container ID';
        }
      },
      error: (err) => {this.errorMessage = err.error?.message || 'Failed to fetch data';
        alert(this.errorMessage);
      }
      
    });
  }

  // Remove container
  removeContainer(containerId: string) {
    this.containerDataService.removeContainer(containerId).subscribe({
      next: () => {
        this.fetchedData = this.fetchedData.filter(container => container.containerId !== containerId);
        this.successMessage = `Container ${containerId} removed successfully.`;
      },
      error: (err) => this.errorMessage = err.error?.message || 'Failed to remove container' 
    });
  }

  // Mark container as paid and process service bus message
  markAsPaid(containerId: string) {
    const token = localStorage.getItem('token');
    if (!token) {
      this.errorMessage = 'Authentication token is missing. Please log in again.';
      alert(this.errorMessage);
    }

    this.paymentService.markAsPaid(containerId).subscribe({
      next: () => {
        this.paymentService.processServiceBusMessage().subscribe({
          next: (response) => {
            this.successMessage = `Payment processed successfully and ${response?.message || 'Service bus message processed.'}`;
            alert(this.successMessage);
            this.fetchDataByUserId();
          },
          error: (err) => this.errorMessage = err.error?.message || 'Failed to process service bus message'
        });
      },
      error: (err) => this.errorMessage = err?.error?.message || 'Failed to mark as paid'
    });
  }

  // Logout method
  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }
}
