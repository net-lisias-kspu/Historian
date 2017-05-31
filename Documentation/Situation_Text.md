# Situation Text Element

Chooses one of several formatted TEXT values to display based on the currently focussed vessels status.  This is useful for making more descriptive captions such as: `Preparing to launch from <LaunchSite>` or `Landed on <Body>'s <LandingZone>` or `Flying at Mach <Mach> (<Speed>) <Altitude> over <Body>'s <Biome>`.

## Properties
This element shares all of the properties of the `TEXT` element except for the `Text` property.

* `EvaOnly` Should this element be displayed only when there is a Kerbal on EVA. Possible values: True, False, Either. __Default__: Either

## Text elements
When a screen shot is taken, one of these properties will be evaluated and displayed based on the vessels status. These are all optional and if the appropriate one for the vessels status is not defined then `Default` will be used instead.
* `Default` 
* `Landed` Used when the vessel is landed.
* `Splashed` Used when the vessel is splashed in water.
* `Prelaunch` Used when the vessel is on the launchpad.
* `Flying` Used when the vessel is flying in atmosphere.
* `SubOrbital` Used when the vessel is in a sub-orbital trajectory.
* `Orbiting` Used when the vessel is orbiting a body.
* `Escaping` Used when the vessel is escaping from a body.
* `Docked` Used when the vessel is docked to another.

* `RagDolled` Used when an EVA Kerbal is 'ragdolled' on the ground as the result of a fall or collision
* `Clambering` Used when an EVA Kerbal is climbing over terrain or a vehicle in response to the 'F' climb key.
* `OnLadder` Used when an EVA Kerbal is holding on to a ladder

Note also that the special EVA values `RagDolled`, `Clambering` and `OnLadder` take precedence over the flight situations if they have a value specified. If no value is specified then the appropriate normal situation is used (e.g. `Flying`, `Landed` etc.)

--
### [Documentation Index](../README.md)