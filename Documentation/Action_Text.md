# Action Text Element

An `ACTION_TEXT` element behaves similarly to a `SITUATION_TEXT` except that the selected text is used only if the corresponding action has occurred recently (how long is considered recent can be configured from the settings window). It has all of its properties except `Text`. Instead, it has the following additional properties, each corresponding to a different action:

* `Default` Used whenever no recent action has taken place. If omitted then this element will normally be blank.
* `Abort` Used if the abort key (`backspace` by default) or abort button has been pressed recently.
* `Stage` Used if the stage key (`spacebar` by default) has been pressed recently.
* `AG1`, `AG2`,... `AG10`: Used if the corresponding action group key (`1`, `2`,... `0` by default) has been pressed recently.

Note that just like `TEXT`, `ACTION_TEXT` also supports rich text and placeholder values.

--
### [Documentation Index](../README.md)