import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContainerdataComponent } from './containerdata.component';

describe('ContainerdataComponent', () => {
  let component: ContainerdataComponent;
  let fixture: ComponentFixture<ContainerdataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContainerdataComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ContainerdataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
