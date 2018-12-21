import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() user: User; // because this is a child of MemberListComponent, want to pass User down to this to display on card
  constructor(private authService: AuthService,
    private userService: UserService,
    private aslertifyService: AlertifyService) { }

  ngOnInit() {
  }

  sendLike(id: number) {
    this.userService.likeUser(this.authService.decodedToken.nameid, id).subscribe(data => {
      this.aslertifyService.success('You have liked ' + this.user.knownAs);
    }, error => {
      this.aslertifyService.error(error);
    });
  }

}
