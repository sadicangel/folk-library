import { Genre } from "./genre";

export interface Item {
  id: string;
  name: string;
  type: string;
  description: string;
  year?: number;
  isYearUncertain?: boolean;
  genres: Genre[];
}
