import { Component, Input, OnInit } from '@angular/core';
import { ArtistReadDto } from '../models/artists-read-dto';

@Component({
  selector: 'app-artist',
  templateUrl: './artist.component.html',
  styleUrls: ['./artist.component.scss']
})
export class ArtistComponent implements OnInit {

  @Input() artist?: ArtistReadDto;

  constructor() { }

  ngOnInit(): void {
  }

  public getLocation(): string | undefined {
    if (!this.artist) {
      return undefined;
    }
    const parts = [];
    if (this.artist.country) {
      parts.push(this.artist.country);
    }
    if (this.artist.district) {
      parts.push(this.artist.district);
    }
    if (this.artist.municipality) {
      parts.push(this.artist.municipality);
    }
    if (this.artist.parish) {
      parts.push(this.artist.parish);
    }

    return parts.join(" - ");
  }

}
