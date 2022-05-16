using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetParser.statics;
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
            Category = "Resources",
            Message = "Hitnormal sample not found.",
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
                "No Hitnormal in Files",
                    new IssueTemplate(Issue.Level.Problem,
                        "No Hitnormal sample can be seen in the Beatmapset's folder.")
                    .WithCause("Cannot find a \"Hitnormal\" sample in the set's Files.")
                },
                {
                "No Hitnormal in Beatmap",
                    new IssueTemplate(Issue.Level.Problem,
                        "{0} No custom Hitnormal sample overwriting default.",
                        "timestamp")
                    .WithCause("Cannot find a \"Hitnormal\" sample in the set's Files.")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
        {
            List<string> hsList = beatmapSet.hitSoundFiles;
            if (hsList.Count == 0) 
                foreach (Beatmap beatmap in beatmapSet.beatmaps)
                    yield return new Issue(GetTemplate("No Hitnormal in Files"), beatmap);

            else 
            foreach (Beatmap beatmap in beatmapSet.beatmaps)
                foreach (var hitObject in beatmap.hitObjects)
                    if (hitObject.filename == null && !hasHitNormal(hsList))
                        yield return new Issue(GetTemplate("No Hitnormal in Beatmap"), beatmap, Timestamp.Get(hitObject));
        }
    }
}
