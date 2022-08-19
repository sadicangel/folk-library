import { ItemReadDto } from "./item-read-dto";

export interface ArtistReadDtoBase extends ItemReadDto {
    readonly shortName: string;
    readonly country: string;
    readonly district?: string;
    readonly municipality?: string;
    readonly parish?: string;
}