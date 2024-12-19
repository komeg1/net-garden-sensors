import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {SensorsViewComponent} from "./sensors/view/sensors-view/sensors-view.component";
import {OverviewViewComponent} from "./sensors/view/overview-view/overview-view.component";
import {WalletViewComponent} from "./sensors/view/wallet-view/wallet-view.component";


const routes: Routes = [
  {
    path: 'sensors',
    component: SensorsViewComponent
  },
  {
    path: 'overview',
    component: OverviewViewComponent
  },
  {
    path: 'wallet',
    component: WalletViewComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
