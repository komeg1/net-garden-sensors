import {Component, OnDestroy, OnInit} from '@angular/core';
import {SensorRealTime} from "../../model/SensorRealTime";
import {SensorService} from "../../service/sensor.service";
import {Sensor} from "../../model/Sensor";
import {SensorFilter} from "../../model/SensorFilter";

@Component({
  selector: 'app-overview-view',
  templateUrl: './overview-view.component.html',
  styleUrls: ['./overview-view.component.css']
})
export class OverviewViewComponent implements OnInit, OnDestroy {
  allSensorData: Sensor[] = [];
  allLatestData: Sensor[] = [];
  sensorData: { [key: number]: SensorRealTime } = {};
  sensorIds: number[] = Array.from({ length: 16 }, (_, id) => id);
  eventSource?: EventSource;
  tableTitles: string[] = ['Temperature', 'Humidity', 'Windy', 'Sun'];
  isReady = false;

  filter: SensorFilter = {
    sensorId: null,
    type: '',
    startDate: '',
    endDate: '',
    sort: '',
  };

  constructor(private sensorService: SensorService) {}

  ngOnInit(): void {
    this.initSensorData();
    this.startSSEConnection();
  }

  ngOnDestroy(): void {
    this.eventSource?.close();
  }

  initSensorData(): void {
    this.sensorService.getFilteredData(this.filter).subscribe({
      next: (data) => {
        this.allSensorData = data;
        this.getLatestData();
      },
      error: (err) => {
        console.error('Data collection failed');
      }
    });
  }

  getLatestData(): void {
    this.sensorService.getLastData().subscribe({
      next: (data) => {
        this.allLatestData = data.sort((data1, data2) => data1.sensorId - data2.sensorId);
        this.sensorIds.forEach(id => {
          const sensor = this.allLatestData.find(data => data.sensorId === id);
          this.sensorData[id] = {
            latest: sensor?.value ?? '-',
            unit: sensor?.unit ?? '',
            values: (this.allSensorData || [])
              .filter(data => data.sensorId === id)
              .sort((data1, data2) => new Date(data2.timestamp).getTime() - new Date(data1.timestamp).getTime())
              .slice(0,100)
              .sort((data1, data2) => new Date(data1.timestamp).getTime() - new Date(data2.timestamp).getTime())
              .map(data => data.value)}
        });
        this.isReady = true;
      },
      error: (err) => {
        console.error('Failed to get latest data')
      }
    });
  }

  private startSSEConnection(): void {
    this.eventSource = new EventSource('sse');

    this.eventSource.onmessage = (event) => {
      const data = JSON.parse(event.data);
      this.processSSEData(data);
    };

    this.eventSource.onerror = (error) => {
      console.error('Error with SSE connection:', error);
      this.eventSource?.close();
    };
  }

  private processSSEData(data: { SensorId: number; Value: number; Unit: string; Timestamp: string }): void {
    const { SensorId: sensorId, Value: value, Unit: unit} = data;

    if (this.sensorData[sensorId]) {
      this.sensorData[sensorId].latest = value;
      this.sensorData[sensorId].unit = unit;
      this.sensorData[sensorId].values.push(value);

      if (this.sensorData[sensorId].values.length > 100) {
        this.sensorData[sensorId].values.shift();
      }

      const avg = this.calculateAverage(this.sensorData[sensorId].values);
      this.setSensorValues(sensorId, value, unit, avg);
    }
  }

  public calculateAverage(values: number[]): string {
    const sum = values.reduce((acc, val) => acc + val, 0);
    return (sum / values.length).toFixed(2);
  }

  private setSensorValues(sensorId: number, latest: number, unit: string, avg: string): void {
    const element = document.getElementById(`sensor-${sensorId}`);
    if (element) {
      const rowLatest = element.querySelector('.row-latest');
      const rowAverage = element.querySelector('.row-average');
      if (rowLatest) {
        rowLatest.textContent = `${latest} ${unit}`;

        rowLatest.classList.remove('animate-text');
        rowLatest.classList.add('animate-text');
        setTimeout(() => {
          rowLatest.classList.remove('animate-text')
        }, 1000)
      }
      if (rowAverage) {
        rowAverage.textContent = `${avg} ${unit}`;

        rowAverage.classList.remove('animate-text');
        rowAverage.classList.add('animate-text');
        setTimeout(() => {
          rowAverage.classList.remove('animate-text')
        }, 1000)
      }
    }
  }
}
