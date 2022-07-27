import { AlbumReadDtoBase } from "./album-read-dto-base";
import { GenreReadDtoBase } from "./genre-read-dto-base";

export interface GenreReadDto extends GenreReadDtoBase {
    readonly albums: AlbumReadDtoBase[];
}