import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { ArtistReadDto } from '../models/artists-read-dto';

@Component({
  selector: 'app-artist-list',
  templateUrl: './artist-list.component.html',
  styleUrls: ['./artist-list.component.css']
})
export class ArtistListComponent implements OnInit {

  public artists: ArtistReadDto[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<ArtistReadDto[]>(baseUrl + 'api/artist').subscribe({
      next: result => this.artists = result.sort((a, b) => a.name.localeCompare(b.name, 'pt-PT')),
      error: error => console.error(error)
    });
  }

  ngOnInit(): void {
  }

}
