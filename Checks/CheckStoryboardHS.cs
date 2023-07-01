using System.Collections.Generic;
using MapsetParser.objects.events;
using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using MapsetParser.statics;
using static MapsetParser.objects.Beatmap;

namespace ManiaChecks
{
    [Check]
    public class CheckSBHS : BeatmapSetCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Category = "Hit Sound",
            Message = "Contains storyboard hitsounds",
            Author = "Tailsdk & Naxess",

            Documentation = new Dictionary<string, string>
            {
                {
                    "Purpose",
                    @"
                    Detects if storyboard hitsounds are present."
                },
                {
                    "Reasoning",
                    @"
                    The hitsound copier will if using imported samples put hitsounds on the storyboard when no notes are present which can give false feedback."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>
            {
                { "Warning",
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} has a storyboard hitsound make sure that this is intentional.",
                        "timestamp")
                    .WithCause(
                        "There is a storyboard hitsound present") }
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
        {
            foreach (Beatmap beatmap in beatmapSet.beatmaps)
            {
                if (beatmap.generalSettings.mode != Mode.Mania)
                {
                    continue;
                }
                foreach (Sample storyHitSound in beatmap.samples)
                    foreach (Issue issue in GetStoryHitSoundIssue(beatmap, storyHitSound, ".osu"))
                        yield return issue;

                if (beatmapSet.osb == null)
                    continue;

                foreach (Sample storyHitSound in beatmapSet.osb.samples)
                    foreach (Issue issue in GetStoryHitSoundIssue(beatmap, storyHitSound, ".osb"))
                        yield return issue;
            }
        }
        private IEnumerable<Issue> GetStoryHitSoundIssue(Beatmap beatmap, Sample sample, string origin)
        {
            yield return new Issue(GetTemplate("Warning"), beatmap,
                Timestamp.Get(sample.time));
        }
    }
}
