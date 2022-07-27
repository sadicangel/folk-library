import { ItemReadDto } from "./item-read-dto";

export interface TrackReadDtoBase extends ItemReadDto {
    readonly number: number;
    readonly duration: string;
    readonly year?: number;
}