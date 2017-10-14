# Text Element

A `TEXT` element renders a string of text. The text may contain [Unity formatting codes](http://docs.unity3d.com/Manual/StyledText.html) and [tags](Tags.md) that will be replaced when displayed by the actual values.

* `Text` A formatted text template. _Default: Empty_
* `TextAnchor` Alignment of the text relative to the bounds of the element. Supports any one of these values: `UpperLeft`, `UpperCenter`, `UpperRight`, `MiddleLeft`,`MiddleCenter`, `MiddleRight`, `LowerLeft`, `LowerCenter`, and `LowerRight`. _Default: MiddleCenter_
* `Font` name of an OS font to use for text in the element. NOTE: this value is case sensitive and the name must __EXACTLY__ match the name of a font installed in your operating system (e.g. `Arial`, `Comic Sans MS`, `Times New Roman`). _Default: none._
* `FontSize` Size of the font. Note that rich text format specifiers can override this. _Default: 10_
* `FontStyle` Style of the font. Supports any one of these values: `Normal`, `Bold`, `Italic`, and `BoldAndItalic`. Note that rich text format specifiers can override this. _Default: Normal_
* `Color` Color of the font. Note that rich text format specifiers can override this. _Default: 1.0,1.0,1.0,1.0_
* `TRAIT` a block level property defining how to display crew information for a specific type of crew member (e.g. Pilot)
    Example:
	TRAIT
	{
		Name = Pilot // the name of the trait from the game's config files
		Label = (P) // optional label to display after crew member's name
		Color = red 
	}
	See [Traits.md](Traits.md) for more details
* `TRAITDEFINITIONS` the name of a file inside the layouts folder that contains a list of `TRAIT` blocks (allows easily defining same settings for multiple TEXT type elements) _Example: TRAITDEFINITIONS = colored.traitsconfig_. See [Traits.md](Traits.md) for more details
* `BaseYear` Offset added to year values and formatted dates. _Default: 1 (Kerbin CalendarMode) or 1951 (Earth CalendarMode)_

### Deprecated properties

* `DateFormat` A C# format string for the <Date> element. Example: `dd/MM/yyyy` for UK style short date. _Default: CurrentCulture.LongDatePattern_
* `PilotColor` Unity richtext color to apply to pilot names in <Crew> or <CrewShort> elements. _Default: clear_
* `EngineerColor` Unity richtext color to apply to engineer names in <Crew> or <CrewShort> elements. _Default: clear_
* `ScientistColor` Unity richtext color to apply to scientist names in <Crew> or <CrewShort> elements. _Default: clear_
* `TouristColor` Unity richtext color to apply to tourist names in <Crew> or <CrewShort> elements. _Default: clear_



--
### [Documentation Index](../README.md)