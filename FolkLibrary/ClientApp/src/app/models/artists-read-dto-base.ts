import { ItemReadDto } from "./item-read-dto";

export interface ArtistReadDtoBase extends ItemReadDto {
    readonly year?: number;
    readonly country: string;
    readonly district?: string;
    readonly municipality?: string;
    readonly parish?: string;
}