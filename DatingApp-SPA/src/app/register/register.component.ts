import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any; // communicate to child (this component)
  @Output() cancelRegister = new EventEmitter(); // communicate to parent (home component)
  user: User;
  registerForm: FormGroup; // https://code.tutsplus.com/tutorials/introduction-to-forms-in-angular-4-reactive-forms--cms-29787
  protected minPwLength = 8;
  protected maxPwLength = 16;
  bsConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService, private alertify: AlertifyService, private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    // The structure of the form model (this.registerForm) and the data model (User) should match
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(this.minPwLength), Validators.maxLength(this.maxPwLength)]],
      confirmPassword: ['', [Validators.required]]
     }, {validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(formGroup: FormGroup) {
    const pw = formGroup.get('password').value;
    const confPw = formGroup.get('confirmPassword').value;
    const errorRet = { 'mismatch': true };

    if (!pw || !confPw) {
      return null;
    }

    return pw === confPw ? null : errorRet;
  }

  isFormControlValid(formControlName: string) {
    if (this.registerForm.get(formControlName).errors
        && this.registerForm.get(formControlName).touched) {
      return true;
    }

    return false;
  }

  isTouchedAndSpecificError(formControlName: string, error: string) {
    if (this.registerForm.get(formControlName).hasError(error)
        && this.registerForm.get(formControlName).touched) {
      return true;
    }

    return false;
  }

  isPasswordsMatching() {
    if (this.registerForm.get('confirmPassword').touched && this.registerForm.hasError('mismatch')) {
      return false;
    }

    return true;
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Registration successful');
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      });
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
