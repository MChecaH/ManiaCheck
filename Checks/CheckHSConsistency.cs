﻿using System.Collections.Generic;
using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using MapsetParser.statics;
using System;
using static MapsetParser.objects.Beatmap;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Linq;
using MapsetParser.objects.events;
using static ManiaChecks.Utils;

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
						"There is a hitsound inconsistency") },
                { "Problem",
                    new IssueTemplate(Issue.Level.Problem,
                        "{0} is used but does not exist.",
                        "hitsound")
                    .WithCause(
                        "Missing hitsound") },
                { "Double Hitsound",
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} you have 2 of the same samples here make sure this is intentional {1}",
                        "timestamp", "hitsound")
                    .WithCause(
                        "Double hitsound") },
            };
		}

		public override IEnumerable<Issue> GetIssues(BeatmapSet beatmapSet)
		{
			
			// List of objects
			List<List<(HitObject.HitSound, double, string, Sampleset, string)>> beatmapListHS = new List<List<(HitObject.HitSound, double, string, Sampleset, string)>>();
			List<List<(string, double, HitObject.HitSound)>> beatmapListSI = new List<List<(string, double, HitObject.HitSound)>>();
			List<int> beatmapsList = new List<int>();
			List<string> filesList = new List<string>();
            foreach (var s in beatmapSet.hitSoundFiles)
            {
                filesList.Add(s.ToLower());
            }
			List<(HitObject.HitSound, Sampleset, string)> checkedHS = new List<(HitObject.HitSound, Sampleset, string)>();
            List<string> checkedSI = new List<string>();
            foreach (var item in beatmapSet.beatmaps.Select((beatmap, i) => new { i, beatmap }))
			{
				beatmapsList.Add(item.i);
				if(item.beatmap.generalSettings.mode != Mode.Mania)
				{
					continue;
				}
				List<(HitObject.HitSound, double, string, Sampleset, string)> hitsoundList = new List<(HitObject.HitSound, double, string, Sampleset, string)>();
				List<(string, double, HitObject.HitSound)> sampleList = new List<(string, double, HitObject.HitSound)>();
				List<(double, Sampleset, string)> samplesetList = new List<(double, Sampleset, string)>();
				int index = 0;

				foreach (var tL in item.beatmap.timingLines)
				{
					samplesetList.Add((tL.offset, tL.sampleset, tL.customIndex.ToString()));
				}
                samplesetList.Add((double.MaxValue, Sampleset.Normal, ""));
                List<(HitObject.HitSound, string, Sampleset, string)> Added = new List<(HitObject.HitSound, string, Sampleset, string)>();
                List<(string, HitObject.HitSound)> SampleAdded = new List<(string, HitObject.HitSound)>();
                double tS = 0;
                foreach (var hitObject in item.beatmap.hitObjects)
				{
					if (hitObject.time != tS) 
					{
						tS = hitObject.time;
						Added.Clear();
						SampleAdded.Clear();
					}
					while (samplesetList[index+1].Item1 <= hitObject.time)
					{
						index += 1;
					}
					// Adds the various hitsounds to the hitsound list
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.Clap))
					{
						(HitObject.HitSound, double, string, Sampleset, string) hitsound = ( (hitObject.filename == null)? HitObject.HitSound.Clap : HitObject.HitSound.None, hitObject.time, hitObject.filename, (hitObject.addition == Sampleset.Auto) ? ((hitObject.sampleset == Sampleset.Auto) ? samplesetList[index].Item2 : hitObject.sampleset) : hitObject.addition, (samplesetList[index].Item3 == "1")? "" : samplesetList[index].Item3);
						if (!Added.Contains((hitsound.Item1,hitsound.Item3,hitsound.Item4,hitsound.Item5)))
						{
                            hitsoundList.Add(hitsound);
							Added.Add((hitsound.Item1, hitsound.Item3, hitsound.Item4, hitsound.Item5));
                        }
						else
						{
							yield return new Issue(GetTemplate("Double Hitsound"), item.beatmap, Timestamp.Get(hitObject.time), hitObject.hitSound);
                        }
					} 
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.Normal))
					{      
						(HitObject.HitSound, double, string, Sampleset, string) hitsound = ((hitObject.filename == null) ? HitObject.HitSound.Normal : HitObject.HitSound.None, hitObject.time, hitObject.filename, (hitObject.addition == Sampleset.Auto) ? ((hitObject.sampleset == Sampleset.Auto) ? samplesetList[index].Item2 : hitObject.sampleset) : hitObject.addition, (samplesetList[index].Item3 == "1") ? "" : samplesetList[index].Item3);
                        if (!Added.Contains((hitsound.Item1, hitsound.Item3, hitsound.Item4, hitsound.Item5)))
                        {
                            hitsoundList.Add(hitsound);
                            Added.Add((hitsound.Item1, hitsound.Item3, hitsound.Item4, hitsound.Item5));
                        }
                    }
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.None))
					{
						(HitObject.HitSound, double, string, Sampleset, string) hitsound = (HitObject.HitSound.None, hitObject.time, hitObject.filename, (hitObject.addition == Sampleset.Auto) ? ((hitObject.sampleset == Sampleset.Auto) ? samplesetList[index].Item2 : hitObject.sampleset) : hitObject.addition, (samplesetList[index].Item3 == "1") ? "" : samplesetList[index].Item3);
                    }
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.Whistle))
					{
						(HitObject.HitSound, double, string, Sampleset, string) hitsound = ((hitObject.filename == null) ? HitObject.HitSound.Whistle : HitObject.HitSound.None, hitObject.time, hitObject.filename, (hitObject.addition == Sampleset.Auto) ? ((hitObject.sampleset == Sampleset.Auto) ? samplesetList[index].Item2 : hitObject.sampleset) : hitObject.addition, (samplesetList[index].Item3 == "1") ? "" : samplesetList[index].Item3);
                        if (!Added.Contains((hitsound.Item1, hitsound.Item3, hitsound.Item4, hitsound.Item5)))
                        {
                            hitsoundList.Add(hitsound);
                            Added.Add((hitsound.Item1, hitsound.Item3, hitsound.Item4, hitsound.Item5));
                        }
                        else
                        {
                            yield return new Issue(GetTemplate("Double Hitsound"), item.beatmap, Timestamp.Get(hitObject.time), hitObject.hitSound);
                        }
                    }
					if (hitObject.hitSound.HasFlag(HitObject.HitSound.Finish))
					{
						(HitObject.HitSound, double, string, Sampleset, string) hitsound = ((hitObject.filename == null) ? HitObject.HitSound.Finish : HitObject.HitSound.None, hitObject.time, hitObject.filename, (hitObject.addition == Sampleset.Auto) ? ((hitObject.sampleset == Sampleset.Auto) ? samplesetList[index].Item2 : hitObject.sampleset) : hitObject.addition, (samplesetList[index].Item3 == "1") ? "" : samplesetList[index].Item3);
                        if (!Added.Contains((hitsound.Item1, hitsound.Item3, hitsound.Item4, hitsound.Item5)))
                        {
                            hitsoundList.Add(hitsound);
                            Added.Add((hitsound.Item1, hitsound.Item3, hitsound.Item4, hitsound.Item5));
                        }
                        else
                        {
                            yield return new Issue(GetTemplate("Double Hitsound"), item.beatmap, Timestamp.Get(hitObject.time), hitObject.hitSound);
                        }
                    }
					// Adds samples to the sample list
					if (!SampleAdded.Contains((hitObject.filename, hitObject.hitSound)))
					{
                        sampleList.Add((hitObject.filename, hitObject.time, hitObject.hitSound));
						SampleAdded.Add((hitObject.filename, hitObject.hitSound));
                    }
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
					foreach ((HitObject.HitSound, double, string, Sampleset, string) T1 in beatmapListHS[i])
					{
						if (T1.Item1 != HitObject.HitSound.None)
						{
                            if (T1.Item5 != "0" && !checkedHS.Contains((T1.Item1, T1.Item4, T1.Item5)) && !isHitNormalInList((T1.Item4.ToString().ToLower() + "-hit" + T1.Item1.ToString().ToLower() + T1.Item5.ToString()), filesList))
                            {
                                yield return new Issue(GetTemplate("Problem"), beatmapSet.beatmaps[beatmapsList[i]], T1.Item4.ToString().ToLower() + "-hit" + T1.Item1.ToString().ToLower() + T1.Item5.ToString() + ".wav/ogg");
								checkedHS.Add((T1.Item1, T1.Item4, T1.Item5));
                            }
                            bool hasNote = false;
							foreach ((HitObject.HitSound, double, string, Sampleset, string) T2 in beatmapListHS[j])
							{

								if (T1.Item2 == T2.Item2 && T1.Item1 == T2.Item1 && T1.Item4 == T2.Item4 && T1.Item5 == T2.Item5)
								{
									break;
								}
								else if (T1.Item2 == T2.Item2 && T2.Item3 == null && T1.Item4 == T2.Item4)
								{
									hasNote = true;
								}
								else if (T1.Item2 < T2.Item2)
								{
									if (hasNote)
									{
										yield return new Issue(GetTemplate("Warning"), beatmapSet.beatmaps[beatmapsList[i]], Timestamp.Get(T1.Item2), beatmapSet.beatmaps[beatmapsList[j]]);
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
							if (!checkedSI.Contains(T1.Item1) && !filesList.Contains(T1.Item1.ToLower()))
							{
                                yield return new Issue(GetTemplate("Problem"), beatmapSet.beatmaps[beatmapsList[i]], T1.Item1);
                                checkedSI.Add(T1.Item1);
                            }
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
                                        yield return new Issue(GetTemplate("Warning"), beatmapSet.beatmaps[beatmapsList[i]], Timestamp.Get(T1.Item2), beatmapSet.beatmaps[beatmapsList[j]]);
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
