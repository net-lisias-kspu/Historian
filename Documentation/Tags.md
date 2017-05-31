# Formatted Text Tags

The following pre-defined tags can be used inside a text element. These tags will be replaced with their corresponding values when a screenshot is taken.

__Note that all tag names are case-sensitive.__

Tags marked with a † will show localized values.

### Gemeral tags
* `<N>` Inserts a new line.
* `<Custom>` The current value of the Custom Text. You can set this value using the configuration window. If custom text is not persistent (default), it will be cleared after the next screenshot.

### Tags describing the vessel
* `<Vessel>` Name of the active vessel or Kerbal. Example: _Jebediah Kerman_, _Kerbal X_
* `<VesselType>` † the type of the current vessel. Will be one of `Base`, `Debris`, `EVA`, `Flag`, `Lander`, `Probe`, `Rover`, `Ship`, `SpaceObject`, `Station`, `Plane`, `Relay`, `Unknown` 
* `<Target>` † Name of currently targeted vessel or body (if any) 
* `<Crew>` † Name of all the crew members, separated by commas, on the active vessel (Only available in Flight Mode). If the vessel is a probe, it will display "Unmanned". If the vessel is space debris, it will display "N/A". Example: _Jebediah Kerman, Bill Kerman_
* `<StageNumber>` The number of the currently active stage. This will be the same as the number displayed in the staging controls at the bottom left of the flight screen UI.

### Location
* `<Body>` † Name of the main body (Only available in Flight Mode). Example: _Kerbin_ 
* `<Situation>` † Current situation of the active vessel (Only available in Flight Mode). Example: _Flying_, _Orbiting_ 
* `<Biome>` † Current biome of the active vessel based on its location (Only available in Flight Mode). Example: _Shores_ 
* `<LandingZone>` † The name of the current location the vessel is landed at (Only available in Flight Mode). Example: _Launchpad_ 
* `<Latitude>` Latitude of the active vessel relative to the main body expressed in decimal degrees e.g. _23.01245_ (Only available in Flight Mode)
* `<LatitudeDMS>` † Latitude of the active vessel relative to the main body expressed in degrees, minutes and seconds. e.g. _23° 05' 23" N_  (Only available in Flight Mode) 
* `<Longitude>` Longitude of the active vessel relative to the main body expressed in decimal degrees e.g. _-17.15_ (Only available in Flight Mode)
* `<LongitudeDMS>`† Longitude of the active vessel relative to the main body expressed in degrees, minutes and seconds e.g. _17° 21' 10" W_  (Only available in Flight Mode) 
* `<Altitude>` Altitude of the active vessel relative to the sea level of the main body in the most appropriate unit (Only available in Flight Mode). The unit is also included as of version 1.0.1.

### Orbital parameters
* `<Ap>` Apoapsis of current orbit (or sub-orbital trajectory) including unit. 
* `<Pe>` Periapsis of current orbit (or sub-orbital trajectory) including unit. 
* `<Inc>` Inclination of current orbit including `°` symbol.
* `<LAN>` Longtitude of Ascending Node including `°` symbol.
* `<ArgPe>` Argument of Periapsis including `°` symbol.
* `<Ecc>` Eccentricity to 3 decimal places.
* `<Period>` Orbital period.
* `<Orbit>` Shorthand for `<Ap> x <Pe>`: Example 120.7 km x 91.3 km.

### Speed
* `<Mach>` The current Mach number of the active vessel (Only available in Flight Mode).
* `<Speed>` Surface speed of the active vessel in the most appropriate unit (Only available in Flight Mode). The unit is also included as of version 1.0.1.
* `<SrfSpeed>` alias for `<Speed>`
* `<OrbSpeed>` orbital speed of the active vessel in the most appropriate unit (only available in flight mode). The unit is also included.

### Other
* `<Heading>` The current compass heading of the active vessel (Only available in Flight Mode).

### Debugging
The following tags are also available mainly for debug purposes or when developing layouts:

* `<ListFonts>` A (long) comma separated list of the official names of all fonts installed in your OS. For use in identifying the correct name to use with the `Font` property.
* `<LastAction>` The last recognised action. See `ACTION_TEXT` below.
* `<EvaState>` The current animation state of an EVA Kerbal.

### Third party mod integration
* `<LaunchSite>` If [KSCSwitcher](http://forum.kerbalspaceprogram.com/index.php?/topic/106206-105-regexs-useful-mod-emporium/) is installed will display the name of the current active space center (e.g. _Cape Canaveral_). If [Kerbal Konstructs](http://forum.kerbalspaceprogram.com/index.php?/topic/94863-112-kerbal-konstructs-v0967_ex-holy-glowing-balls-batman/) is installed then the launchsite most recently set via the menu the VAB or SPH is used. If neither mod is present then the __Default Space Center Name__ is used.
* `<KK-SpaceCenter> If [Kerbal Konstructs](http://forum.kerbalspaceprogram.com/index.php?/topic/94863-112-kerbal-konstructs-v0967_ex-holy-glowing-balls-batman/) is installed will return the BaseName of the closest open space center. If not installed then `NO KK` will be returned.
* `<KK-Distance>` If [Kerbal Konstructs](http://forum.kerbalspaceprogram.com/index.php?/topic/94863-112-kerbal-konstructs-v0967_ex-holy-glowing-balls-batman/) is installed will return the distance to the closest open space center. If not installed then `NO KK` will be returned.



--
### [Documentation Index](../README.md)