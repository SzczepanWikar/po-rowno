import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GroupDetailsPage } from './group-details.page';

describe('GroupDetailsPage', () => {
  let component: GroupDetailsPage;
  let fixture: ComponentFixture<GroupDetailsPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupDetailsPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
