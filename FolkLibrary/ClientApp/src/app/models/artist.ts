import { Album } from "./album";
import { Item } from "./item";
import { Track } from "./track";

export interface Artist extends Item {
  shortName: string;
  country: string;
  district?: string;
  municipality?: string;
  parish?: string;
  albums: Album[];
  tracks: Track[];
}
