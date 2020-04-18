import { environment } from './../../../environments/environment';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from './../../_models/photo';
import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { NotificationService } from 'src/app/_services/notification.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getChangedMemberPhotoUrl = new EventEmitter<string>();

  uploader: FileUploader;
  hasBaseDropZoneOver = false;

  baseUrl = environment.apiUrl;
  currentMainPhoto: Photo;

  constructor(private authService: AuthService, private userService: UserService, private notificationService: NotificationService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);

        const photo = {
          id: res.id,
          url: res.url,
          createdAt: res.createdAt,
          description: res.description,
          isMain: res.isMain
        };

        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
      this.currentMainPhoto.isMain = false;
      photo.isMain = true;

      // this.getChangedMemberPhotoUrl.emit(photo.url);
      this.authService.changeMemberPhoto(photo.url);
      this.authService.currentUser.photoUrl = photo.url;

      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    }, error => {
      this.notificationService.error(error);
    });
  }

  deletePhoto(id: number) {
    this.notificationService.confirm('Are you sure you want to delete this photo?', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.notificationService.success('Photo has been deleted.');
      }, error => {
        this.notificationService.error('Failed to delete the photo.');
      });
    });
  }

}
