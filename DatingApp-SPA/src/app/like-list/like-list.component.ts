import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from '../_models/user';
import { AuthService } from './../_services/auth.service';
import { UserService } from './../_services/user.service';
import { NotificationService } from 'src/app/_services/notification.service';
import { Pagination, PaginatedResult } from './../_models/pagination';

@Component({
  selector: 'app-like-list',
  templateUrl: './like-list.component.html',
  styleUrls: ['./like-list.component.css']
})
export class LikeListComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likeParam: string;

  constructor(private authService: AuthService, private userService: UserService, private notificationService: NotificationService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.likeParam = 'Likers';
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likeParam)
    .subscribe((result: PaginatedResult<User[]>) => {
      this.users = result.result;
      this.pagination = result.pagination;
    }, error => {
      this.notificationService.error(error);
    });
  }

}
