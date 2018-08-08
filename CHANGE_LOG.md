# Historian-Expanded :: Change log

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
