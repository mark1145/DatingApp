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

    // General Routing Flow.
    // 1) User clicks the link.
    // 2) Angular loads the respective component.
    // Routing Flow with Resolver
    // 1) User clicks the link.
    // 2) Angular executes certain code and returns a value or observable.
    // 3) You can collect the returned value or observable in constructor or in ngOnInit, in class of your component which is about to load.
    // 4) Use the collected the data for your purpose.
    // 5) Now you can load your component.
    //
    // Steps 2,3 and 4 are done with a code called Resolver.
    // So basically resolver is that intermediate code, which can be executed when a link has been clicked and before a component is loaded.

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
