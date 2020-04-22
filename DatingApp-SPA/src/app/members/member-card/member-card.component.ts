import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { NotificationService } from 'src/app/_services/notification.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;

  constructor(private authService: AuthService, private userService: UserService, private notificationService: NotificationService) { }

  ngOnInit() {
  }

  sendLike(id: number) {
    this.userService.sendLike(this.authService.decodedToken.nameid, id).subscribe(data => {
      this.notificationService.success('You have liked ' + this.user.knownAs + '.');
    }, error => {
      this.notificationService.error(error);
    });
  }

}
