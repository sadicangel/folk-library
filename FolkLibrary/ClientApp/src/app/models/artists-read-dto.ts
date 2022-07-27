import { AlbumReadDtoBase } from "./album-read-dto-base";
import { ArtistReadDtoBase } from "./artists-read-dto-base";

export interface ArtistReadDto extends ArtistReadDtoBase {
    readonly albums: AlbumReadDtoBase[];
}