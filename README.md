![](https://i.imgur.com/uWTpvxK.png)
![](https://i.imgur.com/2uwVyDO.gif)

ManiaCheck is an osu!mania plugin for [Naxess' Mapset verifier](https://github.com/Naxesss/MapsetVerifier). This tool is used to add extra checks specific for the osu!mania gamemode. 

> **Disclaimer**: This tool is still in early developement and false positives/negatives are likely to ocurr. For information on how to report existing bugs or request new features, see the [Reporting Bugs](#reporting-bugs) section of this Readme.

## Checks

### Unrankables (Problems)
|  Category |            Check            |
|:---------:|:---------------------------:|
| Compose   | Long note too short (<20ms) |
| Timing    | Unnormalized SVs in EZ/NM   |
| Compose   | Stacked notes               |
| Compose   | Drain Time too short (<30s) |
| Resources | Missing hitsound files      |
| Resources | Custom histouns are not overridden |
| Files     | Hitsound difficulty in song folder |

### Minor/Unreliable checks (Warnings)

| Category |                Check               |
|:--------:|:----------------------------------:|
| Compose  | Long note too short (<30ms)        |
| Resources| Missing hitnormal                  |
| Settings | Too high OD                        |
| Settings | Too high HP                        |
| Timing   | Unnormalized SVs                   |
| Spread   | Chord size too high                |
| Compose  | Almost Stacked notes               |
| Compose  | Hitsound inconsistency between difficulties |
| Compose  | Stryboard Hitsounds |
| Compose  | Double hitsounds |


## Planned Checks
- Check for **extremely** close together notes to avoid stuff like [this](https://cdn.discordapp.com/attachments/808360583669874688/996761548536156281/unknown.png).

## How to Install

- Download the latest release of `ManiaChecks.dll`.
- Click the settings icon in Mapset Verifier
- Scroll to the button to the shortcuts
- Click the `Open externals folder` icon
- Open the `checks` folder.
- Place the `ManiaChecks.dll` file in this folder.

## Reporting Bugs

If you find something that doesn't feel right or want to make suggestions for potential checks, feel free to open a GitHub issue. You may also directly contact directly any of the project mantainers:
- Tailsdk (Discord: tailsdk | osu!: [Tailsdk](https://osu.ppy.sh/users/6751666)) 
- MChecaH (Discord: marticheca | osu!: [RandomeLoL](https://osu.ppy.sh/users/7080063)) 
