using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

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

        /// <summary> https://stackoverflow.com/a/30300521 </summary>
        private static String WildCardToRegular(String value) 
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$"; 
        }

        /// <summary> Checks whether the Hitsound list of a Beatmapset has any acceptable hitnormal sample </summary>
        private static bool hasHitNormal(List<string> hsList)
        {
            foreach (var HS in hsList) 
                if (Regex.IsMatch(HS, WildCardToRegular("*-hitnormal*")))
                    return true;
            return false;
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