import { ItemReadDto } from "./item-read-dto";

export interface AlbumReadDtoBase extends ItemReadDto {
    readonly trackCount: number;
    readonly duration: string;
    readonly isIncomplete?: boolean;
}