import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SensorsViewComponent } from './sensors/view/sensors-view/sensors-view.component';
import {HttpClientModule} from "@angular/common/http";
import {FormsModule} from "@angular/forms";
import { OverviewViewComponent } from './sensors/view/overview-view/overview-view.component';
import {BaseChartDirective, provideCharts, withDefaultRegisterables} from "ng2-charts";
import {DatePipe} from "@angular/common";
import { WalletViewComponent } from './sensors/view/wallet-view/wallet-view.component';

@NgModule({
  declarations: [
    AppComponent,
    SensorsViewComponent,
    OverviewViewComponent,
    WalletViewComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    BaseChartDirective,
  ],
  providers: [
    provideCharts(withDefaultRegisterables()),
    DatePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
