import { Album } from "./album";
import { Artist } from "./artist";
import { Genre } from "./genre";
import { Item } from "./item";

export interface Track extends Item {
  number: number;
  duration: string;
  album: Album[];
  artists: Artist[];
}
