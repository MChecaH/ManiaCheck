using System.Collections.Generic;
using MapsetParser.objects;
using MapsetParser.objects.hitobjects;
using MapsetParser.statics;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using MathNet.Numerics;

namespace ManiaChecks
{
    [Check]
    public class CheckConcurrent : BeatmapCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata
        {
            Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
            Category = "Compose",
            Message = "Concurrent hit objects.",
            Author = "Naxess & RandomeLoL",

            Documentation = new Dictionary<string, string>
            {
                {
                    "Purpose",
                    @"
                    Ensuring that only one object needs to be interacted with at any given moment in time."
                },
                {
                    "Reasoning",
                    @"
                    A clickable object during the duration of an already clicked object, for example a slider, is possible to play 
                    assuming the clickable object is within the slider circle whenever a slider tick/edge happens. However, there is 
                    no way for a player to intuitively know how to play such patterns as there is no tutorial for them, and they are 
                    not self-explanatory.
                    <br \><br \>
                    Sliders, spinners, and other holdable objects, teach the player to hold down the key for 
                    the whole duration of the object, so suddenly forcing them to press again would be contradictory to that 
                    fundamental understanding. Because of this, these patterns more often than not cause confusion, even where 
                    otherwise introduced well.
                    <image-right>
                        https://i.imgur.com/2bTX4aQ.png
                        A slider with two concurrent circles. Can be hit without breaking combo.
                    </image>"
                }
            }
        };
        
        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>
            {
                { "Concurrent Objects",
                    new IssueTemplate(Issue.Level.Problem,
                        "{0} Concurrent {1}.",
                        "timestamp - ", "hit objects")
                    .WithCause(
                        "A hit object starts before another hit object has ended. For mania this also " +
                        "requires that the objects are in the same column.") },

                { "Almost Concurrent Objects",
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} Within {1} ms of one another.",
                        "timestamp - ", "gap")
                    .WithCause(
                        "Two hit objects are less than 10 ms apart from one another.For mania this also " +
                        "requires that the objects are in the same column.") }
            };
        }

        public override IEnumerable<Issue> GetIssues(Beatmap beatmap)
        {
            int hitObjectCount = beatmap.hitObjects.Count;
            int keys = (int)beatmap.difficultySettings.circleSize;
            for (int i = 0; i < hitObjectCount - 1; ++i)
            {
                var hitObject = beatmap.hitObjects[i];           // Current Object to check

                for (int j = i + 1; j < hitObjectCount; ++j)
                {
                    var otherHitObject = beatmap.hitObjects[j];  // Next object to check, relative to "hitObject"

                    int hitObjectColumn = getColumn(hitObject, keys);
                    int otherHitObjectColumn = getColumn(otherHitObject, keys);

                    double msApart = otherHitObject.time - hitObject.GetEndTime(); // Time between objects

                    if (msApart > 30) break;                     // The second check will only check up to 30 ms forwards

                    if (hitObjectColumn == otherHitObjectColumn) // Check whether the objects' columns are the same
                    {
                        var timestamp = Timestamp.Get(hitObject);
                        var otherTimestamp = Timestamp.Get(otherHitObject);
                        if (msApart == 0)
                            yield return new Issue(GetTemplate("Concurrent Objects"), beatmap,
                                timestamp.Substring(0, timestamp.IndexOf(" ")), ObjectsAsString(hitObject, otherHitObject));

                        else if (msApart <= 30)
                            yield return new Issue(GetTemplate("Almost Concurrent Objects"), beatmap,
                                timestamp.Substring(0, timestamp.IndexOf(" ")), msApart);
                    }
                }
            }
        }

        private static string ObjectsAsString(HitObject hitObject, HitObject otherHitObject)
        {
            string type = hitObject.GetObjectType();
            string otherType = otherHitObject.GetObjectType();

            return
                type == otherType ?
                    type + "s" :
                    type + " and " + otherType;
        }

        private static int getColumn(HitObject hitObject, float keys)
        {
            // This is the * Magic Fix *
            return (int)hitObject.Position.X / (512 / (int)keys);
        }
    }
}
