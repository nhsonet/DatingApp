import { Component, OnInit } from '@angular/core';
import { AdminService } from './../../_services/admin.service';
import { NotificationService } from 'src/app/_services/notification.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: any;

  constructor(private adminService: AdminService, private notificationService: NotificationService) { }

  ngOnInit() {
    this.getPhotosForModeration();
  }

  getPhotosForModeration() {
    this.adminService.getPhotosForModeration().subscribe((photos) => {
      this.photos = photos;
    }, error => {
      this.notificationService.error(error);
    });
  }

  approvePhoto(photoId) {
    this.adminService.approvePhoto(photoId).subscribe(() => {
      this.photos.spice(this.photos.findIndex(f => f.id === photoId), 1);
    }, error => {
      this.notificationService.error(error);
    });
  }

  rejectPhoto(photoId) {
    this.adminService.rejectPhoto(photoId).subscribe(() => {
      this.photos.spice(this.photos.findIndex(f => f.id === photoId), 1);
    }, error => {
      this.notificationService.error(error);
    });
  }

}
