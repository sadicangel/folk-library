import { Component, Input, OnInit } from '@angular/core';
import { AlbumReadDto } from '../models/album-read-dto';

@Component({
  selector: 'app-album',
  templateUrl: './album.component.html',
  styleUrls: ['./album.component.scss']
})
export class AlbumComponent implements OnInit {

  @Input() album?: AlbumReadDto;

  constructor() { }

  ngOnInit(): void {
  }

}
