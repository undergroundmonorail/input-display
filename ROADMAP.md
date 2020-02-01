# Roadmap

These are the plans for what I want to be customizable via .config modification and supplying custom images.

## Visual Customization

### Buttons

- How many buttons are visible ☐
- Position of buttons ☐
- Which hardware button is mapped to which visible button 🗹
- Pressed and unpressed images of buttons 🗹
- Override images for specific buttons ☐
	- e.g. something like this:
	
	![overriding a specific button image with a custom one](https://i.imgur.com/3IvE2Jy.png)

### Stick

- Choose between left analog, right analog or d-pad ☐
- How the stick looks 🗹
- Position of stick ☐
- Hitbox mode ☐
	- Could be implemented by allowing you to turn off stick display and just adding more buttons, I guess
	- Analog directions would have to be made valid button inputs, but I'm already doing something like that for LT and RT

### Window

- Location (support both x/y coords and something like BOTTOM_MIDDLE, TOP_LEFT) ☐
- Width and height ☐
- Background (support both RGBA colour and a supplied image) ☐

## Misc

- Optionally assign "close" keybind to a combination of stick buttons ☐
- Choose an XInput device instead of defaulting to whichever one is considered player 1 ☐
	- "Press Start" prompt if ambiguous, save to config file?
- Keybinds to generate .config file and default images in execution folder ☐
	- This would allow the distribution of just the .exe file. If someone wishes to customize, those default resources could be generated from the program