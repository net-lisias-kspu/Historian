# Flag Element

A `FLAG` element can be used to render the current mission's flag onto the screen. It behaves very much like a `PICTURE` otherwise.

* `DefaultTexture` Path to a default image file to show if no flag can be determined for the active vessel, or if there is no active vessel. Example: `Squad/Flags/default`
* `BackgroundTexture` Optional path to a image file to show as background behind transparent flags. Default is none. Example: `Squad/Flags/Minimalistic`
* `Scale` Scale of the image relative to itself. For example, a value of `2.0,2.0` doubles the size of the texture, while maintaining the aspect ratio. _Default: 1.0,1.0_

If a `Size` property is not defined (or if the size is a zero vector), the size of the image is used automatically. Otherwise it denotes  the size of the image relative to screen dimensions. For example, a value of `1.0,1.0` ensures the image takes up the size of the entire screen. _Default: 0.0,0.0_

--
### [Documentation Index](../README.md)