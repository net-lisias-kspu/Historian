# TEXT_LIST Element

A `TEXT_LIST` element behaves similarly to a `SITUATION_TEXT` element except that it allows a block of alternative `TEXT` values to be specified for each situation. The element has the can specify the same properties as a `TEXT` element with the addition of:

* `EvaOnly` Should this element be used only when there is a Kerbal on EVA. Possible values: True, False, Either. __Default__: Either
* `Random` Should the different text values be chosen in a random order or sequentially? __Default__: false
* `ResetOnLaunch` When texts are being chosen in sequence should that sequence be reset when switching to a different vessel? __Default__: false

Each situation block takes the same format with the name of the situation acting as a header then a list of `TEXT` entries enclosed between `{` and `}`. e.g.

    Prelaunch
	{
	    Text = ???????!
		Text = Let's go!
	}
	
Any of the situations from the `SITUATION_TEXT` element can be used as a block header. As with the `SITUATION_TEXT` element the `Default` block will be used if there is no block specified for the current situation.

--
### [Documentation Index](../README.md)