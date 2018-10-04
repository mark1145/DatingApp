import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) { }

  ngOnInit() {
    const token = localStorage.getItem('tokenStr');
    // need to do this here because the value is only being populated when we login, when we refresh the page it's gone
    this.authService.decodedToken = this.jwtHelper.decodeToken(token);
  }
}
