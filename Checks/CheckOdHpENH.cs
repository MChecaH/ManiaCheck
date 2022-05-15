using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Collections.Generic;
using System;
using static ManiaChecks.Utils;

namespace ManiaChecks
{
    [Check]
    public class CheckOdHp : BeatmapCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Difficulties = new Beatmap.Difficulty[]
            {
                Beatmap.Difficulty.Easy,
                Beatmap.Difficulty.Normal,
                Beatmap.Difficulty.Hard
            },
            Category = "Settings",
            Message = "OD/HP Values too high",
            Author = "RandomeLoL",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Prevent easier difficulties to go beyond the OD/HP values present in the ruleset's Ranking Criteria."
                },
                {
                    "Reasoning",
                    @"
                    To prevent Easy/Normal/Hard maps from being too difficult, hard limits for both their Overall Difficulty and Health Points were added. These values can be whatever so long they do not go beyond these prestablished limits."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>()
            {
                {
                "HP Problem",
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} should not have a HP value over {1}, currently {2}.",
                        "difficulty", "max hp", "current hp")
                    .WithCause("One of the difficulties' HP breaches the RC limits.")
                },

                {
                "OD Problem",
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} should not have an OD value over {1}, currently {2}.",
                        "difficulty", "max od", "current od")
                    .WithCause("One of the difficulties' OD breaches the RC limits.")
                },
                {
                "Ambiguous",
                    new IssueTemplate(Issue.Level.Minor,
                        "Difficulty name {0} is ambiguous. Please ensure that HP {1} and OD {2} make sense to use.",
                        "difficulty", "current hp", "current od")
                    .WithCause("One of the difficulties uses an ambiguous naming schema.")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(Beatmap beatmap)
        {
            // HP/OD Getters
            double HP = Math.Round(beatmap.difficultySettings.hpDrain, 2, MidpointRounding.ToEven);
            double OD = Math.Round(beatmap.difficultySettings.overallDifficulty, 2, MidpointRounding.ToEven);

            // Difficulty name getter
            string diffName = beatmap.metadataSettings.version;

            // Get current diff for the "switch" to evaluate
            Beatmap.Difficulty difficulty = getManiaDifficulty(diffName);

            switch (difficulty) {
                case Beatmap.Difficulty.Easy:
                {
                    if (HP > 7)
                        yield return new Issue(GetTemplate("HP Problem"), beatmap,
                        diffName, 7, HP);
                    if (OD > 7)
                        yield return new Issue(GetTemplate("OD Problem"), beatmap,
                        diffName, 7, OD);
                    break;
                }
                case Beatmap.Difficulty.Normal:
                {
                    if (HP > 7.5)
                        yield return new Issue(GetTemplate("HP Problem"), beatmap,
                        diffName, 7.5, HP);
                    if (OD > 7.5)
                        yield return new Issue(GetTemplate("OD Problem"), beatmap,
                        diffName, 7.5, OD);
                    break;
                }
                case Beatmap.Difficulty.Hard:
                {
                    if (HP > 8)
                        yield return new Issue(GetTemplate("HP Problem"), beatmap,
                        diffName, 8, HP);
                    if (OD > 8)
                        yield return new Issue(GetTemplate("OD Problem"), beatmap,
                        diffName, 8, OD);
                    break;
                }
                case Beatmap.Difficulty.Ultra: // Ambiguous difficulty name case.
                {
                    yield return new Issue(GetTemplate("Ambiguous"), beatmap,
                        diffName, HP, OD);
                    break;
                }
            }    
        }
    }
}
