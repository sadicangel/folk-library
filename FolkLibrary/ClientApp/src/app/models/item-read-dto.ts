import { Genre } from "./genre";

export interface ItemReadDto {
    readonly id: string;
    readonly name: string;
    readonly type: string;
    readonly description: string;
    readonly year?: number;
    readonly isYearUncertain?: boolean;
    readonly genres: Genre[];
}