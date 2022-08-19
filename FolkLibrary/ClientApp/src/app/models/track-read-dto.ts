import { AlbumReadDtoBase } from "./album-read-dto-base";
import { ArtistReadDtoBase } from "./artists-read-dto-base";
import { TrackReadDtoBase } from "./track-read-dto-base";

export interface TrackReadDto extends TrackReadDtoBase {
    readonly album: AlbumReadDtoBase;
    readonly artists: ArtistReadDtoBase[];
}