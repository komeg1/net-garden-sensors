import {Component, OnInit} from '@angular/core';
import {SensorService} from "../../service/sensor.service";
import {Sensor} from "../../model/Sensor";
import {SensorFilter} from "../../model/SensorFilter";
import {ChartConfiguration, ChartData, ChartOptions} from "chart.js";
import { DatePipe } from '@angular/common';


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

  showChart: boolean = false;

  currentSortColumn: string = '';
  currentSortOrder: 'ASCENDING' | 'DESCENDING' = 'ASCENDING';

  sensorIds: number[] = Array.from({ length: 16 }, (_, i) => i);
  filter: SensorFilter = {
    sensorId: null,
    type: '',
    startDate: '',
    endDate: '',
    sort: '',
  };

  chartDataTemperature: ChartData<'line'> = {
    labels: [],
    datasets: []
  };
  chartConfigurationTemperature: ChartConfiguration<'line'> = {
    type: 'line',
    data: this.chartDataTemperature
  }
  temperatureDataEmpty: boolean = false;

  chartDataHumidity: ChartData<'line'> = {
    labels: [],
    datasets: []
  };
  chartConfigurationHumidity: ChartConfiguration<'line'> = {
    type: 'line',
    data: this.chartDataHumidity
  }
  humidityDataEmpty: boolean = false;

  chartDataWind: ChartData<'line'> = {
    labels: [],
    datasets: []
  };
  chartConfigurationWind: ChartConfiguration<'line'> = {
    type: 'line',
    data: this.chartDataWind
  }
  windDataEmpty: boolean = false;

  chartDataSun: ChartData<'line'> = {
    labels: [],
    datasets: []
  };
  chartConfigurationSun: ChartConfiguration<'line'> = {
    type: 'line',
    data: this.chartDataSun
  }
  sunDataEmpty: boolean = false;

  constructor(private sensorService: SensorService, private datePipe: DatePipe) {}

  ngOnInit(): void {
    this.getData();
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

  /* get name depends on unit */
  getName(unit: string){
    let name = '';
    if (unit == 'C') name = "Temperature";
    else if (unit == 'g/m3') name = "Humidity";
    else if (unit == 'W/m2') name = "Sun";
    else if (unit == 'km/h') name = "Wind";
    return name
  }

  /* transform date */
  formatDate(isoDate: string): string {
    const formattedDate = this.datePipe.transform(isoDate, 'yyyy-MM-dd HH:mm:ss');
    return formattedDate ? formattedDate : 'invalid date';
  }

  /* get filtered data */
  applyFilters(): void {
    this.currentPage = 1;
    this.getData();
    if (this.showChart) {
      this.seeCharts();
    }
  }

  /* clear all filters */
  clearFilters(): void {
    this.filter = {
      sensorId: null,
      type: '',
      startDate: '',
      endDate: '',
      sort: '',
    };
    this.currentPage = 1;
    this.getData();
    if (this.showChart) {
      this.seeCharts();
    }
  }

  /* sorting */
  sortByColumn(column: string): void {
    if (this.currentSortColumn === column) {
      this.currentSortOrder = this.currentSortOrder === 'ASCENDING' ? 'DESCENDING' : 'ASCENDING';
    } else {
      this.currentSortColumn = column;
      this.currentSortOrder = 'ASCENDING';
    }

    this.filter.sort = `${this.currentSortColumn}:${this.currentSortOrder}`;
    this.applyFilters();
  }


  /* set chart axis titles */
  setChartAxisTitles(chartConfig: ChartConfiguration<'line'>, xTitle: string, yTitle: string): void {
    chartConfig.options = {
      responsive: true,
        spanGaps: true,
      scales: {
        x: {
          title: {
            display: true,
            text: xTitle,
          },
          ticks: {
            autoSkip: true  ,
          }
        },
        y: {
          title: {
            display: true,
            text: yTitle
          }
        }
      },
      plugins: {
        legend: {
          position: 'top',
        },
      },
    };
  }

  /* get colors for lines in a chart */
  getColorForLine(index: number): string {
    const colors = ['rgb(173, 216, 230)', 'rgb(255, 0, 255)', 'rgb(75, 0, 130)', 'rgb(100, 149, 237)'];
    return colors[index];
  }

  /* set presentation data for showing a chart */
  setChartPresentation(chartData: ChartData,
                       chartConfiguration: ChartConfiguration<'line'>,
                       typeData: Sensor[],
                       index: number,
                       xTitle: string,
                       yTitle: string): void {
    chartData.labels = Array.from(new Set(typeData.map(data => this.formatDate(data.timestamp))));
    let color = 0;

    for (let j = index; j <= 15; j+=4) {
      const specificSensorData = typeData.filter(data => data.sensorId == j);

      if (specificSensorData.length != 0) {
        const datasetData = chartData.labels.map(timestamp => {
          const match = specificSensorData.find(data => this.formatDate(data.timestamp)  === timestamp);
          return match ? match.value : null;
        });

        chartData.datasets.push({
          data: datasetData,
          label: "sensor: " + j,
          borderColor: this.getColorForLine(color),
          fill: false,
        });

        color += 1;
      }
    }
    this.setChartAxisTitles(chartConfiguration, xTitle, yTitle);
  }

  /* clear chart data */
  clearChartPresentation(chartData: ChartData, chartConfiguration: ChartConfiguration) {
    chartData.labels = [];
    chartData.datasets = [];

    chartConfiguration.data = chartData;
    chartConfiguration.options = {};
  }

  /* display chart */
  seeCharts(){
    this.showChart = !this.showChart;
    if (this.showChart) {
      for (let sensorType = 0; sensorType < 4; sensorType++) {
        const typeData = this.sensorData
            .filter(data => data.sensorId % 4 == sensorType)
            .sort((data1, data2) => new Date(data1.timestamp).getTime() - new Date(data2.timestamp).getTime());
        if (sensorType == 0) {
          if (typeData.length == 0) {
            this.temperatureDataEmpty = true;
          }
          else {
            this.setChartPresentation(this.chartDataTemperature, this.chartConfigurationTemperature, typeData, sensorType, "Timestamp", "Temperature [C]");
          }
        }
        else if (sensorType == 1) {
          if (typeData.length == 0) {
            this.humidityDataEmpty = true;
          }
          else {
            this.setChartPresentation(this.chartDataHumidity, this.chartConfigurationHumidity, typeData, sensorType, "Timestamp", "Humidity [g/m3]");
          }
        }
        else if (sensorType == 2) {
          if (typeData.length == 0) {
            this.windDataEmpty = true;
          }
          else {
            this.setChartPresentation(this.chartDataWind, this.chartConfigurationWind, typeData, sensorType, "Timestamp", "Wind [km/h]");
          }
        }
        else if (sensorType == 3) {
          if (typeData.length == 0) {
            this.sunDataEmpty = true;
          }
          else {
            this.setChartPresentation(this.chartDataSun, this.chartConfigurationSun, typeData, sensorType, "Timestamp", "Sun [W/m2]");
          }
        }
      }
    }
    else {
      this.clearChartPresentation(this.chartDataTemperature, this.chartConfigurationTemperature);
      this.clearChartPresentation(this.chartDataHumidity, this.chartConfigurationHumidity);
      this.clearChartPresentation(this.chartDataWind, this.chartConfigurationWind);
      this.clearChartPresentation(this.chartDataSun, this.chartConfigurationSun);
      this.temperatureDataEmpty = false;
      this.humidityDataEmpty = false;
      this.windDataEmpty = false;
      this.sunDataEmpty = false;
    }
  }

  /* change chart button text */
  getChartsButtonText(): string {
    return this.showChart ? 'Hide Charts' : 'See Charts';
  }

  /* verify if there is only one chart to display */
  onlyOneChart() {
    return [this.temperatureDataEmpty, this.humidityDataEmpty, this.windDataEmpty, this.sunDataEmpty].filter(value => !value).length === 1;
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
  protected readonly length = length;
}
