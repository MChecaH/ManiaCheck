using MapsetParser.objects;
using MapsetParser.objects.timinglines;
using MapsetParser.statics;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using static ManiaChecks.Utils;

namespace ManiaChecks
{
    [Check]
    public class CheckEzSv : BeatmapSetCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Category = "Timing",
            Message = "Abnormal Slider Velocity changes on lower difficulties found.",
            Author = "RandomeLoL",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Lower difficulties must not use inherited timing points on the lowest Easy/Normal difficulty of a set."
                },
                {
                    "Reasoning",
                    @"
                    Adding inherited timing points on the lowest Easy or Normal difficulty of a set can be disorienting for newer players not exposed to the gimmick. Therefore, inherited timing points must only be used to normalize the speed of a variable BPM song."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>()
            {
                {
                "Normalization Problem",
                    new IssueTemplate(Issue.Level.Problem,
                        "{1} {0} has an inaccurate normalized multiplier of {3}. Consider changing the multiplier to {2}.", "difficulty", "timestamp", "correctMultiplier", "currentMultiplier")
                    .WithCause("Wrongly normalized uninherited timing line.")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
        {
            var beatmap = beatmapSet.beatmaps.First();                              // The SV rule only applies to the first chart of a beatmapSet
            var difficulty = getManiaDifficulty(beatmap.metadataSettings.version);  // Get theoretical difficulty if the first chart.

            if (difficulty == Beatmap.Difficulty.Easy | difficulty == Beatmap.Difficulty.Normal)
            {
                var baseBPM = GetMostCommonBeatLength(beatmap); // Theoretical BPM to normalize the chart to    
                var timingLineList = beatmap.timingLines;       // Caling the "timingLines" list once

                // Instanciate needed variables. These will keep track of the previous RedLine which the GreenLines will be relative to.
                UninheritedLine prevUninheritedLine;
                double prevUninheritedBeatLength = 0;
                foreach (var timingLine in timingLineList)
                {   
                    if (timingLine.uninherited == true)
                    {
                        prevUninheritedLine = (UninheritedLine) timingLine;
                        prevUninheritedBeatLength = prevUninheritedLine.msPerBeat;
                    }

                    else
                    {
                        double correctMultiplier = Math.Round(baseBPM / prevUninheritedBeatLength, 2); // Theoretical correct multiplier.
                        double currentMultiplier = Math.Round(timingLine.svMult, 2);                   // Current multiplier being used.

                        if (!almostEquals(correctMultiplier, currentMultiplier, 0.01))
                            yield return new Issue(GetTemplate("Normalization Problem"), beatmap, beatmap.metadataSettings.version, Timestamp.Get(timingLine.offset), correctMultiplier, currentMultiplier);
                    }
                }
            }
        }
    }
}