# Crew Traits

A configuration block defining information about displaying a `<Crew>` tag will be displayed in a text element. In KSP crew have "traits" - i.e. Pilot, Scientist, Engineer, Tourist which define what skills they have. Some mods (e.g. USI MKS) also define extra traits.


#### TRAIT definition blocks

TRAIT blocks can be defined directly inside a text element or in a separate file linked with a `TRAITDEFINITIONS` property (see below). Each trait is defined using a `TRAIT` block.:

    TRAIT
	{
		Name = Kolonist
		DisplayName = MKS Kolonist
		Suffix = (K)
		Color = orange
	}

* `Name` the un-localised name of the trait (i.e. Pilot, Scientist, Engineer, Tourist or mod added trait names)
* `DisplayName` how you want the trait name to be displayed in `<Crew>` tags and ROSTER elements when verbose display is enabled (NOTE: not in current version - future expansion)
* `Suffix` a short label to display after the name to show after the name to distinguish the different crew types. This is enabled by the `showSuffix` parameter of the `<Crew>` tag. Example: __Use `<Crew(false, false, true, ALL)>` to show `Jebediah Kerman (P), Bill Kerman (E), Bob Kerman (S)`__
* `Color` a Unity rich text color to use to display crew with different trait types in different colors. Default is `clear`

If the same trait is defined multiple times then the last definition will be the one used.

#### TRAITDEFINITIONS property

Text elements can contain a `TRAITDEFINITIONS` property that links to a separate file containing a list of trait blocks (example: [Layouts/colouredTest.traitsconfig](../GameData/Historian/Layouts/colouredTest.traitsconfig)).

This file is a simple text file containing one or more `TRAIT` blocks as defined above. The file can have any extension but I would recommend not using .layout or .cfg to avoid confusion with layout and KSP configuration files (suggested extension `.traits` or `.traitsConfig`).

#### Example:

    TEXT
	{
        // [snip] various normal text properties
        
        // provide link to your default traits definition file
        TRAITDEFINITIONS = colouredTest.traitsconfig
        
        // add another trait or override the defaults for this text element
        TRAIT
        {
            Name = Kolonist
            DisplayName = MKS Kolonist
            Suffix = (K)
            Color = orange
        }
        
    }


--
### [Documentation Index](../README.md)