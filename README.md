![](https://i.imgur.com/uWTpvxK.png)
![](https://i.imgur.com/2uwVyDO.gif)

ManiaCheck is an osu!mania plugin for [Naxess' Mapset verifier](https://github.com/Naxesss/MapsetVerifier). This tool is used to add extra checks specific for the osu!mania gamemode. 

> **Disclaimer**: This tool is still in early developement and false positives/negatives are likely to ocurr. For information on how to report existing bugs or request new features, see the [Reporting Bugs](#reporting-bugs) section of this Readme.

## Checks

### Unrankables (Problems)
|  Category |            Check            |
|:---------:|:---------------------------:|
| Compose   | Long note too short (<20ms) |
| Resources | Missing hitnormal           |
| ~~Timing~~    | ~~Unnormalized SVs in EZ/NM~~   |

### Minor/Unreliable checks (Warnings)

| Category |                Check               |
|:--------:|:----------------------------------:|
| Compose  | Long note too short (<30ms)        |
| Settings | Too high OD                        |
| Settings | Too high HP                        |
| Files    | Hitsound difficulty in song folder |
| Spread   | Chord size too high                |


## Planned Checks

- ~~Check that Easy difficulty doesn't have scroll changes~~
- Check whether variable timing maps have scroll Normalisation
- Check whether normalizing Green Lines are right on top of Red Lines
- Complete hitsound checks (consistency between difficulties, duplicate hitsounds, etc.)
- Check for **extremely** close together notes to avoid stuff like [this](https://cdn.discordapp.com/attachments/808360583669874688/996761548536156281/unknown.png).

## How to Install

- Download the latest release of `ManiaChecks.dll`.
- Open your `Roaming` folder, you can find it with either:
  - Typing `%APPDATA%` in your file explorer's address bar.
  - Navigate to the folder yourself `C:/Users/<YOURNAME>/AppData/Roaming`.
- Open the `Mapset Verifier Externals` folder.
- Open the `checks` folder.
- Place the `ManiaChecks.dll` file in this folder.

## Reporting Bugs

If you find something that doesn't feel right or want to make suggestions for potential checks, feel free to open a GitHub issue. You may also directly contact directly any of the project mantainers:
- Randome (Discord: MartíCheca#0770 | osu!: [RandomeLoL](https://osu.ppy.sh/users/7080063)) 
- Komi (Discord: Komirin#9568 | osu!: [Quenlla](https://web.whatsapp.com/)).
