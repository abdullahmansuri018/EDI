import { Component } from '@angular/core';
import { ContainerDataService } from '../../services/containerdata.service';  // Adjust the import path
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

  constructor(private containerDataService: ContainerDataService) {}

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
        this.fetchedData = data;  // Directly update the data
        this.fetchDataByUserId();  // Fetch data by userId after this action
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
        this.fetchDataByUserId();  // Fetch updated data after removal
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to remove container';  // Handle error
      }
    });
  }
}
