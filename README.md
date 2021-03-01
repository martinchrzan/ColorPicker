# ColorPicker
## Windows system-wide color picker

Simple and quick system-wide color picker. Pick colors from any currently running application.
## Now also part of [**PowerToys**](https://github.com/microsoft/PowerToys)! 

**To open Color Picker** - (default shortcut) - *Left Ctrl + Break*

**To pick a color** - *Left mouse click* - copies selected color into the clipboard in the selected format (rgb/hex/hsl/hsv)

**To zoom in** - *Mouse wheel*

**To open Colors History Palette** - *Right mouse click while Color Picker opened* - holds up to 10 previously copied colors

[**Download the latest release here**](https://github.com/martinchrzan/ColorPicker/releases/latest)

![](showcase.gif)

![](ColorsHistory.gif)


Currently supported color formats and their string representation:
- **HEX** - *#292929*
- **RGB** - *rgb(31,31,31)*
- **HSL** - *hsl(0, 0%, 12%)* 
- **HSV** - *hsv(0, 0, 12)*
- **VEC4** - *vec4(0.122, 0.122, 0.122, 1)*

## Differences from the implementation in PowerToys
- different set of color formats, vec4 is only here, however there are some missing
- quick colors history palette
- different design
- no telemetry
- lightweight

