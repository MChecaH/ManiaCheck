using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Collections.Generic;
using static ManiaChecks.Utils;

namespace ManiaChecks
{
    [Check]
    public class checkHN : BeatmapSetCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania }, 
            Category = "Hit Sounds",
            Message = "No Hitnormal sample found.",
            Author = "RandomeLoL",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Maps must always dispose of either custom Hitnormal samples or Hitsound additions."
                },
                {
                    "Reasoning",
                    @"
                    To better enhance their experience, some players might decide to play with Hitsounds on. To be as inclusive as possible, maps are required to, at the very least, have one custom sample to override osu!'s default Hitnormal sample."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>
            {
                {
                "No Hitnormal",
                    new IssueTemplate(Issue.Level.Problem,
                        "No Hitnormal sample can be seen in the Beatmapset's folder.")
                    .WithCause("Cannot find a \"Hitnormal\" sample in the set's Files.")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
        {
            List<string> hsList = beatmapSet.hitSoundFiles;
            if (hsList.Count == 0 ? true : !hasHitNormal(hsList))
                foreach (Beatmap beatmap in beatmapSet.beatmaps)
                    yield return new Issue(GetTemplate("No Hitnormal"), beatmap);
        }
    }
}
