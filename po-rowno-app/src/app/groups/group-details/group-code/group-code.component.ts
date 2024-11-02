import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { GroupDetailsService } from '../group-details.service';
import { addWeeks } from 'date-fns';
import { Subject, take, takeUntil } from 'rxjs';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'app-group-code',
  templateUrl: './group-code.component.html',
  styleUrls: ['./group-code.component.scss'],
})
export class GroupCodeComponent implements OnInit, OnDestroy {
  protected destroy$: Subject<void> = new Subject();

  protected code: string = '';
  protected validTo?: Date;

  constructor(
    private readonly groupService: GroupDetailsService,
    private readonly modalCtrl: ModalController,
  ) {}

  ngOnInit() {
    this.code = this.groupService.group?.joinCode ?? '';
    this.validTo = new Date(this.groupService.group?.joinCodeValidTo + '');

    if (
      !this.code.length ||
      (this.validTo?.getTime() ?? 0) <= new Date().getTime()
    ) {
      this.refreshCode();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  cancel() {
    this.modalCtrl.dismiss();
  }

  copyCode() {
    navigator.clipboard.writeText(this.code);
  }

  private refreshCode() {
    this.groupService
      .refreshJoinCode(addWeeks(new Date(), 2))
      .pipe(takeUntil(this.destroy$))
      .subscribe((e) => {
        this.code = e.joinCode;
        this.validTo = e.joinCodeValidTo;
      });
  }
}
