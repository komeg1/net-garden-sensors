import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SensorsViewComponent } from './sensors-view.component';

describe('SensorsViewComponent', () => {
  let component: SensorsViewComponent;
  let fixture: ComponentFixture<SensorsViewComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SensorsViewComponent]
    });
    fixture = TestBed.createComponent(SensorsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
