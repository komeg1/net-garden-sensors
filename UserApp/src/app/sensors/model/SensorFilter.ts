export interface SensorFilter {
  sensorId: number | null;
  type: string;
  startDate: string;
  endDate: string;
  sort: 'NONE' | 'ASCENDING' | 'DESCENDING';
}
