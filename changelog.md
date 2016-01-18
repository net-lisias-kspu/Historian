# Changelog

#### 1.1.4

* refactor `TEXT` node parsing
* Add text node parameters:
  * LatitudeDMS (degrees, minutes, seconds) e.g. 2° 5' 33" N 
  * LongitudeDMS (degrees, minutes, seconds) e,g 72° 12' 40" W
  * SrfSpeed - surface speed (same as Speed)
  * OrbSpeed - orbital speed
* `Longitude` and `LongitudeDMS` parameters always reports value between -180° (W) and +180° (E)
* Optional `BackgroundTexture` parameter for flag nodes. Allows adding a background texture to transparent flags. e.g. `BackgroundTexture = Squad/Flags/minimalistic` (simple "rippled" white flag background)