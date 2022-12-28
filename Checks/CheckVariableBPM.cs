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
    public class CheckVarBPM : BeatmapSetCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Category = "Timing",
            Message = "Unnormalized inherited timing lines found.",
            Author = "RandomeLoL",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Variable BPM songs usually should be normalized."
                },
                {
                    "Reasoning",
                    @"
                    Charts that do not exploit green lines and slider velocity changes despite having variable BPM should be normalized more often than not. This check will ensure to raise a warning if a variable BPM map is wrongly normalized, or if its normalizing green lines aren't right on top of the red lines that precede them."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>()
            {
                {
                    "Unnormal Value Warning",
                        new IssueTemplate(Issue.Level.Warning,
                            "{0} Isn't normalized. Ensure that the value makes sense.",
                            "timestamp")
                        .WithCause("Unnormalized timing line found.")
                },
                {
                    "Normalized Value Moved Problem",
                        new IssueTemplate(Issue.Level.Problem,
                            "{0} Isn't on top of the previous uninherited timing line. Its offset should be set to {1}.",
                            "timestamp", "newOffset")
                        .WithCause("Normalized timing line not on top of an uninherited timing line.")
                },
                {
                    "Green Line Not Found",
                        new IssueTemplate(Issue.Level.Warning,
                            "{0} Isn't followed by any normalizing green line. An inherited timing line with a multiplier of {1} should be added on top.",
                            "timestamp", "multiplier")
                        .WithCause("Uninherited line is unnormalized.")
                },
                {
                    "Debug",
                        new IssueTemplate(Issue.Level.Warning,
                            "Current BPM == {0} | Normal BPM == {1}",
                            "val1", "val2")
                        .WithCause("Puto tonto.")
                }   
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
        {
            foreach (var beatmap in beatmapSet.beatmaps)
            {
                var difficulty = getManiaDifficulty(beatmap.metadataSettings.version);
                if (beatmap == beatmapSet.beatmaps.First() && (difficulty == Beatmap.Difficulty.Easy | difficulty == Beatmap.Difficulty.Normal))
                    continue;

                var baseBPM = GetMostCommonBeatLength(beatmap); // Theoretical BPM to normalize the chart to 
                var timingLineList = beatmap.timingLines;       // Caling the "timingLines" list once

                // Instanciate needed variables. These will keep track of the previous RedLine which the GreenLines will be relative to.
                UninheritedLine prevUninheritedLine;    // RedLine Initialization
                double prevOffset = 0;                  // RedLine Timestamp
                double prevBPM = 0;                     // RedLine beatLength
                bool firstTimingLine = false;           // Control bit in charge of turning on once the first GreenLine after a RedLine is parsed
                foreach (var timingLine in timingLineList)
                {   
                    if (timingLine.uninherited == false)
                    {   
                        double correctMultiplier = Math.Round(baseBPM / prevBPM, 2); // Theoretical correct multiplier.
                        double currentMultiplier = Math.Round(timingLine.svMult, 2); // Current multiplier being used.

                        yield return new Issue(GetTemplate("Debug"), beatmap, Math.Round(bpmConverter(prevBPM), 2), Math.Round(bpmConverter(baseBPM), 2));

                        // Check for unnormalized values
                        if (!almostEquals(currentMultiplier, correctMultiplier, 0.01))
                            yield return new Issue(GetTemplate("Unnormal Value Warning"), beatmap, Timestamp.Get(timingLine.offset));

                        // Check for normalizing GreenLines not being right on top of the previous RedLine
                        else if (!firstTimingLine && timingLine.offset != prevOffset && !almostEquals(baseBPM, prevBPM, 1))
                            yield return new Issue(GetTemplate("Normalized Value Moved Problem"), beatmap, Timestamp.Get(timingLine.offset), prevOffset);

                        firstTimingLine = true;
                    }

                    else
                    {
                        // Adapt variables to new RedLine.
                        prevUninheritedLine = (UninheritedLine) timingLine;
                        prevOffset = prevUninheritedLine.offset;
                        prevBPM = prevUninheritedLine.msPerBeat;

                        // Reset control bit for the first GreenLine found after a RedLine.
                        firstTimingLine = false;

                        // Check if a RedLine needs to be normalized if it doesn't have any GreenLines on it.
                        if (timingLine.Next() is UninheritedLine && !almostEquals(baseBPM, prevUninheritedLine.msPerBeat, 1))
                            yield return new Issue(GetTemplate("Green Line Not Found"), beatmap, Timestamp.Get(prevOffset), Math.Round(prevBPM / baseBPM, 2));
                    }
                }
            }
        }
    }
}