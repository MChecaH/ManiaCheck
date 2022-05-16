using MapsetParser.objects;
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
                { Beatmap.Difficulty.Easy,   new List<string>(){ "EZ", "Beginner", "Begginning", "Basic", "Easy"} },
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
    }
}