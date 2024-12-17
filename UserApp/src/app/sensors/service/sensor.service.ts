import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Sensor} from '../model/Sensor';
import {Observable} from 'rxjs';
import {SensorFilter} from "../model/SensorFilter";

@Injectable({
  providedIn: 'root'
})
export class SensorService {
  constructor(private http: HttpClient) { }

  getFilteredData(filter: SensorFilter): Observable<Sensor[]> {
    let url = 'https://localhost:7258/Sensors?';

    if (filter.sensorId !== null) url += `sensorId=${filter.sensorId}&`;
    if (filter.type) url += `type=${filter.type}&`;
    if (filter.startDate) url += `startDate=${filter.startDate}&`;
    if (filter.endDate) url += `endDate=${filter.endDate}&`;
    if (filter.sort) url += `sort=${filter.sort}&`;

    return this.http.get<Sensor[]>(url);
  }

  getLastData(): Observable<Sensor[]> {
    return this.http.get<Sensor[]>('https://localhost:7258/Sensors/latest');
  }

  exportData(filter: SensorFilter, format: 'CSV' | 'JSON'): Observable<Blob> {
    let url = `https://localhost:7258/Sensors/export?exportFormat=${format}&`;

    if (filter.sensorId !== null) url += `sensorId=${filter.sensorId}&`;
    if (filter.type) url += `type=${filter.type}&`;
    if (filter.startDate) url += `startDate=${filter.startDate}&`;
    if (filter.endDate) url += `endDate=${filter.endDate}&`;
    if (filter.sort) url += `sort=${filter.sort}&`;

    return this.http.get(url, { responseType: 'blob' });
  }
}
