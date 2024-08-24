using System.Linq;
using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetParser.statics;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Collections.Generic;
using static ManiaChecks.Utils;
using System.Reflection.Metadata.Ecma335;
using static MapsetParser.objects.Beatmap;
using System.Security.Cryptography;

namespace ManiaChecks
{
    [Check]
    public class CheckColumnDistobution : BeatmapSetCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Category = "Compose",
            Message = "Underutilized column.",
            Author = "Tailsdk",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Maps should use all columns somewhat equally."
                },
                {
                    "Reasoning",
                    @"
                    Maps that do not use all columns somewhat evenly may have major handbalancing issues."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>
            {
                {
                "Underused column",
                    new IssueTemplate(Issue.Level.Warning,
                        "Column {0} is severely underused",
                        "column")
                    .WithCause("A column is being underused.")
                },
                {
                "Overused column",
                    new IssueTemplate(Issue.Level.Warning,
                        "Column {0} is severely overused",
                        "column")
                    .WithCause("A column is being overused.")
                },
                {
                "Unused column",
                    new IssueTemplate(Issue.Level.Problem,
                        "Column {0} is unused",
                        "column")
                    .WithCause("A column is unused.")
                }
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
                int keys = (int)beatmap.difficultySettings.circleSize;
                int totalNotes = 0;
                int[] columnDistrobution = new int[keys];
                foreach (var hitObject in beatmap.hitObjects)
                {
                    columnDistrobution[getColumn(hitObject, keys)] += 1;
                    totalNotes += 1;
                }

                int averageNotes = totalNotes / keys;
                int belowAverageNotes = (int)(averageNotes * 0.7);
                int aboveAverageNotes = (int)(averageNotes * 1.3);

                for (int i = 0; i < columnDistrobution.Length; i++)
                {
                    if (columnDistrobution[i] == 0)
                    {
                        yield return new Issue(GetTemplate("Unused column"), beatmap, i+1);
                    }
                    else if(columnDistrobution[i] >= aboveAverageNotes)
                    {
                        yield return new Issue(GetTemplate("Overused column"), beatmap, i + 1);
                    }
                    else if (columnDistrobution[i] <= belowAverageNotes)
                    {
                        yield return new Issue(GetTemplate("Underused column"), beatmap, i + 1);
                    }
                }
                
            }
            


        }
    }
}
