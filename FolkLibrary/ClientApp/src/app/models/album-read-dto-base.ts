import { ItemReadDto } from "./item-read-dto";

export interface AlbumReadDtoBase extends ItemReadDto {
    readonly year?: number;
    readonly trackCount: number;
    readonly duration: string;
}