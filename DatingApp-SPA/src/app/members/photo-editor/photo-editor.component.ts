import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from '../../_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  // output properties emit events; there are 4 parts
  // 1. having the output property like below
  // 2. calling this.outputProperty.emit(x);
  // 3. the attribute in parent html; (getMemberPhotoChange)="methodInParentComponent($event)"
  // 4. Method in Parent component with what you want to do when event triggers
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMainPhoto: Photo;

  constructor(private authService: AuthService, private userService: UserService, private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.initialiseUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initialiseUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('tokenStr'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024 // 10 mb
    });

    // Tell uploader our file is not going with credentials, fix for CORS error
    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };

        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(() => {
        this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
        this.currentMainPhoto.isMain = false;
        photo.isMain = true;
        this.authService.changeMemberPhoto(photo.url);
        this.authService.currentUser.photoUrl = photo.url;
        localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      }, error => {
        this.alertifyService.error(error);
      });
  }

  deletePhoto(photoId: number) {
    this.alertifyService.confirm('Are you sure you want to delete?',
    () => { this.userService.deletePhoto(this.authService.decodedToken.nameid, photoId)
            .subscribe(() => {
              this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1);
              this.alertifyService.success('Photo deleted');
             }, error => {
               this.alertifyService.error(error);
             });
          });
  }
}
