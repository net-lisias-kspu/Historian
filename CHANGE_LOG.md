# Historian-Expanded :: Change log

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
