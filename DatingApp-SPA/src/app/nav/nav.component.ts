import { Component, OnInit } from '@angular/core';
import { AuthService } from './../_services/auth.service';
import { NotificationService } from '../_services/notification.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(private router: Router, public authService: AuthService, private notificationService: NotificationService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.notificationService.success('Logged in successfully.');
      this.router.navigate(['/members']);
    }, error => {
      this.notificationService.error(error);
    });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.notificationService.message('logged out.');
    this.router.navigate(['/home']);
  }

}
