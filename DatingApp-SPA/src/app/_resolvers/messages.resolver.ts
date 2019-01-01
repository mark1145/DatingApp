import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';
import { MessagesToViewEnum } from '../enums/MessagesToViewEnum.enum';

@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
    pageNumber = 1;
    pageSize = 5;
    messageContainer = MessagesToViewEnum.Unread;

    constructor(private userService: UserService,
                private authService: AuthService,
                private router: Router,
                private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
        return this.userService.getMessages(
            this.authService.decodedToken.nameid, this.pageNumber, this.pageSize, this.messageContainer) // resolves automatically subscribe
            .pipe(
            catchError(error => {
                this.alertify.error('Problem retrieveing data');
                this.router.navigate(['/home']);
                return of(null); // this is how we return a null observable
            })
        );
    }
}
