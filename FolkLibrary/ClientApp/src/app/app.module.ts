import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AlbumComponent } from './album/album.component';
import { AlbumListComponent } from './album/album-list.component';
import { CommonModule } from '@angular/common';
import { ArtistComponent } from './artist/artist.component';
import { ArtistListComponent } from './artist/artist-list.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AlbumComponent,
    AlbumListComponent,
    ArtistComponent,
    ArtistListComponent
  ],
  imports: [
    CommonModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'artists', component: ArtistListComponent },
      { path: 'albums', component: AlbumListComponent },
    ]),
    BrowserAnimationsModule,
    MatExpansionModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
