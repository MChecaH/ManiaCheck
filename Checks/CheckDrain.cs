using System.Collections.Generic;
using System.Linq;
using MapsetParser.objects.events;
using MapsetParser.objects;
using MapsetParser.statics;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;

namespace ManiaChecks
{
    [Check] 
    public class CheckDrainTime : BeatmapCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Category = "Compose",
            Message = "Too short drain time.",
            Author = "Naxess",

            Documentation = new Dictionary<string, string>
            {
                {
                    "Purpose",
                    @"
                    Prevents beatmaps from being too short, for example 10 seconds long.
                    <image>
                        https://i.imgur.com/uNDPeJI.png
                        A beatmap with a total mp3 length of ~21 seconds.
                    </image>"
                },
                {
                    "Reasoning",
                    @"
                    Beatmaps this short do not offer a substantial enough gameplay experience for the standards of 
                    the ranked section."
                }
            }
        };
        
        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>
            {
                { "Problem",
                    new IssueTemplate(Issue.Level.Info,
                        "MANIA ONLY - Less than 30 seconds of Drain Time, currently {0}.",
                        "drain time")
                    .WithCause(
                        "The time from the first object to the end of the last object, subtracting any time between two objects " +
                        "where a break exists, is in total less than 30 seconds.") }
            };
        }

        public override IEnumerable<Issue> GetIssues(Beatmap beatmap)
        {
            double drainTime = 0;
            if (beatmap.hitObjects.Count > 0)
            {
                double startTime = beatmap.hitObjects.First().time;
                double endTime = 0;
                for (int i = 0; i < beatmap.hitObjects.Count; ++i)
                    endTime = endTime < beatmap.hitObjects[i].GetEndTime() ? beatmap.hitObjects[i].GetEndTime() : endTime;

                // remove breaks
                double breakReduction = 0;
                foreach (Break @break in beatmap.breaks)
                    breakReduction += @break.GetDuration(beatmap);

                drainTime = endTime - startTime - breakReduction;
            }

            if (drainTime < 30 * 1000)
                yield return new Issue(GetTemplate("Problem"), beatmap,
                    Timestamp.Get(drainTime));
        }
    }
}
