using MapsetParser.objects;
using MapsetParser.objects.hitobjects;
using MapsetParser.statics;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Collections.Generic;
using System.Linq;

namespace ManiaChecks
{
    [Check]
    public class CheckSeven : BeatmapCheck 
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Difficulties = new Beatmap.Difficulty[] {
                Beatmap.Difficulty.Easy,
                Beatmap.Difficulty.Normal,
                Beatmap.Difficulty.Hard,
                Beatmap.Difficulty.Insane
            },
            Category = "Compose",
            Message = "Chord too big for its difficulty.",
            Author = "RandomeLoL",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Prevent abnormally big chords from being used in lower difficulties."
                },
                {
                    "Reasoning",
                    @"
                    Due to how many players do not dispose of keyboards capable of registering more than 6-7 keystrokes at once (A.K.A Ghosting), lower difficulties should not allow for more than 7 notes to be pressed at once to be as accessible as possible.
                    <br><br>
                    Moreover, bigger chords in lower difficulties might be too daunting for players to read at their given skill level. Those should exclusively be used in >=Extra difficulties."
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
                        "{0} The chord should not have more than 6 notes.", 
                        "timestamp - ")
                    .WithCause("Chord bigger than expected.")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(Beatmap beatmap)
        {   
            // Modifiable maximum value for the chord's size
            const int maxChordSize = 7;

            // First iteration will skip the first value but still count it as if two notes were together.
            int counter = 1;

            foreach (var currentObject in beatmap.hitObjects.Skip(1))
            {
                var prevObject = currentObject.Prev();

                if (prevObject.time == currentObject.time)
                    ++counter;
                else
                    counter = 1;
                
                if (counter == maxChordSize) {
                    counter = 1;
                    yield return new Issue(GetTemplate("Problem"),
                        beatmap, Timestamp.Get(currentObject));
                }
            }
        }
    }
}