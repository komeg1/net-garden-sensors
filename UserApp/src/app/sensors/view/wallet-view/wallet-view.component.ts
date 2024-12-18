import {Component, OnInit} from '@angular/core';
import {SensorService} from "../../service/sensor.service";

@Component({
  selector: 'app-wallet-view',
  templateUrl: './wallet-view.component.html',
  styleUrls: ['./wallet-view.component.css']
})
export class WalletViewComponent implements OnInit {
  tableTitles: string[] = ['Temperature', 'Humidity', 'Windy', 'Sun'];
  sensorIds: number[] = Array.from({ length: 16 }, (_, id) => id);
  wallet: { [key: number]: number } = {};
  errorMessage: string | null = null;
  isWalletReady = false;

  constructor(private sensorService: SensorService) {}

  ngOnInit(){
    this.sensorService.getWallet().subscribe({
      next: (wallet) => {
        this.wallet = wallet;
        this.isWalletReady = true;
      },
      error: (err) => {
        this.errorMessage = 'Wallet collection failed.';
      }
    });
  }
}
