using MapsetParser.objects;
using MapsetParser.objects.timinglines;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ManiaChecks
{
    /// <summary> Collection of auxiliary functions exclusive to Mania checks </summary>
    public class Utils 
    {
        /// <summary> https://stackoverflow.com/a/30300521 </summary>
        public static String WildCardToRegular(String value) 
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$"; 
        }

        /// <summary> Checks whether the Hitsound list of a Beatmapset has any acceptable hitnormal sample </summary>
        public static bool hasHitNormal(List<string> hsList)
        {
            foreach (var HS in hsList) 
                if (Regex.IsMatch(HS, WildCardToRegular("*-hitnormal.*")))
                    return true;
            return false;
        }

        /// <summary> Updated difficulty dictionary for Mania </summary>
        public static readonly Dictionary<Beatmap.Difficulty, IEnumerable<string>> maniaDiffList =
            new Dictionary<Beatmap.Difficulty, IEnumerable<string>>() {
                { Beatmap.Difficulty.Easy,   new List<string>(){ "EZ", "Beginner", "Beginning", "Basic", "Easy"} },
                { Beatmap.Difficulty.Normal, new List<string>(){ "NM", "Normal", "Novice"} },
                { Beatmap.Difficulty.Hard,   new List<string>(){ "HD", "Hard", "Advanced", "Hyper"} },
                { Beatmap.Difficulty.Insane, new List<string>(){ "MX", "SC", "Another", "Exhaust", "Insane", "Lunatic"} },
                { Beatmap.Difficulty.Expert, new List<string>(){ "SHD", "EX", "Black Another",  "Infinite", "Gravity", "Heavenly", "Maximum", "Extra", "White Another", "Vivid", "Exceed" } }
            };

        /// <summary> Updated "getDifficultyFromName" method from MapsetParser only for Mania </summary>
        public static Beatmap.Difficulty getManiaDifficulty(string diffName)
        {
            foreach (var pair in maniaDiffList.Reverse())
                if (pair.Value.Any(value => new Regex(@$"(?i)(^| )[!-@\[-`{{-~]*{value}[!-@\[-`{{-~]*( |$)").IsMatch(diffName)))
                    return pair.Key;
            
            //In case no difficulty name is found, it will assume it's ambiguous
            return Beatmap.Difficulty.Ultra;     
        }

        /// <summary> Function that replicates osu!'s algorithm to determine a map's common beat length (Not BPM!). More info in https://github.com/ppy/osu/blob/3e5d29ed007277014eec19a65e4958508d7d8bb3/osu.Game/Beatmaps/Beatmap.cs#L83. This is also partly based of a Python script made to remove SVs in a beatmap. Don't ask, they generally suck anyways. </summary>
        public static double GetMostCommonBeatLength(Beatmap beatmap)
        {
            // We first get the last playable time in a beatmap with this. Returns 0 if no objects are present.
            double lastTime = 0;
            if (beatmap.hitObjects.Count == 0)
                return lastTime;
            else 
                lastTime = beatmap.hitObjects.Last().GetEndTime();

            // To make things easier, I've parsed only the timing points we'll use (RedLines // Uninherited). 
            // We'll unwrap it into a (offset, beatLength) map. MP doesn't have a Constructor for it.
            Dictionary<double, double> timingLineMap = new Dictionary<double, double>();
            foreach (var timingLine in beatmap.timingLines.OfType<UninheritedLine>().ToList())
            {
                timingLineMap[timingLine.offset] = timingLine.msPerBeat; 
            }

            // We add the last time under the assumption it has the same BPM as the last timing point.
            var lastOffset = timingLineMap.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            timingLineMap[lastTime] = timingLineMap[lastOffset];

            // Then, we create a Dictionary of tuples with (beatLength, totalDuration)
            Dictionary<double, double> durationList = new Dictionary<double, double>();
            KeyValuePair<double, double> prevTimingLine = timingLineMap.First();
            foreach (KeyValuePair<double, double> timingLine in timingLineMap.Skip(1))
            {
                var offsetDelta = timingLine.Key - prevTimingLine.Key;
                durationList[timingLine.Value] += offsetDelta;
                prevTimingLine = timingLine;
            }

            // Finally, we order the Dictionary in descending value fashion and return the key value of the first element.
            durationList.OrderByDescending(key => key.Value);
            return durationList.Keys.First();
        }
    }
}