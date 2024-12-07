import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {SensorsViewComponent} from "./sensors/view/sensors-view/sensors-view.component";


const routes: Routes = [
  {
    path: 'sensors',
    component: SensorsViewComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
