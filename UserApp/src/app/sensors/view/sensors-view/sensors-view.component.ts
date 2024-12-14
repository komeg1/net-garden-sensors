import {Component, OnInit} from '@angular/core';
import {SensorService} from "../../service/sensor.service";
import {Sensor} from "../../model/Sensor";
import {SensorFilter} from "../../model/SensorFilter";

@Component({
  selector: 'app-sensors-view',
  templateUrl: './sensors-view.component.html',
  styleUrls: ['./sensors-view.component.css']
})
export class SensorsViewComponent implements OnInit{
  sensorData: Sensor[] = [];
  paginatedData: Sensor[] = [];
  lastSensorData: Sensor | null = null;
  avgSensorData: number | null = null;
  errorMessage: string | null = null;

  currentPage: number = 1;
  pageSize: number = 15;

  filter: SensorFilter = {
    sensorId: null,
    type: '',
    startDate: '',
    endDate: '',
    sort: 'NONE',
  };

  constructor(private sensorService: SensorService) {}

  ngOnInit(): void {
    this.getData();
    this.getLastData();
  }

  /* get data from sensors */
  getData(): void {
    this.sensorService.getFilteredData(this.filter).subscribe({
      next: (data) => {
        this.sensorData = data;
        this.updatePagination();
      },
      error: (err) => {
        this.errorMessage = 'Data collection failed.';
      }
    });
  }

  /* get last data from sensors */
  getLastData(): void {
    this.sensorService.getLastData().subscribe({
      next: (data) => {
        this.lastSensorData = data[0];
        this.calculateAverage();
      },
      error: (err) => {
        this.errorMessage = 'Failed to get latest data.';
      }
    });
  }

  /* calculate average from data */
  private calculateAverage(): void {
      const sensorValues = this.sensorData.slice(0, 100);
      const total = sensorValues.reduce((sum, sensor) => sum + sensor.value, 0);
      this.avgSensorData = total / sensorValues.length;
  }

  /* get filtered data */
  applyFilters(): void {
    this.currentPage = 1;
    this.getData();
  }

  /* clear all filters */
  clearFilters(): void {
    this.filter = {
      sensorId: null,
      type: '',
      startDate: '',
      endDate: '',
      sort: 'NONE',
    };
    this.currentPage = 1;
    this.getData();
  }

  /* display chart  */
  //TODO
  seeCharts(){

  }

  /* download CSV with filtered data sensors */
  downloadCSV(): void {
    this.sensorService.exportData(this.filter, 'CSV').subscribe({
      next: (response) => this.downloadFile(response, 'sensors_data.csv'),
      error: (err) => {
        this.errorMessage = 'Failed to download CSV file.';
      }
    });
  }

  /* download JSON with filtered data sensors */
  downloadJSON(): void {
    this.sensorService.exportData(this.filter, 'JSON').subscribe({
      next: (response) =>{
        console.log("downloadFile called");
        this.downloadFile(response, 'sensors_data.json');},
      error: (err) => {
        this.errorMessage = 'Failed to download JSON file.';
      }
    });
  }

  private downloadFile(data: Blob, filename: string): void {
    const blob = new Blob([data], { type: 'application/octet-stream' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = filename;
    link.click();
  }

  /* pagination */
  updatePagination(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedData = this.sensorData.slice(startIndex, endIndex);
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePagination();
    }
  }

  nextPage(): void {
    if (this.currentPage < Math.ceil(this.sensorData.length / this.pageSize)) {
      this.currentPage++;
      this.updatePagination();
    }
  }

  protected readonly Math = Math;
}
