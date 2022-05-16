using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static ManiaChecks.Utils;

namespace ManiaChecks
{
    [Check]
    public class CheckHSDiff : BeatmapSetCheck 
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Category = "Hit Sounds",
            Message = "Potential Hitsound difficulty found.",
            Author = "RandomeLoL",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Maps must not be nominated with Hitsound difficulties on them."
                },
                {
                    "Reasoning",
                    @"
                    Extra difficulties in a spread used as Hitsounding templates are commonly used in Mania as easy ways to copy an entire set of additions into the entiriety of a given spread. These tools ensure Hitsounds to be consistent when transfered over to all difficulties.
                    <br><br>
                    However, these extra difficulties aren't meant to be played nor Ranked. These are fully auxiliary and should be promptly deleted once they've served their purpose."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>()
            {
                {
                "Warning",
                    new IssueTemplate(Issue.Level.Problem,
                        "{0} has been detected to be a Hitsound diff. Please, ensure you delete it once you're done with it.", "difficulty")
                    .WithCause("Potential Hitsound difficulty detected.")
                },
                {
                "Reminder",
                    new IssueTemplate(Issue.Level.Warning,
                        "Custom Hitsound additions detected. Make sure the difficulty they were copied from is no longer in the spread.")
                    .WithCause("Hitsound Additions detected")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
        {
            bool foundDiff = false;
            foreach (Beatmap beatmap in beatmapSet.beatmaps) 
            {
                string difficulty = beatmap.metadataSettings.version;
                if (Regex.IsMatch(difficulty, WildCardToRegular("*h*i*t*s*o*u*n*d*"), RegexOptions.IgnoreCase)) 
                {
                    yield return new Issue(GetTemplate("Warning"), beatmap, difficulty);
                    foundDiff = true;
                }
            }

            if (!foundDiff) 
            {
                List<string> hsList = beatmapSet.hitSoundFiles;
                if (hsList.Count > 1)
                    foreach (Beatmap beatmap in beatmapSet.beatmaps)
                    {
                        string difficulty = beatmap.metadataSettings.version;
                        yield return new Issue(GetTemplate("Reminder"), beatmap);
                    }
            }
        }
    }
}