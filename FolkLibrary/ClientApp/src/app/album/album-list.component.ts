import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { AlbumReadDto } from '../models/album-read-dto';

@Component({
  selector: 'app-album-list',
  templateUrl: './album-list.component.html',
  styleUrls: ['./album-list.component.css']
})
export class AlbumListComponent implements OnInit {

  public albums: AlbumReadDto[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<AlbumReadDto[]>(baseUrl + 'api/album').subscribe({
      next: result => {
        this.albums = result.sort((a, b) => a.name.localeCompare(b.name, 'pt-PT'));
        this.albums.forEach(album => album.tracks.sort((a, b) => a.number - b.number));
      },
      error: error => console.error(error)
    });
  }

  ngOnInit(): void {
  }

}
