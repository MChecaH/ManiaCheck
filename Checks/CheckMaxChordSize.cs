using MapsetParser.objects;
using MapsetParser.statics;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Collections.Generic;
using System.Linq;
using static ManiaChecks.Utils;

namespace ManiaChecks
{
    [Check]
    public class CheckSeven : BeatmapCheck 
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Difficulties = new Beatmap.Difficulty[] 
            {
                Beatmap.Difficulty.Easy,
                Beatmap.Difficulty.Normal,
                Beatmap.Difficulty.Hard,
                Beatmap.Difficulty.Insane
            },
            Category = "Spread",
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
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} - Chords should not have more than {1} notes in {2}K for the given difficulty.", 
                        "timestamp - ", "maxChordSize", "keymode")
                    .WithCause("Chord bigger than expected.")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(Beatmap beatmap)
        {
            // Get current diff for the "switch" to evaluate
            Beatmap.Difficulty difficulty = getManiaDifficulty(beatmap.metadataSettings.version);

            // Get the chart's Keymode
            float keymode = beatmap.difficultySettings.circleSize;

            int counter = 1;
            foreach (var currentObject in beatmap.hitObjects.Skip(1)) {
                var prevObject = currentObject.Prev();
                var timestamp = Timestamp.Get(prevObject);

                if (prevObject.time == currentObject.time)
                    ++counter;
                else
                    counter = 1;

                // Prior check to avoid having to go through... all of the stuff below... I am sorry.   
                if (counter < 3) continue;
                
                // What difficulty to check for module
                switch (difficulty) {
                    case Beatmap.Difficulty.Easy:
                    {
                        if (counter == 3) 
                        {
                            yield return new Issue(GetTemplate("Warning"),
                                beatmap, timestamp.Substring(0, timestamp.IndexOf(" ")), 2, keymode);
                            counter = 1;
                        }
                        break;
                    }

                    case Beatmap.Difficulty.Normal:
                    {
                        if (counter == 4 && keymode >= 7)
                        {
                            yield return new Issue(GetTemplate("Warning"),
                                beatmap, timestamp.Substring(0, timestamp.IndexOf(" ")), 3, keymode);
                            counter = 1;
                        }
                        else if (counter == 3 && keymode >= 4 && keymode < 7) 
                        {
                            yield return new Issue(GetTemplate("Warning"),
                                beatmap, timestamp.Substring(0, timestamp.IndexOf(" ")), 2, keymode);
                            counter = 1;
                        }
                        break;
                    }

                    case Beatmap.Difficulty.Hard:
                    {
                        if (counter == 5 && keymode >= 7)
                        {
                            yield return new Issue(GetTemplate("Warning"),
                                beatmap, timestamp.Substring(0, timestamp.IndexOf(" ")), 4, keymode);
                            counter = 1;
                        }
                        else if (counter == 4 && keymode >= 4 && keymode < 7)
                        {
                            yield return new Issue(GetTemplate("Warning"),
                                beatmap, timestamp.Substring(0, timestamp.IndexOf(" ")), 3, keymode);
                            counter = 1;
                        }
                        break;
                    }

                    case Beatmap.Difficulty.Insane:
                    {
                        if (counter == 7) 
                        {
                            yield return new Issue(GetTemplate("Warning"),
                                beatmap, timestamp.Substring(0, timestamp.IndexOf(" ")), 7, keymode);
                            counter = 1;
                        }
                        break;
                    }
                }
            }       
        }   
    }
}
