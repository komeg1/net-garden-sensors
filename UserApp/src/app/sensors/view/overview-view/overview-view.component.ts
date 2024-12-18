import {Component, OnDestroy, OnInit} from '@angular/core';
import {SensorRealTime} from "../../model/SensorRealTime";

@Component({
  selector: 'app-overview-view',
  templateUrl: './overview-view.component.html',
  styleUrls: ['./overview-view.component.css']
})
export class OverviewViewComponent implements OnInit, OnDestroy {
  sensorData: { [key: number]: SensorRealTime } = {};
  sensorIds: number[] = Array.from({ length: 16 }, (_, id) => id);
  eventSource?: EventSource;
  tableTitles: string[] = ['Temperature', 'Density', 'Speed', 'Power'];

  ngOnInit(): void {
    this.initSensorData();
    this.startSSEConnection();
  }

  ngOnDestroy(): void {
    this.eventSource?.close();
  }

  private initSensorData(): void {
    const storedData = localStorage.getItem('sensorData');

    if (storedData) {
      this.sensorData = JSON.parse(storedData);
    } else {
      this.sensorIds.forEach((id) => {
        this.sensorData[id] = { latest: 0, unit: '', values: [] };
      });
    }
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
    const { SensorId: sensorId, Value: value, Unit: unit, Timestamp: timestamp } = data;

    if (this.sensorData[sensorId]) {
      const sensor = this.sensorData[sensorId];
      sensor.latest = value;
      sensor.unit = unit;
      sensor.values.push(value);

      if (sensor.values.length > 100) {
        sensor.values.shift();
      }

      const avg = this.calculateAverage(sensor.values);
      this.updateLog(sensorId, value, unit, avg);

      localStorage.setItem('sensorData', JSON.stringify(this.sensorData));
    }
  }

  public calculateAverage(values: number[]): string {
    const sum = values.reduce((acc, val) => acc + val, 0);
    return (sum / values.length).toFixed(2);
  }

  private updateLog(sensorId: number, latest: number, unit: string, avg: string): void {
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
