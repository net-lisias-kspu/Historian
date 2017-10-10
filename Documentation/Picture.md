# Picture Element

A `PICTURE` element renders a static 2D texture onto the screen. To save KSP memory usage, you can use any of the pre-loaded textures, such as missions flags, agency flags, or even part textures. You can also use any other custom texture as long as the path to your image is valid. Vintage border effects, anyone? ;)

* `Texture` Path to the image file, relative to the `GameData` directory. Example: `Squad/Flags/default`
* `Scale` Scale of the image relative to itself. For example, a value of `2.0,2.0` doubles the size of the texture, while maintaining the aspect ratio. _Default: 1.0,1.0_

If a `Size` property is not defined (or if the size is a zero vector), the size of the image is used automatically. Otherwise it denotes  the size of the image relative to screen dimensions. For example, a value of `1.0,1.0` ensures the image takes up the size of the entire screen. _Default: 0.0,0.0_

--
### [Documentation Index](../README.md)