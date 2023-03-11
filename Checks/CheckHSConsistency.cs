using System.Collections.Generic;
using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using MapsetParser.statics;
using System;
using static MapsetParser.objects.Beatmap;
using System.Drawing;
using Microsoft.VisualBasic;

namespace ManiaChecks
{
	[Check]
	public class CheckHSCons : BeatmapSetCheck
	{
		public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata
		{
			Modes = new Beatmap.Mode[] { Beatmap.Mode.Mania },
			Category = "HS Cons",
			Message = "Hitsound inconsistency detected",
			Author = "Tailsdk",

			Documentation = new Dictionary<string, string>
			{
				{
					"Purpose",
					@"
					Check for incosistent hitsounds between difficulties."
				},
				{
					"Reasoning",
					@"
					Hitsounds should generally be consistent between difficulties unless it is intentional."
				}
			}
		};

		public override Dictionary<string, IssueTemplate> GetTemplates()
		{
			return new Dictionary<string, IssueTemplate>
			{
				{ "Warning",
					new IssueTemplate(Issue.Level.Warning,
						"{0} has a hitsound inconsistency with {1}.",
						"timestamp", "beatmap")
					.WithCause(
						"There is a hitsound inconsistency") }
			};
		}

		public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
		{
			// List of objects
			List<List<(HitObject.HitSound, double, string)>> beatmapListHS = new List<List<(HitObject.HitSound, double, string)>>();
			List<List<(string, double, HitObject.HitSound)>> beatmapListSI = new List<List<(string, double, HitObject.HitSound)>>();
			List<Beatmap> beatmapsList = new List<Beatmap>();
			foreach (Beatmap beatmap in beatmapSet.beatmaps)
			{
				beatmapsList.Add(beatmap);
				List<(HitObject.HitSound, double, string)> hitsoundList = new List<(HitObject.HitSound, double, string)>();
				List<(string, double, HitObject.HitSound)> sampleList = new List<(string, double, HitObject.HitSound)>();
				foreach (var hitObject in beatmap.hitObjects)
				{
					// Adds the various hitsounds to the hitsound list
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.Clap))
					{
						(HitObject.HitSound, double, string) hitsound = (HitObject.HitSound.Clap, hitObject.time, hitObject.filename);
						hitsoundList.Add(hitsound);
					}
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.Normal))
					{
						(HitObject.HitSound, double, string) hitsound = (HitObject.HitSound.Normal, hitObject.time, hitObject.filename);
						hitsoundList.Add(hitsound);
					}
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.None))
					{
						(HitObject.HitSound, double, string) hitsound = (HitObject.HitSound.None, hitObject.time, hitObject.filename);
						hitsoundList.Add(hitsound);
					}
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.Whistle))
					{
						(HitObject.HitSound, double, string) hitsound = (HitObject.HitSound.Whistle, hitObject.time, hitObject.filename);
						hitsoundList.Add(hitsound);
					}
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.Finish))
					{
						(HitObject.HitSound, double, string) hitsound = (HitObject.HitSound.Finish, hitObject.time, hitObject.filename);
						hitsoundList.Add(hitsound);
					}
					// Adds samples to the sample list
					sampleList.Add((hitObject.filename, hitObject.time, hitObject.hitSound));
				}
				beatmapListHS.Add(hitsoundList);
				beatmapListSI.Add(sampleList);
			}
			// Checks if a hitsound could be placed on another difficulty
			for (int i = 0; i < beatmapListHS.Count; i++)
			{
				for (int j = 0; j < beatmapListHS.Count; j++)
				{
					if (j == i)
					{
						continue;
					}
					foreach ((HitObject.HitSound, double, string) T1 in beatmapListHS[i])
					{
						if (T1.Item1 != HitObject.HitSound.Normal)
						{
							bool hasNote = false;
							foreach ((HitObject.HitSound, double, string) T2 in beatmapListHS[j])
							{

								if (T1.Item2 == T2.Item2 && T1.Item1 == T2.Item1)
								{
									break;
								}
								else if (T1.Item2 == T2.Item2 && T2.Item3 == null)
								{
									hasNote = true;
								}
								else if (T1.Item2 < T2.Item2)
								{
									if (hasNote)
									{
										yield return new Issue(GetTemplate("Warning"), beatmapsList[i], Timestamp.Get(T1.Item2), beatmapsList[j]);
									}
									break;
								}
							}
						}
					}
				}
			}
            // Checks if a import sample could be placed on another difficulty
            for (int i = 0; i < beatmapListSI.Count; i++)
            {
                for (int j = 0; j < beatmapListSI.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    foreach ((string, double, HitObject.HitSound) T1 in beatmapListSI[i])
					{
						if (T1.Item1 != null)
						{
                            bool hasNote = false;
                            foreach ((string, double, HitObject.HitSound) T2 in beatmapListSI[j])
                            {

                                if (T1.Item2 == T2.Item2 && T1.Item1 == T2.Item1)
                                {
                                    break;
                                }
                                else if (T1.Item2 == T2.Item2 && T2.Item1 == null && (T2.Item3 == HitObject.HitSound.Normal || T2.Item3 == HitObject.HitSound.None) )
                                {
                                    hasNote = true;
                                }
                                else if (T1.Item2 < T2.Item2)
                                {
                                    if (hasNote)
                                    {
                                        yield return new Issue(GetTemplate("Warning"), beatmapsList[i], Timestamp.Get(T1.Item2), beatmapsList[j]);
                                    }
                                    break;
                                }
                            }
                        }
					}
				}
			}
		}
	}
}
