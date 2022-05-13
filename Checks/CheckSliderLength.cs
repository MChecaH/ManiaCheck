using MapsetParser.objects;
using MapsetParser.objects.hitobjects;
using MapsetParser.statics;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Collections.Generic;

namespace ManiaChecks
{
    [Check]
    public class CheckLNL : BeatmapCheck 
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Category = "Compose",
            Message = "Long Note's length too short.",
            Author = "RandomeLoL",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Prevent abnormally short Long Notes from being used across the entire spread.
                    <image>
                        https://i.imgur.com/S5KYuDJ.png
                        Examples of extremely short LNs, both beyond the Warning and Problem thresholds.
                    </image>"
                },
                {
                    "Reasoning",
                    @"
                    Due to how Long Notes work in osu!mania, if a Long Note's length is around 30 milliseconds, getting full accuracy on it can be very annoying and unforgiving.
                    <br><br>
                    This mainly has to do with human and hardware limitations. Ensuring these shorter LNs are far and few between will ensure that charts will be played much more fairly at no extra cost."
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
                        "{0} The Long Note should be held for at least {1} ms from start to finish, currently {2} ms.",
                        "timestamp - ", "required length", "current length")
                    .WithCause("The Long Note is a bit shorter than expected.")
                },

                {
                "Problem",
                    new IssueTemplate(Issue.Level.Problem,
                        "{0} The Long Note should be held for at least {1} ms from start to finish, currently {2} ms.",
                        "timestamp - ", "required length", "current length")
                    .WithCause("The Long Note is much shorter than expected.")
                }
            };
        }

        public override IEnumerable<Issue> GetIssues(Beatmap beatmap)
        {
            //Minimum slider length allowance in milliseconds.
            const int WarningThreshold = 30; // WARNING
            const int ProblemThreshold = 20; // PROBLEM

            foreach (var hitObject in beatmap.hitObjects) 
            {
                // Checks whether a given hitObject is a Long Note. Otherwise, checks the next hitObject.
                if (!(hitObject is HoldNote noodle))
                    continue;

                double LNLength = noodle.endTime - noodle.time;

                if (ProblemThreshold > LNLength)
                    yield return new Issue(GetTemplate("Problem"),
                        beatmap, Timestamp.Get(hitObject), 30, LNLength);

                else if (WarningThreshold > LNLength)
                    yield return new Issue(GetTemplate("Warning"),
                        beatmap, Timestamp.Get(hitObject), 30, LNLength);
            }
        }
    }
}