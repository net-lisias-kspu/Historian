# The Crew Tag

Used to display a list of crew manning the current vessel

* `<Crew(isList, isShort, showSuffix, traits)>`
    * `isList` - if true displays crew names in a verticle bulleted list. If false names are displayed inline separated by commas
    * `isShort` - if true the surname (Kerman or localised equivalent) is displayed only once at end of list.  __This setting is ignored if `isList=true`.__
    * `showSuffix` - if true then a trait abbreviation will be shown after each name e.g. *Jebediah Kerman (P), Bill Kerman (E). The abbreviations can be defined in the new [`TRAIT`](Traits.md) block or [`TRAITDEFINITIONS`](Traits.md) file.
    * `traits` - a comma separated list of which types of crew should be listed or ALL to list everyone

### Notes
* `<Crew>` or `<Crew()>` is equivalent to `<Crew(false, false, false, ALL)>` which was the pre-Historian 1.3.0 behaviour of the `<Crew>` tag.
* The tag __MUST__ be used with either no parameters or the full set.


### Deprecated

As of Historian version 1.3.0 the following crew related tags are deprecated in favour of the simple <Crew()> tag described above.

* `<CrewShort>` Equivalent to `<Crew(false, true, false, ALL)>`
* `<Pilots>` Equivalent to `<Crew(false, false, false, Pilots)>`
* `<Engineers>` Equivalent to `<Crew(false, false, false, Engineers)>`
* `<Scientists>` Equivalent to `<Crew(false, false, false, Scientists)>`
* `<Tourists>` Equivalent to `<Crew(false, false, false, Tourists)>`
* `<CrewList>` Equivalent to `<Crew(true, false, false, ALL)>`

* `<PilotsList>, <EngineersList>, <ScientistsList>, <ToursistsList>` Equivalent to `<Crew(true, false, false, Pilots>` etc.
* `<PilotsShort>, <EngineersShort>, <ScientistsShort>, <TouristsShort>` Equivalent to `<Crew(false, true, false, Pilots>` etc.


--
### [Documentation Index](../README.md)
