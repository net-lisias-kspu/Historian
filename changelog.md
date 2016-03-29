# Changelog

#### 1.1.7a
* Earth dates can be specified in two formats. "Normal" fixed length 365d year used by game engine or "KAC compatible" variable length years. If using variable length years, the in-game day 1 of a year may not be 1 January.
* Add text node parameters for date support:
  * DateKAC - formatted date calculated based on number of seconds (UT) since base start date. This date includes leap years.
  * YearKAC - the current year of the variable length date. At start and end of year may not match the Yx Dx display shown in game.
  * DayKAC - the current day of the variable length date. At start and end of year may not match the Yx Dx display shown in game. Some years will have 366 days.  

#### 1.1.6a
* Add text node parameters:
  * Target - name of currently targeted vessel or body (if any)
  * LaunchSite - shows 'KSC' unless KSCSwitcher is installed then it will show the active launch site's name (e.g. Cape Canaveral)
  
#### 1.1.5a
* minor bug fixes.

#### 1.1.4

* refactor `TEXT` node parsing
* Add text node parameters:
  * LatitudeDMS (degrees, minutes, seconds) e.g. 2° 5' 33" N 
  * LongitudeDMS (degrees, minutes, seconds) e,g 72° 12' 40" W
  * SrfSpeed - surface speed (same as Speed)
  * OrbSpeed - orbital speed
* `Longitude` and `LongitudeDMS` parameters always reports value between -180° (W) and +180° (E)
* Optional `BackgroundTexture` parameter for flag nodes. Allows adding a background texture to transparent flags. e.g. `BackgroundTexture = Squad/Flags/minimalistic` (simple "rippled" white flag background)
