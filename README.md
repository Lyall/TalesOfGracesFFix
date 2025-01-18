# Tales of Graces f Remastered Fix
[![Patreon-Button](https://raw.githubusercontent.com/Lyall/TalesOfGracesFFix/refs/heads/master/.github/Patreon-Button.png)](https://www.patreon.com/Wintermance) 
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/W7W01UAI9)<br />
[![Github All Releases](https://img.shields.io/github/downloads/Lyall/TalesOfGracesFFix/total.svg)](https://github.com/Lyall/TalesOfGracesFFix/releases)

This is a BepInEx plugin for Tales of Graces f Remastered that aims to add ultrawide/narrower support and more.

## Features
- Ultrawide and narrower aspect ratio support.
- Enable MSAA.
- Disable depth of field.

## Installation
- Grab the latest release of TalesOfGracesFFix from [here.](https://github.com/Lyall/TalesOfGracesFFix/releases)
- Extract the contents of the release zip in to the the game folder. <br />
(e.g. "**steamapps\common\Tales of Graces f Remastered**" for Steam).

### Steam Deck/Linux Additional Instructions
🚩**You do not need to do this if you are using Windows!**
- Open up the game properties in Steam and add `WINEDLLOVERRIDES="winhttp=n,b" %command%` to the launch options.

## Configuration
- See **`GameFolder`\BepInEx\config\TalesOfGracesFFix.cfg** (after launching the game at least once) to adjust settings for the plugin.

## Known Issues
Please report any issues you see.
This list will contain bugs which may or may not be fixed.

- Screen capture textures (such as the pause transition) are squashed when using an ultrawide/narrower display.
- Enabling MSAA can cause visual issues with the map screen.

## Screenshots

| ![ezgif-4-73b55bf57a](https://github.com/user-attachments/assets/27422730-2bd5-4eb1-9f92-caa8f6e7c9ea) |
|:--:|
| Gameplay |

## Credits
[BepinEx](https://github.com/BepInEx/BepInEx) for plugin loading.
