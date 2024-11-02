import { Injectable } from '@angular/core';
import { GroupService } from '../../_services/group/group.service';
import { Group } from 'src/app/_common/models/group';
import { map } from 'rxjs';

@Injectable()
export class GroupDetailsService {
  group?: Group;
  groupId?: string;

  constructor(private readonly groupService: GroupService) {}

  getGroup(id: string) {
    this.groupId = id;
    return this.groupService.getOne(id).pipe(
      map((e) => {
        this.group = e;

        return e;
      }),
    );
  }

  refreshJoinCode(validTo: Date) {
    return this.groupService
      .refreshJoinCode(this.groupId ?? '', validTo)
      .pipe(() => {
        return this.getGroup(this.groupId ?? '');
      });
  }
}
