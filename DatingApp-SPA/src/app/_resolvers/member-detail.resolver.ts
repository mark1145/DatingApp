import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

// Angular Router uses Angular dependency injection to access resolvers, so we have to make sure we register
// MemberDetailResolver with Angular’s dependency injection system by adding it to the providers property
// in @NgModule metadata:
@Injectable()
export class MemberDetailResolver implements Resolve<User> {
    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) { }

    // The resolve method returns an observable of the type User, so Angular Router
    // will wait for the observable to complete before the route’s component is activated.
    // This is then stored in route.data['user'] where it can be accessed in the component by
    // injecting the ActivatedRoute or ActivatedRouteSnapshot into the constructor of the component
    // https://www.sitepoint.com/component-routing-angular-router/
    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(route.params['id']).pipe( // resolves automatically subscribe
            catchError(error => {
                this.alertify.error('Problem retrieveing data');
                this.router.navigate(['/members']);
                return of(null); // this is how we return a null observable
            })
        );
    }
}
