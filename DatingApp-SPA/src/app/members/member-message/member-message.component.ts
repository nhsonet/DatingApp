import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from './../../_models/message';
import { AuthService } from './../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { NotificationService } from '../../_services/notification.service';
import { Pagination, PaginatedResult } from './../../_models/pagination';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-message',
  templateUrl: './member-message.component.html',
  styleUrls: ['./member-message.component.css']
})
export class MemberMessageComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private authService: AuthService, private userService: UserService, private notificationService: NotificationService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
    .pipe(
      tap(messages => {
        for (const message of messages) {
          if (message.isRead === false && message.recipientId === currentUserId) {
            this.userService.markMessageAsRead(currentUserId, message.id);
          }
        }

        // for (let i = 0; i < messages.length; i++) {
        //   if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
        //     this.userService.markMessageAsRead(currentUserId, messages[i].id);
        //   }
        // }
      })
    )
    .subscribe(messages => {
      this.messages = messages;
    }, error => {
      this.notificationService.error(error);
    });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe((message: Message) => {
      this.messages.unshift(message);
      this.newMessage.content = '';
    }, error => {
      this.notificationService.error(error);
    })
  }

}
