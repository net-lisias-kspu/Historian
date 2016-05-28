# Changelog

#### 1.2.4.12 for KSP 1.1.2
* Add settings window option to Auto Hide UI when taking screen shot. (Caution do not use this with other mods that also auto-hide the UI)
* Extend maximum configurable setting for `Time to remember action` to 10s following additional play testing.
* Fix [issue #5](https://github.com/Aelfhe1m/Historian-Expanded/issues/5) - Action text persists longer than configured
* Fix [issue #6](https://github.com/Aelfhe1m/Historian-Expanded/issues/6) - TEXT_LIST generating ArgumentOutOfRange exceptions just after lift-off

#### 1.2.4 for KSP 1.1.2
* Add new elements:
	* ACTION_TEXT: conditional message when abort, stage or action group pressed
	* TEXT_LIST: random or sequential list of messages to display based on situation
* Added `EvaOnly` property toggle to `SITUATION_TEXT` and `TEXT_LIST`
* Added `Clambering`, `OnLadder` and `RagDolled` situations to `SITUATION_TEXT` and `TEXT_LIST`.
* Add settings window fields to customise Kerbal day and month names.
* Add settings window slider for `Time to remember action`

#### 1.2.3.43 - dev build
* Changed `Climbing` to `OnLadder` and `Clambering`

#### 1.2.3.41 - dev build
* Settings window field for customising time to 'remember' action key

#### 1.2.3.34 - dev build
* Added `EvaOnly`, `Climbing` and `RagDolled` situation switches
* Settings window fields for customising Kerbal day and month names.

#### 1.2.3.24 - dev build
* Fix toggling of editor menu display when both toolbar and app launcher button are used.
* Clean up debug log messages

#### 1.2.3.22 - dev build
* TEXT_LIST and ACTION_TEXT situation aware

#### 1.2.3.8 - dev build
* Add new element TEXT_LIST

#### 1.2.3.2 - dev build
* Add new element ACTION_TEXT

#### 1.2.3.1 for KSP 1.1.2
* Revert persistence model to config file format and set save path to `GameData\KSEA\Historian\Plugins\PluginData`
* Fix: BUG - multiple layouts overlaying each other
* Fix: BUG - semi-transparent rectangle elements becoming increasingly opaque over multiple screenshots
* Fix: potential problem with configuration window id clashing with window ids from other mods.

#### 1.2.2.11 for KSP 1.1.2
* Change persistence model for configuration settings to use game save files rather than a config file in GameData. NOTE: Starting a new save will set configuration to default values. Reverting a flight or quickloading may load an older version of the configuration if you've made recent changes.

#### 1.2.1.14 for KSP 1.1.2
* Regression fix for `DateFormat` - same issue as with crew trait colouring

#### 1.2.1.9 for KSP 1.1.2
* Add text node parameters:
	* `VesselType` - the type of vessel e.g. probe, debris, lander etc.
	* `RealDate` - the real world date and time formatted using the block level DateFormat setting
* Support for Kerbal Konstructs](http://forum.kerbalspaceprogram.com/index.php?/topic/94863-112-kerbal-konstructs-v0967_ex-holy-glowing-balls-batman/) and [Kerbin Side](http://forum.kerbalspaceprogram.com/index.php?/topic/74776-112-kerbin-side-v110-supplements/)
	* `LaunchSite` modified to detect most recently set KK launch site
	* `KK-SpaceCenter` - the closest detected "space center"
	* `KK-Distance` - the straight line distance from the closest detected "space center"
* Setting added to main configuration dialog for "Default space center name". Will replace "KSC" in `LaunchSite` or `KK-SpaceCenter` parameter results. NOTE: only replaces the value if "KSC" is the full return value. "KSC Launch pad", "Kerbal Space Center" or similar results will NOT be affected
* `TEXT` and `SITUATION_TEXT` blocks can now specify a `Font` property for the name of the OS font to use to display the block. NOTE: font name must EXACTLY match OS font name. A utility parameter `ListFonts` is supplied to list the names of all installed fonts.
* "Historian.cfg" moved to `GameData\KSEA\Historian\Plugins` folder to avoid interfering with Module Manager caching.
* Current version number displayed in settings window title bar
* General code clean-up and refactoring

#### 1.2.0.41 for KSP 1.1.2
* Compile against 1.1.2 KSP.

#### 1.2.0.40 for KSP 1.1 Pre
* Fix: regression fix for missing "Flying" situation trigger.

#### 1.1.8.1 for KSP 1.0.5a
* Fix: regression fix for missing "Flying" situation trigger.

#### 1.2.0.39 for KSP 1.1 Pre
* Fix: Compatibility for Automatic Screenshots and Saves

#### 1.1.8 for KSP 1.0.5
* Fix: Compatibility for Automatic Screenshots and Saves

#### 1.2.0.38 for KSP 1.1 Pre - bug fixed
* Fix: remove debug message from crew names.

#### 1.2.0.37 for KSP 1.1 Pre - bug fixes
* Fix: crew trait colours not showing correctly
* Fix: `<UT>` incorrectly included base year now shows first year as Y1 etc.

#### 1.2.0 for KSP 1.1 Pre
* initial recompile for KSP 1.1 Pre

#### 1.1.7a
* Earth dates can be specified in two formats. "Normal" fixed length 365d year used by game engine or "KAC compatible" variable length years. If using variable length years, the in-game day 1 of a year may not be 1 January.
* Add text node parameters for date support:
  * DateKAC - formatted date calculated based on number of seconds (UT) since base start date. This date includes leap years.
  * YearKAC - the current year of the variable length date. At start and end of year may not match the Yx Dx display shown in game.
  * DayKAC - the current day of the variable length date. At start and end of year may not match the Yx Dx display shown in game. Some years will have 366 days.
  * MET - an alias for T+. The mission ellapsed time.
* Fix: MET was incorrectly calculated for periods > 1 day.
* Fix: MET always showed Kerbin based periods even when calendarmode is set to Earth.

#### 1.1.6a
* Add text node parameters:
  * Target - name of currently targeted vessel or body (if any)
  * LaunchSite - shows 'KSC' unless KSCSwitcher is installed then it will show the active launch site's name (e.g. Cape Canaveral)
  
#### 1.1.5a - not released
* minor refactoring of situation text.

#### 1.1.4

* refactor `TEXT` node parsing
* Add text node parameters:
  * LatitudeDMS (degrees, minutes, seconds) e.g. 2° 5' 33" N 
  * LongitudeDMS (degrees, minutes, seconds) e,g 72° 12' 40" W
  * SrfSpeed - surface speed (same as Speed)
  * OrbSpeed - orbital speed
* `Longitude` and `LongitudeDMS` parameters always reports value between -180° (W) and +180° (E)
* Optional `BackgroundTexture` parameter for flag nodes. Allows adding a background texture to transparent flags. e.g. `BackgroundTexture = Squad/Flags/minimalistic` (simple "rippled" white flag background)
