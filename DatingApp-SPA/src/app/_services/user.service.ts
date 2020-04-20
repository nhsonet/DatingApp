import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PaginatedResult } from './../_models/pagination';
import { User } from './../_models/user';

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

  getUsers(currentPage?, itemsPerPage?): Observable<PaginatedResult<User[]>> {
    // return this.httpClient.get<User[]>(this.baseUrl + 'users', httpOptions);
    // return this.httpClient.get<User[]>(this.baseUrl + 'users');

    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let httpParams = new HttpParams();

    if (currentPage != null && itemsPerPage != null) {
      httpParams = httpParams.append('pageNumber', currentPage);
      httpParams = httpParams.append('pageSize', itemsPerPage);
    }

    return this.httpClient.get<User[]>(this.baseUrl + 'users', { observe: 'response', httpParams })
                          .pipe(
                            map(response => {
                              paginatedResult.result = response.body;

                              if (response.headers.get('Pagination') != null) {
                                paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
                              }

                              return paginatedResult;
                            })
                          );
  }

  getUser(id: number): Observable<User> {
    // return this.httpClient.get<User>(this.baseUrl + 'users/' + id, httpOptions);
    return this.httpClient.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.httpClient.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this.httpClient.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMainPhoto', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.httpClient.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

}
