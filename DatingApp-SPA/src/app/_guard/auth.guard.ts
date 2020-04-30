import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from './../_services/auth.service';
import { NotificationService } from '../_services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, public authService: AuthService, private notificationService: NotificationService) { }

  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data['roles'] as Array<string>;

    if (roles) {
      const isMatched = this.authService.isRoleMatched(roles);
      if (isMatched) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.notificationService.error('You are not authorized to access this area.');
      }
    }

    if (this.authService.loggedIn()) {
      return true;
    }

    this.notificationService.error('Route guard cant be bypassed.');
    this.router.navigate(['/home']);
    return false;
  }
}
