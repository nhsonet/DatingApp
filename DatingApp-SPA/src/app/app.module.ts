import { AuthService } from './_services/auth.service';
import { AdminService } from './_services/admin.service';
import { NotificationService } from 'src/app/_services/notification.service';
import { UserService } from './_services/user.service';
import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';

import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
// import { NgxGalleryModule } from 'ngx-gallery';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeagoModule } from 'ngx-timeago';

import { appRoutes } from './routes';
import { AuthGuard } from './_guard/auth.guard';
import { PreventUnsavedChangesGuard } from './_guard/prevent-unsaved-changes.guard';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { LikeListResolver } from './_resolvers/like-list.resolver';
import { MessageResolver } from './_resolvers/message.resolver';

import { AppComponent } from './app.component';
import { ValueComponent } from './value/value.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { LikeListComponent } from './like-list/like-list.component';
import { MessageComponent } from './message/message.component';
import { MemberMessageComponent } from './members/member-message/member-message.component';

import { HasRoleDirective } from './_directives/hasRole.directive';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { RoleModalComponent } from './admin/role-modal/role-modal.component';

export function tokenGetter() {
   return localStorage.getItem('token');
}

// export class CustomHammerConfig extends HammerGestureConfig {
//    overrides = {
//       pinch: { enable: false },
//       rotate: { enable: false }
//    };
// }

@NgModule({
   declarations: [
      AppComponent,
      ValueComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      MemberListComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      PhotoEditorComponent,
      LikeListComponent,
      MessageComponent,
      MemberMessageComponent,
      HasRoleDirective,
      AdminPanelComponent,
      UserManagementComponent,
      PhotoManagementComponent,
      RoleModalComponent
   ],
   imports: [
      BrowserModule,
      BrowserAnimationsModule,
      HttpClientModule,
      RouterModule.forRoot(appRoutes),
      FormsModule,
      ReactiveFormsModule,
      ModalModule.forRoot(),
      PaginationModule.forRoot(),
      ButtonsModule.forRoot(),
      BsDatepickerModule.forRoot(),
      BsDropdownModule.forRoot(),
      TabsModule.forRoot(),
      NgxGalleryModule,
      FileUploadModule,
      TimeagoModule.forRoot(),
      JwtModule.forRoot({
         config: {
            tokenGetter: tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/api/auth']
         }
      })
   ],
   providers: [
      AuthService,
      AdminService,
      NotificationService,
      UserService,
      AuthGuard,
      PreventUnsavedChangesGuard,
      ErrorInterceptorProvider,
      MemberListResolver,
      MemberDetailResolver,
      MemberEditResolver,
      LikeListResolver,
      MessageResolver
      // { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig }
   ],
   entryComponents: [
      RoleModalComponent
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
