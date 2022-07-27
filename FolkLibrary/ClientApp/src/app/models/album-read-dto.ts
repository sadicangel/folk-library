import { AlbumReadDtoBase } from "./album-read-dto-base";
import { ArtistReadDtoBase } from "./artists-read-dto-base";
import { ItemReadDto } from "./item-read-dto";
import { TrackReadDtoBase } from "./track-read-dto-base";

export interface AlbumReadDto extends AlbumReadDtoBase {
    readonly artists: ArtistReadDtoBase[];
    readonly genres: ItemReadDto[];
    readonly tracks: TrackReadDtoBase[];
}