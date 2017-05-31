# Situation Text Element

A `SITUATION_TEXT` element behaves similar to the `TEXT` element. It has all of its properties except `Text`. Instead, it has the following additional properties, each corresponding to a different flight situation:

* `EvaOnly` Should this element be used only when there is a Kerbal on EVA. Possible values: True, False, Either. __Default__: Either

* `Default` Used when no flight situation is available.
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

When a screen shot is taken, the `SITUATION_TEXT` element uses only one of the above values for its text, depending on the situation. This is useful for making more descriptive captions such as: `Preparing to launch from <LaunchSite>` or `Landed on <Body>'s <LandingZone>` or `Flying at Mach <Mach> (<Speed>) <Altitude> over <Body>'s <Biome>`.

Note that just like `TEXT`, `SITUATION_TEXT` also supports rich text and placeholder values.

Note also that the special EVA values `RagDolled`, `Clambering` and `OnLadder` take precedence over the flight situations if they have a value text specified. If no value is specified then the appropriate normal situation is used (e.g. `Flying`, `Landed` etc.)

--
### [Documentation Index](../README.md)