import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';

// Angular routing works on first match wins, therefore ordering is important
export const appRoutes: Routes = [
    { path: '', component: HomeComponent }, // route for nothing and home
    {
        path: '', // blank so that we can do like; localhost:4200/members
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent,
                resolve: { users: MemberListResolver } },
            { path: 'members/:id', component: MemberDetailComponent,
                resolve: { user: MemberDetailResolver } }, // this is how we access data from the routes
            { path: 'messages', component: MessagesComponent },
            { path: 'lists', component: ListsComponent },
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' }, // if nothing matches; go back to home
];
