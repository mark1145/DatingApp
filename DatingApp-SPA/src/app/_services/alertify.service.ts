import { Injectable } from '@angular/core';
declare let alertify: any; // else tslint will give errors

@Injectable({
  providedIn: 'root'
})
export class AlertifyService { // wrapper around alertify

constructor() { }

  confirm(message: string, okCallBack: () => any) {
    alertify.confirm(message, function(e) { // function(e) is user clicking button
      if (e) {
        okCallBack();
      } else { }

    });
  }

  success(message: string) {
    alertify.success(message);
  }

  error(message: string) {
    alertify.error(message);
  }

  warning(message: string) {
    alertify.warning(message);
  }

  message(message: string) {
    alertify.message(message);
  }
}
