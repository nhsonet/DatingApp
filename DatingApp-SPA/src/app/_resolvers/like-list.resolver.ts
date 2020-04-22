import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { NotificationService } from '../_services/notification.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class LikeListResolver implements Resolve<User[]> {
    pageNumber = 1;
    pageSize = 5;
    likeParam = 'Likers';

    constructor(private router: Router, private userService: UserService, private notificationService: NotificationService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likeParam).pipe(
            catchError(error => {
                this.notificationService.error('Problem retrieving data.');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
    
}