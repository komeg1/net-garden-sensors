import {Component, OnInit} from '@angular/core';
import {SensorService} from "../../service/sensor.service";

@Component({
  selector: 'app-wallet-view',
  templateUrl: './wallet-view.component.html',
  styleUrls: ['./wallet-view.component.css']
})
export class WalletViewComponent{
  tableTitles: string[] = ['Temperature', 'Humidity', 'Windy', 'Sun'];
  sensorIds: number[] = Array.from({ length: 16 }, (_, id) => id);
  sensorBalances: { [key: number]: number | undefined } = {};
  loadingSensorId: number | null = null;
  errorMessages: { [key: number]: string | null } = {};
  expandedSensor: number | null = null;

  constructor(private sensorService: SensorService) {}

  toggleSensor(sensorId: number) {
    if (this.expandedSensor === sensorId) {
      this.expandedSensor = null;
    } else {
      this.expandedSensor = sensorId;
      if (this.sensorBalances[sensorId] === undefined) {
        this.fetchSensorBalance(sensorId);
      }
    }
  }

  fetchSensorBalance(sensorId: number) {
    this.loadingSensorId = sensorId;
    this.errorMessages[sensorId] = null;

    this.sensorService.getWalletId(sensorId.toString()).subscribe({
      next: (balance) => {
        this.sensorBalances[sensorId] = balance;
        this.loadingSensorId = null;
      },
      error: (err) => {
        this.errorMessages[sensorId] = `Error loading balance for sensor ${sensorId}.`;
        this.loadingSensorId = null;
      }
    });
  }
}
