import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddViewPage } from './add-view.page';

describe('AddViewPage', () => {
  let component: AddViewPage;
  let fixture: ComponentFixture<AddViewPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(AddViewPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
