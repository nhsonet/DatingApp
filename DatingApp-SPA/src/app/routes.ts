import { MessageComponent } from './message/message.component';
import { LikeListComponent } from './like-list/like-list.component';
import { MemberListComponent } from './member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';
import { AuthGuard } from './_guard/auth.guard';

export const appRoutes: Routes = [
    // { path: '', component: HomeComponent },
    // {
    //     path: '',
    //     runGuardsAndResolvers: 'always',
    //     canActivate: [AuthGuard],
    //     children: [
    //         { path: 'members', component: MemberListComponent },
    //         { path: 'likes', component: LikeListComponent },
    //         { path: 'messages', component: MessageComponent }
    //     ]
    // },
    // { path: '**', redirectTo: '', pathMatch: 'full' }

    { path: 'home', component: HomeComponent },
    { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] },
    { path: 'likes', component: LikeListComponent, canActivate: [AuthGuard] },
    { path: 'messages', component: MessageComponent, canActivate: [AuthGuard] },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];
