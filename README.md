![ManiaCheck (BETA)](https://i.imgur.com/uWTpvxK.png)
# Mania Extension Project [BETA]
osu!mania plugin for [Naxess' Mapset verifier](https://github.com/Naxesss/MapsetVerifier). Includes various extra checks for osu!mania. These have been requested by Beatmap Nominators, but any extra check can be added if we need to!

## Current Plugins
**- [WARNING / PROBLEM] Long Note Length Check:** Checks if Long Notes are shorter than the minimum recommended in the Ranking Criteria.

**- [WARNING] OD/HP Check:** Checks if [Easy](https://osu.ppy.sh/wiki/en/Ranking_Criteria/osu!mania#easy)/[Normal](https://osu.ppy.sh/wiki/en/Ranking_Criteria/osu!mania#normal)/[Hard](https://osu.ppy.sh/wiki/en/Ranking_Criteria/osu!mania#hard) difficulties surpass the maximum allowed HP/OD values or not. Treated as a Warning as MV is not reliable when it comes to difficulty.

**- [WARNING] Max Chord Size:** Checks if [Easy](https://osu.ppy.sh/wiki/en/Ranking_Criteria/osu!mania#easy)/[Normal](https://osu.ppy.sh/wiki/en/Ranking_Criteria/osu!mania#normal)/[Hard](https://osu.ppy.sh/wiki/en/Ranking_Criteria/osu!mania#hard)/[Insane](https://osu.ppy.sh/wiki/en/Ranking_Criteria/osu!mania#insane) difficulties respect their given Max Chord Size guidelines in their respective Keymodes.

**- [PROBLEM] Hitnormal detector:** Detects whether a set disposes of a custom hitnormal sample in its Files or not.

**- [PROBLEM / WARNING] Hitsound difficulty detector:** Detects/Assumes there is a potential Hitsound difficulty still present in the map. It will mark it as a Problem if a difficulty is explicitly named *"Hitsound"* and mark it as a Warning if the map has any sort of Hitsound additions, just as a reminder.

## Future Goals
As a primary goal, we should try to accomplish the first checks requested by the BNG
![](https://i.imgur.com/Xg8Qis4.png)

Moreover, other suggested plugins by their *(respective users)* were:

**- ✔️ *(Dudehacker)* HS Difficulty Warning:** Detect if there's still a HS Diff present in a set. This is heavily geared towards BNs who'd want to avoid [this](https://imgur.com/aBrmnky) before nominating a set.

I am more than open to be tasked with more challenges, but I'll try focusing on the screenshot provided above first and foremost.

## How to Install
Excerpt taken from [Naxess' original Mapset Verifier repository](https://github.com/Naxesss/MapsetVerifier).

![](https://cdn.discordapp.com/attachments/367053814122938368/974695123994701844/unknown.png)

Basically just drag n' drop `ManiaChecks.dll` into that folder!
