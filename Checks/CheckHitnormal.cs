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
            Message = "Missing custom hitnormal",
            Author = "RandomeLoL",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Maps must always have a hitnormal sample."
                },
                {
                    "Reasoning",
                    @"
                    To have a better playing experience, some players prefer active hitsound feedback. osu!mania Ranking Criteria mandates maps to have at least a custom sample overriding osu!'s default hitnormal."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>
            {
                {
                "HitnormalFile",
                    new IssueTemplate(Issue.Level.Problem,
                        "No hitnormal sample found in beatmap folder")
                    .WithCause("Cannot find a hitnormal sample in the beatmap folder.")
                },
                {
                "HitnormalOverride",
                    new IssueTemplate(Issue.Level.Problem,
                        "{0} Custom hitnormal isn't overriding default sample.",
                        "timestamp")
                    .WithCause("A hitnormal file is present, but it's not overwriting the default hitnormal.")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
        {
            List<string> hsList = beatmapSet.hitSoundFiles;
            if (hsList.Count == 0) 
                foreach (Beatmap beatmap in beatmapSet.beatmaps)
                    yield return new Issue(GetTemplate("HitnormalFile"), beatmap);

            else 
            foreach (Beatmap beatmap in beatmapSet.beatmaps)
                foreach (var hitObject in beatmap.hitObjects)
                    if (hitObject.filename == null && !hasHitNormal(hsList))
                        yield return new Issue(GetTemplate("HitnormalOverride"), beatmap, Timestamp.Get(hitObject));
        }
    }
}
