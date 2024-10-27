import { Component, OnInit } from '@angular/core';
import { GroupService } from '../_services/group/group.service';
import { Observable } from 'rxjs';
import { Group } from '../_common/models/group';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.page.html',
  styleUrls: ['./groups.page.scss'],
})
export class GroupsPage implements OnInit {
  protected groups$: Observable<Group[]>;

  constructor(private readonly groupService: GroupService) {
    this.groups$ = this.groupService.get();
  }

  ngOnInit() {}
}
