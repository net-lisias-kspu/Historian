# Inherit Element

An `INHERIT` element will include all the elements from another layout file (unless they are explicitly excluded) into the current layout.

* `LayoutName` The filename of the layout to include. NOTE: this is case sensitive and MUST end in `.layout`. Invalid layout names will result in the `INHERIT` element being ignored.
* `EXCLUDE` A block specifying a list of named elements from the inherited layout that should not be displayed. Unrecognised element names will be ignored. 

Example:

	KSEA_HISTORIAN_LAYOUT
	{
		INHERIT
		{
			// We'll copy from Default.layout
			LayoutName = Default
		
			// We don't want to display the flag or the existing text
			EXCLUDE
			{
				Element = SmallFlag
				Element = DetailText
			}
		}
		
		// Extra Elements to display can be added before or after the INHERIT element
		// (we'll keep it simple with just the vessel name)
		TEXT
		{
			Name = OverrideText
			Anchor = 0.0, 0.5
			Position = 0.05, 0.85
			Size = 0.9, 0.1
			Text = <size=50><b><Vessel></b></size>
		}
	
	}

![](inherit-example.png)

--
### [Documentation Index](../README.md)