import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SignUpSuccessPage } from './sign-up-success.page';

describe('SignUpSuccessPage', () => {
  let component: SignUpSuccessPage;
  let fixture: ComponentFixture<SignUpSuccessPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(SignUpSuccessPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
