import { Artist } from "./artist";
import { Item } from "./item";
import { Track } from "./track";

export interface Album extends Item {
  trackCount: number;
  duration: string;
  isIncomplete?: boolean;
  artists: Artist[];
  tracks: Track[];
}
