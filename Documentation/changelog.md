# Changelog

#### 1.3.1 fpr KSP 1.3.1
* Recompiled for KSP 1.3.1
* Add text tags:
    * `<Mass>` - total mass of active vessel in kg
	* `<Cost>` - total cost of active vessel
	* `<PartCount>` - number of parts in active vessel

#### 1.3 for KSP 1.3 (Dev build)
* Recompiled for KSP 1.3 - __NOT compatible with KSP 1.2.2__

__BREAKING CHANGE__
* Move Historian install folder from `GameData/KSEA/Historian` to `GameData/Historian`. If upgrading from Historian version 1.2.8 or earlier then any custom layouts must be copied to the new location and the old folder deleted.

* Localisation support 
	* where stock game provides localised values these will be displayed by `TEXT` tags. e.g. planet names, biomes, vessel types etc.
	* the file `GameData/Historian/Localisation.cfg` contains all the non-stock text strings used by Historian. This can be used to localise these values also (I don't know any of the other languages well enough to attempt this myself)
	* Date and Time formatting is delegated to the ingame default dateTimeFormatter allowing standard localised Kerbin and Earth calendar dates (as well as UT and MET) to be displayed
* Parsing of `TEXT` (and associated) blocks occurs only once when the layout is loaded rather than on every draw event as in previous versions. A parsed expression tree is cached and executed on each draw instead. This should improve both memory usage and performance in all but trivial layouts.
* Improvements to tag parser and evaluator to allow tags to accept parameters
	* `<Date(format)>` override the default dateTimeFormatter using a standard .NET date/time formatting string e.g. *<Date(dd/MM/yyyy)>*. See [documentation](Tags-Date.md) for more details.
	* `<Crew(isList, isShort, showSuffix, traits)>` replaces the existing `<Crew>`, `<CrewShort>`, `<CrewList>`, `<Pilots>`, `<PilotsShort>` etc. with a single configurable tag (previous tags are retained for backwards compatibility but are now deprecated and support may be dropped in a future version).
		* `isList` - if true displays crew names in a verticle bulleted list. If false names are displayed inline separated by commas
		* `isShort` - if true the surname (Kerman or localised equivalent) is displayed only once at end of list.  __This setting is ignored if `isList=true`.__
		* `showSuffix` - if true then a trait abbreviation will be shown after each name e.g. *Jebediah Kerman (P), Bill Kerman (E)*. The abbreviations can be defined in the new [`TRAIT`](Traits.md) block or [`TRAITDEFINITIONS`](Traits.md) file.
		* `traits` - a comma separated list of which types of crew should be listed or ALL to list everyone
		* Example: `<Crew(false, true, true, Pilots, Scouts)>` will be a comma separated list of the first names of all piots or scouts on the current vessel. See [the documentation](Tags-Crew.md) for full details and more examples.
* The `TEXT` element property `DateFormat` is now deprecated in favour of the parameterised `<Date>` tag described above
* The `TEXT` element properties `PilotColor`, `ScientistColor`, `EngineerColor` and `TouristColor` are now deprecated in favour of using [`TRAIT`](Traits.md) or [`TRAITDEFINITIONS`](Traits.md). See [documentation](Traits.md) for details.
* Fix: -ve values were always displayed in base units instead of most appropriate unit (i.e. -99999m instead of -99.999km)
* Revised and reformatted [documentation](../README.md)
* Additional [example layouts](Example.md)

#### Dev Build 1.2.9
* Updated `SITUATION_TEXT`, `ACTION_TEXT` and `TEXT_LIST` to use new tokenized string loader and evaluator
* Update source files with consistent licecnse header
* Moved readme and changelog into Historian folder rather than root of archive

__BREAKING CHANGE__
* Move Historian install folder from `GameData/KSEA/Historian` to `GameData/Historian` - any custom layouts must be copied to the new location and the old folder deleted.


#### Dev build 1.2.8
* Refactor text parsing to move tokenizer into layout load rather than OnDraw should improve performance of layouts with large number of tags substantially with no penalty to simple layouts
* Add unit tests for text parser tokenizing and reconstituting
* Add initial support for parameterized tags (e.g. `Crew(Medics)`) - actual tags still to be implemented
* Add `TRAIT { Name Suffix Color }` block level node on TEXT element definition for specifying trait names, colours and abbreviations (to support for custom traits e.g. from USI MKS) - legacy PilotColor etc. still supported

__KNOWN ISSUES__
* Tokenized parsing not yet implemented for layout blocks inheriting from TEXT so they currently show blank

#### 1.2.7 for KSP 1.2.2
* Fix issue #10 where configuration dialog would not show correctly if settings.cfg file had to be created during start-up (e.g. after first installing mod and add action to Visual Studio build sequence to automatically delete settings.cfg so I don't overlook this breaking in the future
*  Add configuration option to allow setting text to be shown for empty crew slots i.e. in `pilot`, `scientist`, `engineer` (and short or list variants) tags when no crew of that type are present in vessel. *Default = None (same as previous version)*
* Add configuration option to allow setting text to be shown in `Crew`, `CrewShort` or `CrewList` tags for a vessel with no crew. *Default = unmanned (same as previous version)*.
* Fix #13 Out by one error when displaying Kerbin formatted dates.
* Modify text parsing routines to remove many temporary strings and to use cached stringbuilder if available. Should reduce garbage collection footprint.


#### 1.2.6
* Recompile for 1.2.1
* Update toolbar wrapper to latest version for compatibility with Contract Configurator
* Not tested against Kerbal Konstructs or KSCSwitcher as no 1.2 compatible versions exist at this time

#### 1.2.5.1
* Add setting to configure Blizzy's Toolbar right click action to toggle either "Suppressed", "Always Active" or "Auto Hide UI"

#### 1.2.5 for KSP 1.1.22
* add `Name` property to all elements
* add new `INHERIT` element.
	* required `LayoutName` property to specify layout to inherit from (e.g. `LayoutName = test.layout`). NOTE: the layout file name is case sensitive
	* optional `EXCLUDE` block with list of named `Element` blocks from inherited layout to exclude from combined layout ([Example](https://github.com/Aelfhe1m/Historian-Expanded/blob/master/GameData/KSEA/Historian/Layouts/inherit_C.layout)).


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
