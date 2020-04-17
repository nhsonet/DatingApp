import { MessageComponent } from './message/message.component';
import { LikeListComponent } from './like-list/like-list.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { HomeComponent } from './home/home.component';

import { Routes } from '@angular/router';
import { AuthGuard } from './_guard/auth.guard';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';

export const appRoutes: Routes = [
    // { path: '', component: HomeComponent },
    // {
    //     path: '',
    //     runGuardsAndResolvers: 'always',
    //     canActivate: [AuthGuard],
    //     children: [
    //         { path: 'members', component: MemberListComponent }, resolve: { users: MemberListResolver } },
    //         { path: 'members/:id', component: MemberDetailComponent, resolve: { user: MemberDetailResolver } },
    //         { path: 'likes', component: LikeListComponent },
    //         { path: 'messages', component: MessageComponent }
    //     ]
    // },
    // { path: '**', redirectTo: '', pathMatch: 'full' }

    { path: 'home', component: HomeComponent },
    { path: 'members', component: MemberListComponent, canActivate: [AuthGuard], resolve: { users: MemberListResolver } },
    { path: 'members/:id', component: MemberDetailComponent, canActivate: [AuthGuard], resolve: { user: MemberDetailResolver } },
    { path: 'likes', component: LikeListComponent, canActivate: [AuthGuard] },
    { path: 'messages', component: MessageComponent, canActivate: [AuthGuard] },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];
