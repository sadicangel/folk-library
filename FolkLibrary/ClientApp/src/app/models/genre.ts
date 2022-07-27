import { Album } from "./album";
import { Item } from "./item";
import { Track } from "./track";

export interface Genre extends Item {
  albums: Album[];
  tracks: Track[];
}
