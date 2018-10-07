import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() user: User; // because this is a child of MemberListComponent, want to pass User down to this to display on card
  constructor() { }

  ngOnInit() {
  }

}
