import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from 'src/app/_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getUsersWithRoles() {
    return this.httpClient.get(this.baseUrl + 'admin/usersWithRoles');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.httpClient.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
  }

  getPhotosForModeration() {
    return this.httpClient.get(this.baseUrl + 'admin/getPhotosForModeration');
  }

  approvePhoto(photoId) {
    return this.httpClient.get(this.baseUrl + 'admin/approvePhoto' + photoId, {});
  }

  rejectPhoto(photoId) {
    return this.httpClient.get(this.baseUrl + 'admin/rejectPhoto' + photoId, {});
  }

}
