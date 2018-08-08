# Historian-Expanded :: Change log

* 2016-0528: 1.2.4.12 (Aelfhe1m) for KSP 1.1.2
	+ 1.2.4.12 for KSP 1.1.2
		- Add settings window option to Auto Hide UI when taking screen shot. (Caution do not use this with other mods that also auto-hide the UI)
		- Extend maximum configurable setting for `Time to remember action` to 10s following additional play testing.
		- Fix [issue #5](https://github.com/Aelfhe1m/Historian-Expanded/issues/5) - Action text persists longer than configured
		- Fix [issue #6](https://github.com/Aelfhe1m/Historian-Expanded/issues/6) - TEXT_LIST generating ArgumentOutOfRange exceptions just after lift-off
* 2016-0527: 1.2.4 (Aelfhe1m) for KSP 1.1.2
	+ Add new elements:
		- ACTION_TEXT: conditional message when abort, stage or action group pressed
		- TEXT_LIST: random or sequential list of messages to display based on situation
	+ Added `EvaOnly` property toggle to `SITUATION_TEXT` and `TEXT_LIST`
	+ Added `Clambering`, `OnLadder` and `RagDolled` situations to `SITUATION_TEXT` and `TEXT_LIST`.
	+ Add settings window fields to customise Kerbal day and month names.
	+ Add settings window slider for `Time to remember action`
* 2016-0516: 1.2.3.1 (Aelfhe1m) for KSP 1.1.2
	+ Revert persistence model to config file format and set save path to `GameData\KSEA\Historian\Plugins\PluginData`
	+ Fix: BUG - multiple layouts overlaying each other
	+ Fix: BUG - semi-transparent rectangle elements becoming increasingly opaque over multiple screenshots
	+ Fix: potential problem with configuration window id clashing with window ids from other mods.
* 2016-0510: 1.2.2.11 (Aelfhe1m) for KSP 1.1.2
	+ Change persistence model for configuration settings to use game save files rather than a config file in GameData. NOTE: Starting a new save will set configuration to default values. Reverting a flight or quickloading may load an older version of the configuration if you've made recent changes.
* 2016-0509: 1.2.1.14 (Aelfhe1m) for KSP 1.1.2
	+ Regression fix for `DateFormat` - same issue as with crew trait colouring
* 2016-0509: 1.2.1.9 (Aelfhe1m) for KSP 1.1.2
	+ Add text node parameters:
		- `VesselType` - the type of vessel e.g. probe, debris, lander etc.
		- `RealDate` - the real world date and time formatted using the block level DateFormat setting
	+ Support for [Kerbal Konstructs](http://forum.kerbalspaceprogram.com/index.php?/topic/94863-112-kerbal-konstructs-v0967_ex-holy-glowing-balls-batman/) and [Kerbin Side](http://forum.kerbalspaceprogram.com/index.php?/topic/74776-112-kerbin-side-v110-supplements/)
		- `LaunchSite` modified to detect most recently set KK launch site
		- `KK-SpaceCenter` - the closest detected "space center"
		- `KK-Distance` - the straight line distance from the closest detected "space center"
	+ Setting added to main configuration dialog for "Default space center name". Will replace "KSC" in `LaunchSite` or `KK-SpaceCenter` parameter results. **NOTE**: only replaces the value if "KSC" is the full return value. "KSC Launch pad", "Kerbal Space Center" or similar results will **NOT** be affected
	+ `TEXT` and `SITUATION_TEXT` blocks can now specify a `Font` property for the name of the OS font to use to display the block. **NOTE**: font name must _EXACTLY_ match OS font name. A utility parameter `ListFonts` is supplied to list the names of all installed fonts.
		- Historian.cfg" moved to `<KSP>/GameData/KSEA/Historian/Plugins` folder to avoid interfering with Module Manager caching.
	+ Current version number displayed in settings window title bar
	+ General code clean-up and refactoring
* 2016-0503: 1.2.0.41 (Aelfhe1m) for KSP 1.1.2
	+ compile against KSP 1.1.2 binaries
	+ update documentation
	+ master branch updated to KSP 1.1.2
* 2016-0407: 1.2.0.40 (Aelfhe1m) for KSP 1.1 Pre
	+ Fix: regression fix for missing "Flying" situation trigger.
* 2016-0407: 1.2.0.39 (Aelfhe1m) for KSP 1.1 Pre
	+ Fix: Compatibility for Automatic Screenshots and Saves
* 2016-0406: 1.2.0.38 (Aelfhe1m) for KSP 1.0.5
	+ Fix: remove debug message from crew names.
* 2016-0406: 1.2.0.37 (Aelfhe1m) for KSP 1.0.5
	+ Fix: crew trait colours not showing correctly
	+ Fix: `<UT>` incorrectly included base year now shows first year as Y1 etc.
* 2016-0405: 1.2.0 (Aelfhe1m) for KSP 1.0.5
	+ Initial KSP 1.1 release
* 2016-0407: 1.1.18.1 (Aelfhe1m) for KSP 1.0.5
	+ Fix: regression fix for missing "Flying" situation trigger.
* 2016-0407: 1.1.8 (Aelfhe1m) for KSP 1.0.5
	+ Fix: compatibility with Automated Screenshots and Saves
* 2016-0329: 1.1.7a (Aelfhe1m) for KSP 1.3.1 PRE-RELEASE
	+ Earth dates can be specified in two formats. "Normal" fixed length 365d year used by game engine or "KAC compatible" variable length years. If using variable length years, the in-game day 1 of a year may not be 1 January.
	+ Add text node parameters for date support:
		- DateKAC - formatted date calculated based on number of seconds (UT) since base start date. This date includes leap years.
		- YearKAC - the current year of the variable length date. At start and end of year may not match the Yx Dx display shown in game.
		- DayKAC - the current day of the variable length date. At start and end of year may not match the Yx Dx display shown in game. Some years will have 366 days.
		- MET - an alias for T+. The mission ellapsed time.
	+ Fix: MET was incorrectly calculated for periods > 1 day.
	+ Fix: MET always showed Kerbin based periods even when calendarmode is set to Earth.
	+ 1.1.6 - unreleased
		- Add text node parameters:
			- Target - name of currently targeted vessel or body (if any)
			- LaunchSite - shows 'KSC' unless KSCSwitcher is installed then it will show the active launch site's name (e.g. Cape Canaveral)
	+ 1.1.5a -unreleased
		- minor refactoring of situation text.
	+ 1.1.4a - unreleased
		- refactor `TEXT` node parsing
		- Add text node parameters:
			- LatitudeDMS (degrees, minutes, seconds) e.g. 2° 5' 33" N
			- LongitudeDMS (degrees, minutes, seconds) e,g 72° 12' 40" W
			- SrfSpeed - surface speed (same as Speed)
			- OrbSpeed - orbital speed
		- `Longitude` and `LongitudeDMS` parameters always reports value between -180° (W) and +180° (E)
		- Optional `BackgroundTexture` parameter for flag nodes. Allows adding a background texture to transparent flags. e.g. `BackgroundTexture = Squad/Flags/minimalistic` (simple "rippled" white flag background)
* 2016-0113: 1.1.3 (Aelfhe1m) for KSP 1.3.1
	+ Change log:
		- Fix Pe not substituted
		- Added Kerbin date formatting (alternating 35 and 36 day months). Names fixed currently but will be configurable in later version.
		- All crew related parameters will be empty string if Kerbal on EVA.
* 2015-0805: 1.1.1 (Zeenobit) for KSP 1.0.4 PRE-RELEASE
	+ Features
		- Contains the required compatibility for linuxgurugamer's Automated Screenshots mod (implemented by linuxgurugamer)
	+ Note: This is a very minor update and required only if you plan on using the Automated Screenshots mod.
* 2015-0627: 1.1.0 (Zeenobit) for KSP 1.0.4
	+ Features
		- Added option to Suppress Historian with the right mouse click on the Toolbar button (Only supported on Blizzy's Toolbar)
		- Recompiled for KSP 1.0.4
* 2015-0602: 1.0.2 (Zeenobit) for KSP 0.90
	+ Features
		- `<Crew>` placeholder value
		- Option to turn off the persistent configuration window
	+ Fixes
		- Fixed issue with empty configuration window shown when game was paused with it open
		- Potential fix for "Persistent Custom Text" option not being saved
		- Fixed problems with `<LandingZone>` not showing correct values in a few rare locations
* 2015-0526: 1.0.1 (Zeenobit) for KSP 0.90
	+ Fixes
		- `<LandingZone>` should no longer show up as an empty value when landing on actual biomes
		- `<Altitude>` and `<Speed>` now convert the value to the proper unit instead of always using meters and showing huge values. The unit will be displayed as part of the value.
* 2015-0524: 1.0 (Zeenobit) for KSP 0.90
	+ Features
		- New placeholder values `<Mach>`, `<LandingZone>`, `<Speed>`, and `<Custom>`
		- Support for custom text to be displayed
		- `SITUATION_TEXT` element to display different text values depending on current vessel situation
	+ Fixes
		- Fixed the missing name for Blizzy's Toolbar
		- Adjusted the precision of some placeholder values to make them look nicer
* 2015-0522: 0.2 (Zeenobit) for KSP 0.90 PRE-RELEASE
	+ Features
		- Layout support
		- Runtime switching between layouts
		- Saving default layout
		- Support for Blizzy's Toolbar
* 2015-0519: 0.1 (Zeenobit) for KSP 0.90 PRE-RELEASE
	+ Features
		- Fully configurable layout for the screenshot overlay
		- Ability to reload the layout configuration while the game is running
		- Automatic activation when taking screenshots
		- Suppression toggle to disable automatic activation when required
