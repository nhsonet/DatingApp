import { map } from 'rxjs/operators';
import { Component, OnInit } from '@angular/core';
import { User } from './../../_models/user';
import { AdminService } from './../../_services/admin.service';
import { NotificationService } from 'src/app/_services/notification.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal/ngx-bootstrap-modal';
import { RoleModalComponent } from '../role-modal/role-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[];
  bsModalRef: BsModalRef;

  constructor(private modalService: BsModalService,
              private adminService: AdminService, private notificationService: NotificationService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe((users: User[]) => {
      this.users = users;
    }, error => {
      this.notificationService.error(error);
    });
  }

  modalRoleEdit(user: User) {
    const initialState = {
      user,
      roles: this.getRoles(user)
    };

    this.bsModalRef = this.modalService.show(RoleModalComponent, { initialState });
    this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
      const rolesToUpdate = {
        roleNames: [...values.filter(element => element.checked === true).map(element => element.name)]
      };

      if (rolesToUpdate) {
        this.adminService.updateUserRoles(user, rolesToUpdate).subscribe(() => {
          user.roles = [...rolesToUpdate.roleNames];
        }, error => {
          this.notificationService.error(error);
        })
      }
    });
  }

  private getRoles(user) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      { name: 'Admin', value: 'Admin' },
      { name: 'Moderator', value: 'Moderator' },
      { name: 'Member', value: 'Member' },
      { name: 'VIP', value: 'VIP' }
    ];

    for (let i = 0; i < availableRoles.length; i++) {
      let isMatched = false;
      for (let j = 0; j < userRoles.length; j++) {
        if (availableRoles[i].name === userRoles[j]) {
          isMatched = true;
          availableRoles[i].checked = true;

          roles.push(availableRoles[i]);
          break;
        }
      }

      if (!isMatched) {
        availableRoles[i].checked = true;

        roles.push(availableRoles[i]);
      }
    }

    return roles;
  }

}
