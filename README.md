# ColorPicker
## Windows system-wide color picker

Simple and quick system-wide color picker. Pick colors from any currently running application.
## Now also part of [**PowerToys**](https://github.com/microsoft/PowerToys)! 

**To open Color Picker** - (default shortcut) - *Left Ctrl + Break*

**To pick a color** - *Left mouse click* - copies selected color into the clipboard in the selected format (rgb/hex/hsl/hsv)

**To zoom in** - *Mouse wheel*

**To open Colors History Palette** - *Right mouse click while Color Picker opened* - holds up to 10 previously copied colors

**To get an average color in a selected area** - *Left and hold mouse click while Color Picker opened* - select the area - color is copied on the left mouse button released

[**Download the latest release here**](https://github.com/martinchrzan/ColorPicker/releases/latest)

**Color picking**
![](showcase.gif)


**Color history**
![](ColorsHistory.gif)


**Average color of the area**
![](colorMeter.gif)

Currently supported color formats and their string representation:
- **HEX** - *#292929*
- **RGB** - *rgb(31,31,31)*
- **HSL** - *hsl(0, 0%, 12%)* 
- **HSV** - *hsv(0, 0, 12)*
- **VEC4** - *vec4(0.122, 0.122, 0.122, 1)*
- **RGB565** - *#4C8A*
- **DecimalBE (Big-endian)** - *2114460*
- **DecimalLE (Little-endian)** - *1213756*

## Differences from the implementation in PowerToys
- different set of supported color formats
- color meter feature (average color of an area)
- quick colors history palette
- different design
- no telemetry
- lightweight

