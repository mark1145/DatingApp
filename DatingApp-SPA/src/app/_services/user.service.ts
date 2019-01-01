import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';
import { UserParams } from '../_models/userParams';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsers(page?, itemsPerPage?, userParams?: UserParams, likesParams?): Observable<PaginatedResult<User[]>> {
    const paginatedResults: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (likesParams === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParams === 'Likees') {
      params = params.append('likees', 'true');
    }

    if (userParams != null) {
      if (userParams.minAge != null) {
        params = params.append('minAge', userParams.minAge.toString());
      }
      if (userParams.maxAge != null) {
        params = params.append('maxAge', userParams.maxAge.toString());
      }
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(map(response => {
        paginatedResults.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResults.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResults;
       }));
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, photoId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + photoId + '/setMain', {});
  }

  deletePhoto(userId: number, photoId: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + photoId);
  }

  getLikedUsers(userId: number) {
  }

  likeUser(userId: number, likeeId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/like/' + likeeId, {});
  }

  getMessages(userId: number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<Message[]>(this.baseUrl + 'users/' + userId + '/messages', { observe: 'response', params })
    .pipe(map(response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') !== null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }

      return paginatedResult;
    })
    );
  }

  getMessageThread(id: Number, recipientId: Number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: Number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(id: Number, userId: Number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id, {});
  }

  markAsRead(userId: Number, messageIds: Number[]) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/read', { messageIds });
  }
}
