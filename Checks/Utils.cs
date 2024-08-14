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
        public static bool hasHitNormal(string hitsound)
        {
            if (Regex.IsMatch(hitsound.ToLower(), WildCardToRegular("*-hitnormal*")))
                return true;
            else return false;
        }

        /// <summary> Checks whether a specific sample is found in the hitNormal list </summary>
        public static bool isHitNormalInList(string sample, List<string> hitNormalList)
        {
            bool isInList = false;
            foreach (var hitNormal in hitNormalList)
                if (Regex.IsMatch(hitNormal, WildCardToRegular("*" + sample + "*")))
                    isInList = true;

            return isInList;
        }

        /// <summary> Returns a list of all hitnormal files used </summary>
        public static IEnumerable<string> getHitNormalSamples(List<string> hitSoundList)
        {
            foreach (var hitSound in hitSoundList)
                if (hasHitNormal(hitSound))
                    yield return hitSound.ToLower();
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

        /*
        public static double GetMostCommonBeatLength(Beatmap beatmap)
        {
            var timingLines = beatmap.timingLines;

            double lastTime = 0;
            if (beatmap.hitObjects.Count == 0)
                return lastTime;
            else 
                lastTime = beatmap.hitObjects.Last().GetEndTime();

            var durationList = new Dictionary<double, double>();
            foreach (UninheritedLine timingLine in timingLines.OfType<UninheritedLine>().ToList())
            {
                if (timingLine.Prev() != null)
                {
                    var prevTimingLine = (UninheritedLine) timingLine.Prev();
                    double prevBeatLength = prevTimingLine.msPerBeat;
                    double currentBeatLength = timingLine.msPerBeat;
                    if (!durationList.ContainsKey(prevBeatLength))
                        durationList[prevBeatLength] = Math.Abs(prevBeatLength - currentBeatLength);
                    else durationList[prevBeatLength] += Math.Abs(prevBeatLength - currentBeatLength);
                }
            }

            var lastLine = (UninheritedLine) timingLines.OfType<UninheritedLine>().Last();
            var lastBPM = lastLine.msPerBeat;
            durationList[lastBPM] += Math.Abs(durationList[lastBPM] - lastTime);

            durationList.OrderByDescending(key => key.Value);
            return durationList.Keys.First();
        }
        */

        /// <summary> "osu!" calc to get the base BPM of a beatmap. All credits go to the original devs. For consistency sake, we'll be using their algorithm. Original method github.com/Naxesss/MapsetParser/blob/master/objects/TimingLine.cs </summary>
        public static double GetMostCommonBeatLength(Beatmap beatmap)
        {
            // The last playable time in the beatmap - the last timing point extends to this time.
            // Note: This is more accurate and may present different results because osu-stable didn't have the ability to calculate slider durations in this context.
            double lastTime = beatmap.hitObjects.LastOrDefault()?.GetEndTime() ?? beatmap.timingLines.LastOrDefault()?.offset ?? 0;
            double firstTime = beatmap.hitObjects.FirstOrDefault()?.time ?? 0;

            // TimingLine -> UninheritedLine cast conversion to fetch "beatLength" values
            List<UninheritedLine> uninheritedLines = beatmap.timingLines.OfType<UninheritedLine>().Cast<UninheritedLine>().ToList();

            List<double> timeList = new List<double>();
            List<double> BPMList = new List<double>();

            bool first = true;
            double currentBPM = 0;
            foreach (var item in uninheritedLines)
            {
                if (!first)
                {
                    if (BPMList.Any(item => item - currentBPM == 0))
                    {
                        timeList[BPMList.IndexOf(currentBPM)] += item.offset - firstTime;
                    }
                    else
                    {
                        timeList.Add(item.offset - firstTime);
                        BPMList.Add(currentBPM);
                        
                    }
                    firstTime = item.offset;
                    currentBPM = Math.Round(item.bpm, 2);

                }
                else
                {
                    first = false;
                    currentBPM = Math.Round(item.bpm,2);
                }

            }
            if (BPMList.Any(item => Math.Abs(item - currentBPM) <= 1e-8))
            {
                timeList[BPMList.IndexOf(currentBPM)] += lastTime - firstTime;
            }
            else {
                timeList.Add(lastTime - firstTime);
                BPMList.Add(currentBPM);
            }

            return Math.Round(BPMList[timeList.IndexOf(timeList.Max())],2);
        }

        /// <summary> I love working with BeatLength. Converts "beatLength" to "BPM" and viceversa. </summary>
        public static double bpmConverter (double beatLengthORbpm)
        {
            return 60000 / beatLengthORbpm;
        }

        /// <summary> Checks if two values are almost equal given a third delta value. </summary>
        public static bool almostEquals (double value1, double value2, double epsilon)
        {
            return Math.Abs(value1 - value2) <= epsilon;
        }
    }
}