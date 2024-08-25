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
using static MapsetParser.objects.Beatmap;

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
            List<float> keymodes = new List<float>();
            foreach (var beatmap in beatmapSet.beatmaps)
            {
                if (beatmap.generalSettings.mode != Mode.Mania)
                {
                    continue;
                }
                var difficulty = getManiaDifficulty(beatmap.metadataSettings.version);

                if ((difficulty == Beatmap.Difficulty.Easy || difficulty == Beatmap.Difficulty.Normal) && !keymodes.Contains(beatmap.difficultySettings.circleSize))
                {
                    keymodes.Add(beatmap.difficultySettings.circleSize);
                    if (difficulty == Beatmap.Difficulty.Easy | difficulty == Beatmap.Difficulty.Normal)
                    {
                        var baseBPM = GetMostCommonBeatLength(beatmap); // Theoretical BPM to normalize the chart to    
                        var timingLineList = beatmap.timingLines;       // Caling the "timingLines" list once

                        // Instanciate needed variables. These will keep track of the previous RedLine which the GreenLines will be relative to.
                        UninheritedLine prevUninheritedLine;
                        double prevUninheritedBPM = 0;
                        

                        foreach (var timingLine in timingLineList)
                        {
                            if (timingLine.uninherited == true)
                            {
                                prevUninheritedLine = (UninheritedLine)timingLine;
                                prevUninheritedBPM = prevUninheritedLine.bpm;
                            }

                            else
                            {
                                double correctMultiplier = Math.Round(baseBPM / prevUninheritedBPM, 2); // Theoretical correct multiplier.
                                double currentMultiplier = Math.Round(timingLine.svMult, 2);                   // Current multiplier being used.

                                if (!almostEquals(currentMultiplier, correctMultiplier, 0.02))
                                    yield return new Issue(GetTemplate("Normalization Problem"), beatmap, beatmap.metadataSettings.version, Timestamp.Get(timingLine.offset), correctMultiplier, currentMultiplier);
                            }
                        }
                    }
                }
            }
        }
    }
}