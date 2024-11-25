import { Component } from '@angular/core';
import { ContainerDataService } from '../../services/containerdata.service';
import { PaymentService } from '../../services/payment.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-containerdata',
  templateUrl: './containerdata.component.html',
  styleUrls: ['./containerdata.component.css'],
  imports: [CommonModule, RouterModule, FormsModule],
  standalone: true
})
export class ContainerDataComponent {
  containerId: string = '';
  fetchedData: any[] = [];
  errorMessage: string = '';
  successMessage: string = '';

  constructor(private containerDataService: ContainerDataService, private paymentService: PaymentService) {}

  ngOnInit(): void {
    this.fetchDataByUserId();  // Fetch initial data when the component is initialized
  }

  // Fetch data by userId on component initialization
  fetchDataByUserId() {
    this.containerDataService.fetchContainerDataByUserId().subscribe({
      next: (data) => {
        this.fetchedData = data;  // Directly update the data
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to fetch data';  // Handle error
      }
    });
  }

  // Fetch data by containerId
  onFetchData() {
    if (this.containerId.trim() === '') {
      this.errorMessage = 'Container ID is required';
      return;
    }

    // Check if the containerId already exists in the fetched data
    const containerExists = this.fetchedData.some(container => container.containerId === this.containerId);
    if (containerExists) {
      this.errorMessage = 'This container ID has already been entered';
      return;
    }

    this.containerDataService.fetchContainerDataByContainerId(this.containerId).subscribe({
      next: (data) => {
        // Add the new data to the fetched list if it's not already present
        this.fetchedData.push(data);  // Assuming the service returns one container object
        this.successMessage = `Container ${this.containerId} fetched successfully.`;
        this.errorMessage = '';  // Clear previous errors
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to fetch data';  // Handle error
      }
    });
  }

  // Remove container
  removeContainer(containerId: string) {
    this.containerDataService.removeContainer(containerId).subscribe({
      next: () => {
        this.fetchedData = this.fetchedData.filter(container => container.containerId !== containerId);  // Remove container locally
        this.successMessage = `Container ${containerId} removed successfully.`;
        this.errorMessage = '';  // Clear previous errors
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to remove container';  // Handle error
      }
    });
  }

  // Mark container as paid and process service bus message
  markAsPaid(containerId: string) {
    this.errorMessage = ''; // Reset error message
    this.successMessage = ''; // Reset success message
    
    // Retrieve token from localStorage
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Authentication token is missing');
      this.errorMessage = 'Authentication token is missing. Please log in again.';
      return;
    }
  
    // Call the mark-as-paid API
    this.paymentService.markAsPaid(containerId).subscribe({
      next: (response) => {
        console.log('Payment successful', response);
    
        // Once payment is successful, process the service bus message
        this.paymentService.processServiceBusMessage().subscribe({
          next: (serviceBusResponse) => {
            console.log('Service bus message processed', serviceBusResponse);
            const message = serviceBusResponse?.message || 'Service bus message processed successfully.';
            this.successMessage = `Payment processed successfully and ${message}`;
            alert(this.successMessage);
          },
          error: (serviceBusError) => {
            console.error('Failed to process service bus message', serviceBusError);
            this.errorMessage = serviceBusError?.error?.message || 'Failed to process service bus message.';
          }
        });
      },
      error: (err) => {
        console.error('Payment failed', err);
        this.errorMessage = err?.error?.message || 'Failed to mark as paid';
      }
    });
  } 
}
