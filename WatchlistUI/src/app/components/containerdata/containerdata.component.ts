import { Component } from '@angular/core';
import { ContainerDataService } from '../../services/containerdata.service';  // Adjust the import path
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-containerdata',
  templateUrl: './containerdata.component.html',
  styleUrls: ['./containerdata.component.css'],
  imports:[CommonModule,RouterModule,FormsModule],
  standalone:true
})
export class ContainerDataComponent {
  containerId: string = '';
  fetchedData: any[] = [];
  errorMessage: string = '';

  constructor(private containerDataService: ContainerDataService) {}

  onFetchData() {
    if (this.containerId.trim() === '') {
      this.errorMessage = 'Container ID is required';
      return;
    }

    this.errorMessage = '';  // Clear any previous error message

    this.containerDataService.fetchContainerDataByContainerId(this.containerId).subscribe({
      next: (data) => {
        this.fetchedData = data;
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to fetch data';
      }
    });
  }
}
