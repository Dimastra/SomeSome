using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x02000422 RID: 1058
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[NetSerializable]
	[Serializable]
	public sealed class MarkingSet
	{
		// Token: 0x06000C9B RID: 3227 RVA: 0x00029486 File Offset: 0x00027686
		public MarkingSet()
		{
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x000294A4 File Offset: 0x000276A4
		public MarkingSet(List<Marking> markings, string pointsPrototype, [Nullable(2)] MarkingManager markingManager = null, [Nullable(2)] IPrototypeManager prototypeManager = null)
		{
			IoCManager.Resolve<MarkingManager, IPrototypeManager>(ref markingManager, ref prototypeManager);
			MarkingPointsPrototype points;
			if (!prototypeManager.TryIndex<MarkingPointsPrototype>(pointsPrototype, ref points))
			{
				return;
			}
			this.Points = MarkingPoints.CloneMarkingPointDictionary(points.Points);
			foreach (Marking marking in markings)
			{
				MarkingPrototype prototype;
				if (markingManager.TryGetMarking(marking, out prototype))
				{
					this.AddBack(prototype.MarkingCategory, marking);
				}
			}
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x00029548 File Offset: 0x00027748
		public MarkingSet(List<Marking> markings, [Nullable(2)] MarkingManager markingManager = null)
		{
			IoCManager.Resolve<MarkingManager>(ref markingManager);
			foreach (Marking marking in markings)
			{
				MarkingPrototype prototype;
				if (markingManager.TryGetMarking(marking, out prototype))
				{
					this.AddBack(prototype.MarkingCategory, marking);
				}
			}
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x000295CC File Offset: 0x000277CC
		public MarkingSet(MarkingSet other)
		{
			foreach (KeyValuePair<MarkingCategories, List<Marking>> keyValuePair in other.Markings)
			{
				MarkingCategories markingCategories;
				List<Marking> list;
				keyValuePair.Deconstruct(out markingCategories, out list);
				MarkingCategories key = markingCategories;
				foreach (Marking marking in list)
				{
					this.AddBack(key, new Marking(marking));
				}
			}
			this.Points = MarkingPoints.CloneMarkingPointDictionary(other.Points);
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x0002969C File Offset: 0x0002789C
		[NullableContext(2)]
		public void FilterSpecies([Nullable(1)] string species, MarkingManager markingManager = null, IPrototypeManager prototypeManager = null)
		{
			IoCManager.Resolve<MarkingManager>(ref markingManager);
			IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
			List<ValueTuple<MarkingCategories, string>> toRemove = new List<ValueTuple<MarkingCategories, string>>();
			SpeciesPrototype speciesProto = prototypeManager.Index<SpeciesPrototype>(species);
			bool onlyWhitelisted = prototypeManager.Index<MarkingPointsPrototype>(speciesProto.MarkingPoints).OnlyWhitelisted;
			foreach (KeyValuePair<MarkingCategories, List<Marking>> keyValuePair in this.Markings)
			{
				MarkingCategories markingCategories;
				List<Marking> list;
				keyValuePair.Deconstruct(out markingCategories, out list);
				MarkingCategories category = markingCategories;
				foreach (Marking marking in list)
				{
					MarkingPrototype prototype;
					if (!markingManager.TryGetMarking(marking, out prototype))
					{
						toRemove.Add(new ValueTuple<MarkingCategories, string>(category, marking.MarkingId));
					}
					else
					{
						if (onlyWhitelisted && prototype.SpeciesRestrictions == null)
						{
							toRemove.Add(new ValueTuple<MarkingCategories, string>(category, marking.MarkingId));
						}
						if (prototype.SpeciesRestrictions != null && !prototype.SpeciesRestrictions.Contains(species))
						{
							toRemove.Add(new ValueTuple<MarkingCategories, string>(category, marking.MarkingId));
						}
					}
				}
			}
			foreach (ValueTuple<MarkingCategories, string> remove in toRemove)
			{
				this.Remove(remove.Item1, remove.Item2);
			}
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x00029820 File Offset: 0x00027A20
		[NullableContext(2)]
		public void FilterSponsor([Nullable(1)] string[] sponsorMarkings, MarkingManager markingManager = null, IPrototypeManager prototypeManager = null)
		{
			IoCManager.Resolve<MarkingManager>(ref markingManager);
			IoCManager.Resolve<IPrototypeManager>(ref prototypeManager);
			List<ValueTuple<MarkingCategories, string>> toRemove = new List<ValueTuple<MarkingCategories, string>>();
			foreach (KeyValuePair<MarkingCategories, List<Marking>> keyValuePair in this.Markings)
			{
				MarkingCategories markingCategories;
				List<Marking> list;
				keyValuePair.Deconstruct(out markingCategories, out list);
				MarkingCategories category = markingCategories;
				foreach (Marking marking in list)
				{
					MarkingPrototype proto;
					if (prototypeManager.TryIndex<MarkingPrototype>(marking.MarkingId, ref proto) && !proto.SponsorOnly)
					{
						return;
					}
					if (!sponsorMarkings.Contains(marking.MarkingId))
					{
						toRemove.Add(new ValueTuple<MarkingCategories, string>(category, marking.MarkingId));
					}
				}
			}
			foreach (ValueTuple<MarkingCategories, string> marking2 in toRemove)
			{
				this.Remove(marking2.Item1, marking2.Item2);
			}
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x0002995C File Offset: 0x00027B5C
		[NullableContext(2)]
		public void EnsureValid(MarkingManager markingManager = null)
		{
			IoCManager.Resolve<MarkingManager>(ref markingManager);
			List<int> toRemove = new List<int>();
			foreach (KeyValuePair<MarkingCategories, List<Marking>> keyValuePair in this.Markings)
			{
				MarkingCategories markingCategories;
				List<Marking> list2;
				keyValuePair.Deconstruct(out markingCategories, out list2);
				MarkingCategories category = markingCategories;
				List<Marking> list = list2;
				for (int i = 0; i < list.Count; i++)
				{
					MarkingPrototype marking;
					if (!markingManager.TryGetMarking(list[i], out marking))
					{
						toRemove.Add(i);
					}
					else if (marking.Sprites.Count != list[i].MarkingColors.Count)
					{
						list[i] = new Marking(marking.ID, marking.Sprites.Count);
					}
				}
				foreach (int j in toRemove)
				{
					this.Remove(category, j);
				}
			}
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x00029A80 File Offset: 0x00027C80
		[NullableContext(2)]
		public void EnsureDefault(Color? skinColor = null, MarkingManager markingManager = null)
		{
			IoCManager.Resolve<MarkingManager>(ref markingManager);
			foreach (KeyValuePair<MarkingCategories, MarkingPoints> keyValuePair in this.Points)
			{
				MarkingCategories markingCategories;
				MarkingPoints markingPoints;
				keyValuePair.Deconstruct(out markingCategories, out markingPoints);
				MarkingCategories category = markingCategories;
				MarkingPoints points = markingPoints;
				if (points.Points > 0 && points.DefaultMarkings.Count > 0)
				{
					int index = 0;
					while (points.Points > 0 || index < points.DefaultMarkings.Count)
					{
						MarkingPrototype prototype;
						if (markingManager.Markings.TryGetValue(points.DefaultMarkings[index], out prototype))
						{
							Marking marking;
							if (skinColor == null)
							{
								marking = new Marking(points.DefaultMarkings[index], prototype.Sprites.Count);
							}
							else
							{
								List<Color> colors = new List<Color>();
								for (int i = 0; i < prototype.Sprites.Count; i++)
								{
									colors.Add(skinColor.Value);
								}
								marking = new Marking(points.DefaultMarkings[index], colors);
							}
							this.AddBack(category, marking);
						}
						index++;
					}
				}
			}
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x00029BD8 File Offset: 0x00027DD8
		public int PointsLeft(MarkingCategories category)
		{
			MarkingPoints points;
			if (!this.Points.TryGetValue(category, out points))
			{
				return -1;
			}
			return points.Points;
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x00029C00 File Offset: 0x00027E00
		public void AddFront(MarkingCategories category, Marking marking)
		{
			MarkingPoints points;
			if (!marking.Forced && this.Points.TryGetValue(category, out points))
			{
				if (points.Points <= 0)
				{
					return;
				}
				points.Points--;
			}
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				markings = new List<Marking>();
				this.Markings[category] = markings;
			}
			markings.Insert(0, marking);
		}

		// Token: 0x06000CA5 RID: 3237 RVA: 0x00029C68 File Offset: 0x00027E68
		public void AddBack(MarkingCategories category, Marking marking)
		{
			MarkingPoints points;
			if (!marking.Forced && this.Points.TryGetValue(category, out points))
			{
				if (points.Points <= 0)
				{
					return;
				}
				points.Points--;
			}
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				markings = new List<Marking>();
				this.Markings[category] = markings;
			}
			markings.Add(marking);
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x00029CD0 File Offset: 0x00027ED0
		public List<Marking> AddCategory(MarkingCategories category)
		{
			List<Marking> markings = new List<Marking>();
			this.Markings.Add(category, markings);
			return markings;
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x00029CF4 File Offset: 0x00027EF4
		public void Replace(MarkingCategories category, int index, Marking marking)
		{
			List<Marking> markings;
			if (index < 0 || !this.Markings.TryGetValue(category, out markings) || index >= markings.Count)
			{
				return;
			}
			markings[index] = marking;
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x00029D28 File Offset: 0x00027F28
		public bool Remove(MarkingCategories category, string id)
		{
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return false;
			}
			for (int i = 0; i < markings.Count; i++)
			{
				if (!(markings[i].MarkingId != id))
				{
					MarkingPoints points;
					if (!markings[i].Forced && this.Points.TryGetValue(category, out points))
					{
						points.Points++;
					}
					markings.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x00029DA4 File Offset: 0x00027FA4
		public void Remove(MarkingCategories category, int idx)
		{
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return;
			}
			if (idx < 0 || idx >= markings.Count)
			{
				return;
			}
			MarkingPoints points;
			if (!markings[idx].Forced && this.Points.TryGetValue(category, out points))
			{
				points.Points++;
			}
			markings.RemoveAt(idx);
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x00029E04 File Offset: 0x00028004
		public bool RemoveCategory(MarkingCategories category)
		{
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return false;
			}
			MarkingPoints points;
			if (this.Points.TryGetValue(category, out points))
			{
				using (List<Marking>.Enumerator enumerator = markings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Forced)
						{
							points.Points++;
						}
					}
				}
			}
			this.Markings.Remove(category);
			return true;
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x00029E90 File Offset: 0x00028090
		public void Clear()
		{
			foreach (MarkingCategories category in Enum.GetValues<MarkingCategories>())
			{
				this.RemoveCategory(category);
			}
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x00029EC0 File Offset: 0x000280C0
		public int FindIndexOf(MarkingCategories category, string id)
		{
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return -1;
			}
			return markings.FindIndex((Marking m) => m.MarkingId == id);
		}

		// Token: 0x06000CAD RID: 3245 RVA: 0x00029F00 File Offset: 0x00028100
		public bool TryGetCategory(MarkingCategories category, [Nullable(new byte[]
		{
			2,
			1
		})] [NotNullWhen(true)] out IReadOnlyList<Marking> markings)
		{
			markings = null;
			List<Marking> list;
			if (this.Markings.TryGetValue(category, out list))
			{
				markings = list;
				return true;
			}
			return false;
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x00029F28 File Offset: 0x00028128
		public bool TryGetMarking(MarkingCategories category, string id, [Nullable(2)] [NotNullWhen(true)] out Marking marking)
		{
			marking = null;
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return false;
			}
			foreach (Marking i in markings)
			{
				if (i.MarkingId == id)
				{
					marking = i;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x00029F9C File Offset: 0x0002819C
		public void ShiftRankUp(MarkingCategories category, int idx)
		{
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return;
			}
			if (idx < 0 || idx >= markings.Count || idx - 1 < 0)
			{
				return;
			}
			List<Marking> list = markings;
			int index = idx - 1;
			List<Marking> list2 = markings;
			Marking value = markings[idx];
			Marking value2 = markings[idx - 1];
			list[index] = value;
			list2[idx] = value2;
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x00029FFC File Offset: 0x000281FC
		public void ShiftRankUpFromEnd(MarkingCategories category, int idx)
		{
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return;
			}
			this.ShiftRankUp(category, markings.Count - idx - 1);
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x0002A02C File Offset: 0x0002822C
		public void ShiftRankDown(MarkingCategories category, int idx)
		{
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return;
			}
			if (idx < 0 || idx >= markings.Count || idx + 1 >= markings.Count)
			{
				return;
			}
			List<Marking> list = markings;
			int index = idx + 1;
			List<Marking> list2 = markings;
			Marking value = markings[idx];
			Marking value2 = markings[idx + 1];
			list[index] = value;
			list2[idx] = value2;
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x0002A094 File Offset: 0x00028294
		public void ShiftRankDownFromEnd(MarkingCategories category, int idx)
		{
			List<Marking> markings;
			if (!this.Markings.TryGetValue(category, out markings))
			{
				return;
			}
			this.ShiftRankDown(category, markings.Count - idx - 1);
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x0002A0C4 File Offset: 0x000282C4
		public ForwardMarkingEnumerator GetForwardEnumerator()
		{
			List<Marking> markings = new List<Marking>();
			foreach (KeyValuePair<MarkingCategories, List<Marking>> keyValuePair in this.Markings)
			{
				MarkingCategories markingCategories;
				List<Marking> list2;
				keyValuePair.Deconstruct(out markingCategories, out list2);
				List<Marking> list = list2;
				markings.AddRange(list);
			}
			return new ForwardMarkingEnumerator(markings);
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0002A134 File Offset: 0x00028334
		public ForwardMarkingEnumerator GetForwardEnumerator(MarkingCategories category)
		{
			List<Marking> markings = new List<Marking>();
			List<Marking> listing;
			if (this.Markings.TryGetValue(category, out listing))
			{
				markings = new List<Marking>(listing);
			}
			return new ForwardMarkingEnumerator(markings);
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x0002A164 File Offset: 0x00028364
		public ReverseMarkingEnumerator GetReverseEnumerator()
		{
			List<Marking> markings = new List<Marking>();
			foreach (KeyValuePair<MarkingCategories, List<Marking>> keyValuePair in this.Markings)
			{
				MarkingCategories markingCategories;
				List<Marking> list2;
				keyValuePair.Deconstruct(out markingCategories, out list2);
				List<Marking> list = list2;
				markings.AddRange(list);
			}
			return new ReverseMarkingEnumerator(markings);
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x0002A1D4 File Offset: 0x000283D4
		public ReverseMarkingEnumerator GetReverseEnumerator(MarkingCategories category)
		{
			List<Marking> markings = new List<Marking>();
			List<Marking> listing;
			if (this.Markings.TryGetValue(category, out listing))
			{
				markings = new List<Marking>(listing);
			}
			return new ReverseMarkingEnumerator(markings);
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x0002A204 File Offset: 0x00028404
		public bool CategoryEquals(MarkingCategories category, MarkingSet other)
		{
			List<Marking> markings;
			List<Marking> markingsOther;
			return this.Markings.TryGetValue(category, out markings) && other.Markings.TryGetValue(category, out markingsOther) && markings.SequenceEqual(markingsOther);
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x0002A23C File Offset: 0x0002843C
		public bool Equals(MarkingSet other)
		{
			foreach (KeyValuePair<MarkingCategories, List<Marking>> keyValuePair in this.Markings)
			{
				MarkingCategories markingCategories;
				List<Marking> list;
				keyValuePair.Deconstruct(out markingCategories, out list);
				MarkingCategories category = markingCategories;
				if (!this.CategoryEquals(category, other))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x0002A2A8 File Offset: 0x000284A8
		public IEnumerable<MarkingCategories> CategoryDifference(MarkingSet other)
		{
			foreach (KeyValuePair<MarkingCategories, List<Marking>> keyValuePair in this.Markings)
			{
				MarkingCategories markingCategories;
				List<Marking> list;
				keyValuePair.Deconstruct(out markingCategories, out list);
				MarkingCategories category = markingCategories;
				if (!this.CategoryEquals(category, other))
				{
					yield return category;
				}
			}
			Dictionary<MarkingCategories, List<Marking>>.Enumerator enumerator = default(Dictionary<MarkingCategories, List<Marking>>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x04000C8D RID: 3213
		[DataField("markings", false, 1, false, false, null)]
		public Dictionary<MarkingCategories, List<Marking>> Markings = new Dictionary<MarkingCategories, List<Marking>>();

		// Token: 0x04000C8E RID: 3214
		[DataField("points", false, 1, false, false, null)]
		public Dictionary<MarkingCategories, MarkingPoints> Points = new Dictionary<MarkingCategories, MarkingPoints>();
	}
}
