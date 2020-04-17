import { User } from './../_models/user';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

// const httpOptions = {
//   headers: new HttpHeaders({
//     'Authorization': 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getUsers(): Observable<User[]> {
    // return this.httpClient.get<User[]>(this.baseUrl + 'users', httpOptions);
    return this.httpClient.get<User[]>(this.baseUrl + 'users');
  }

  getUser(id): Observable<User> {
    // return this.httpClient.get<User>(this.baseUrl + 'users/' + id, httpOptions);
    return this.httpClient.get<User>(this.baseUrl + 'users/' + id);
  }

}
