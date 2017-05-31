# Layout Elements

The following element types are currently supported by Historian:

## Decorative elements
* `[FLAG](Flag.md)` Renders the current mission's flag onto the screen.
* `[PICTURE](Picture.md)` Renders a 2D image onto the screen.
* `[RECTANGLE](Rectangle.md)` Draws a simple block of color on the screen. Often used as a background for one of the other elements.

## Information elements
* `[TEXT](Text.md)` Draws some text on the screen. Supports rich text and special tags that display current in-game values. 

The other information elements automate the selection of one of several text elements to display.

* `[ACTION_TEXT](Action_text.md)` Selects a different text to display based on a recent action - staging, abort or an action group button press.
* `[SITUATION_TEXT](Situation_Text.md)` Selects a different text to display based on flight situation - flying, orbiting, landed etc.
* `[TEXT_LIST](Text_List.md)` - Selects a different text to display from a list of possible options either randomly or in sequence. Different lists can be specified for different situations

## Miscellaneous
* `[INHERIT](Inherit.md)` - Combine another layout with the current one.

--
### [Documentation Index](../README.md)