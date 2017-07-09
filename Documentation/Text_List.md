# TEXT_LIST Element

A `TEXT_LIST` element behaves similarly to a `[SITUATION_TEXT](Situation_Text.md)` element except that it allows a block of formatted text values to be specified for each situation. 

## Properties
This element shares all of the properties of the `TEXT` element except for the `Text` property.

* `EvaOnly` Should this element be used only when there is a Kerbal on EVA. Possible values: True, False, Either. __Default__: Either
* `Random` Should the different text values be chosen in a random order or sequentially? __Default__: false (sequential)
* `ResetOnLaunch` When texts are being chosen in sequence should that sequence be reset when switching to a different vessel? __Default__: false (don't reset)

Any of the situations from the `[SITUATION_TEXT](Situation_Text.md)` element can be used as a block header. As with the `[SITUATION_TEXT](Situation_Text.md)` element the `Default` block will be used if there is no block specified for the current situation. If there is no default block then the element will be blank.

Each situation block takes the same format with the name of the situation acting as a header then a list of formatted text values enclosed between `{` and `}`. e.g.

## Example:	

    TEXT_LIST
	{
		// common properties
		Anchor = 0.0,0.05
		Position = 0.3,0.4
		Size = 0.6,0.24
		Color = 1.0,0,1.0,1.0
		TextAnchor = UpperLeft
		
		// specific
		EvaOnly = false
		Random = false // should the texts in each list be displayed in random order.
		ResetOnLaunch = true
		
		Default
		{
			Text = First message
			Text = <b>Message number two</b>
			Text = Message three
		}
		
		Prelaunch
		{
			Text = Поехали!
			Text = Let's go!
		}
		
		Landed 
		{
			Text = The Eagle has landed - <MET>
			Text = Is it safe now?
		}
	}


--
### [Documentation Index](../README.md)