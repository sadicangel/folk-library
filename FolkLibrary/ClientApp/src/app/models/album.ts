import { Artist } from "./artist";
import { Genre } from "./genre";
import { Item } from "./item";
import { Track } from "./track";

export interface Album extends Item {
  year?: number;
  trackCount: number;
  duration: string;
  artists: Artist[];
  genres: Genre[];
  tracks: Track[];
}
