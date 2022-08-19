import { SelectionModel } from '@angular/cdk/collections';
import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatSort, SortDirection } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ArtistReadDto } from '../models/artists-read-dto';

type Header = 'name' | 'location' | 'albums' | 'year';

@Component({
  selector: 'app-artist-list',
  templateUrl: './artist-list.component.html',
  styleUrls: ['./artist-list.component.scss']
})
export class ArtistListComponent implements OnInit, AfterViewInit {

  @ViewChild(MatSort) sort: MatSort = new MatSort();
  artists!: MatTableDataSource<ArtistReadDto>;
  public readonly selection: SelectionModel<ArtistReadDto> = new SelectionModel<ArtistReadDto>(false, []);
  public readonly displayedColumns: Header[] = ["name", "location", "albums", "year"];

  public readonly comparers: Record<Header, (a: ArtistReadDto, b: ArtistReadDto) => number> = {
    name: (a: ArtistReadDto, b: ArtistReadDto) => a.shortName.localeCompare(b.shortName, 'pt-PT'),
    location: (a: ArtistReadDto, b: ArtistReadDto) => {
      let result = a.country.localeCompare(b.country);
      if (result !== 0)
        return result;
      if (typeof a.district !== 'string')
        result = typeof b.district !== 'string' ? 0 : 1;
      else
        result = typeof b.district !== 'string' ? -1 : a.district.localeCompare(b.district);
      if (result !== 0)
        return result;
      if (typeof a.municipality !== 'string')
        result = typeof b.municipality !== 'string' ? 0 : 1;
      else
        result = typeof b.municipality !== 'string' ? -1 : a.municipality.localeCompare(b.municipality);
      if (result !== 0)
        return result;
      if (typeof a.parish !== 'string')
        result = typeof b.parish !== 'string' ? 0 : 1;
      else
        result = typeof b.parish !== 'string' ? -1 : a.parish.localeCompare(b.parish);
      return result;
    },
    albums: (a: ArtistReadDto, b: ArtistReadDto) => Math.sign(a.albums.length - b.albums.length),
    year: (a: ArtistReadDto, b: ArtistReadDto) => {
      if (typeof a.year !== 'number')
        return typeof b.year !== 'number' ? 0 : 1;
      if (typeof b.year !== 'number')
        return -1;
      let result = Math.sign(a.year - b.year);
      return result;
    }
  };

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<ArtistReadDto[]>(baseUrl + 'api/artist').subscribe({
      next: result => {
        this.artists = new MatTableDataSource<ArtistReadDto>(result.sort(this.comparers.year));
        this.artists.sort = this.sort;
        this.artists.sortData = (data: ArtistReadDto[], sort: MatSort): ArtistReadDto[] => {
          const active = sort.active as Header;
          const direction = sort.direction === 'asc' ? 1 : -1;
          const comparer = this.comparers[active] || this.comparers.name;
          let result = 0;
          return data.sort((a, b) => (result = comparer(a, b)) !== 0 ? result * direction : this.comparers.name(a, b));
        };
      },
      error: error => console.error(error)
    });
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
  }

  public getLocation(artist: ArtistReadDto): string | undefined {
    if (!artist) {
      return undefined;
    }
    const parts = [];
    if (artist.country) {
      parts.push(artist.country);
    }
    if (artist.district) {
      parts.push(artist.district);
    }
    if (artist.municipality) {
      parts.push(artist.municipality);
    }
    if (artist.parish) {
      parts.push(artist.parish);
    }

    return parts.join(" - ");
  }

  public sortOrder(): void {
    switch (this.sort.direction) {
      case 'asc':
        this.sort.direction = 'asc';
        break;
      case 'desc':
        this.sort.direction = 'desc';
        break;
      default:
        this.sort.direction = 'asc';
    }
  }
}
