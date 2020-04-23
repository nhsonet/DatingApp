import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from './../_models/message';
import { AuthService } from './../_services/auth.service';
import { UserService } from '../_services/user.service';
import { NotificationService } from '../_services/notification.service';
import { Pagination, PaginatedResult } from './../_models/pagination';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  constructor(private authService: AuthService, private userService: UserService, private notificationService: NotificationService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

  loadMessages() {
    this.userService.getMessages(this.authService.decodedToken.nameid,
                                 this.pagination.currentPage, this.pagination.itemsPerPage, this.messageContainer)
    .subscribe((result: PaginatedResult<Message[]>) => {
      this.messages = result.result;
      this.pagination = result.pagination;
    }, error => {
      this.notificationService.error(error);
    });
  }

}
