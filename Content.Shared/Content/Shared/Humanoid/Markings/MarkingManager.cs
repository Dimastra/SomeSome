using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x0200041D RID: 1053
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MarkingManager
	{
		// Token: 0x06000C81 RID: 3201 RVA: 0x0002905C File Offset: 0x0002725C
		public void Initialize()
		{
			this._prototypeManager.PrototypesReloaded += this.OnPrototypeReload;
			foreach (MarkingCategories category in Enum.GetValues<MarkingCategories>())
			{
				this._markingDict.Add(category, new Dictionary<string, MarkingPrototype>());
			}
			foreach (MarkingPrototype prototype in this._prototypeManager.EnumeratePrototypes<MarkingPrototype>())
			{
				this._index.Add(prototype);
				this._markingDict[prototype.MarkingCategory].Add(prototype.ID, prototype);
				this._markings.Add(prototype.ID, prototype);
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06000C82 RID: 3202 RVA: 0x0002912C File Offset: 0x0002732C
		public IReadOnlyDictionary<string, MarkingPrototype> Markings
		{
			get
			{
				return this._markings;
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06000C83 RID: 3203 RVA: 0x00029134 File Offset: 0x00027334
		public IReadOnlyDictionary<MarkingCategories, Dictionary<string, MarkingPrototype>> CategorizedMarkings
		{
			get
			{
				return this._markingDict;
			}
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x0002913C File Offset: 0x0002733C
		public IReadOnlyDictionary<string, MarkingPrototype> MarkingsByCategory(MarkingCategories category)
		{
			return this._markingDict[category];
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0002914C File Offset: 0x0002734C
		public IReadOnlyDictionary<string, MarkingPrototype> MarkingsByCategoryAndSpecies(MarkingCategories category, string species)
		{
			SpeciesPrototype speciesProto = this._prototypeManager.Index<SpeciesPrototype>(species);
			bool onlyWhitelisted = this._prototypeManager.Index<MarkingPointsPrototype>(speciesProto.MarkingPoints).OnlyWhitelisted;
			Dictionary<string, MarkingPrototype> res = new Dictionary<string, MarkingPrototype>();
			foreach (KeyValuePair<string, MarkingPrototype> keyValuePair in this.MarkingsByCategory(category))
			{
				string text;
				MarkingPrototype markingPrototype;
				keyValuePair.Deconstruct(out text, out markingPrototype);
				string key = text;
				MarkingPrototype marking = markingPrototype;
				if ((!onlyWhitelisted || marking.SpeciesRestrictions != null) && (marking.SpeciesRestrictions == null || marking.SpeciesRestrictions.Contains(species)))
				{
					res.Add(key, marking);
				}
			}
			return res;
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x00029200 File Offset: 0x00027400
		public bool TryGetMarking(Marking marking, [Nullable(2)] [NotNullWhen(true)] out MarkingPrototype markingResult)
		{
			return this._markings.TryGetValue(marking.MarkingId, out markingResult);
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x00029214 File Offset: 0x00027414
		public bool IsValidMarking(Marking marking, MarkingCategories category, string species)
		{
			MarkingPrototype proto;
			return this.TryGetMarking(marking, out proto) && proto.MarkingCategory == category && (proto.SpeciesRestrictions == null || proto.SpeciesRestrictions.Contains(species)) && marking.MarkingColors.Count == proto.Sprites.Count;
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x0002926C File Offset: 0x0002746C
		private void OnPrototypeReload(PrototypesReloadedEventArgs args)
		{
			PrototypesReloadedEventArgs.PrototypeChangeSet set;
			if (!args.ByType.TryGetValue(typeof(MarkingPrototype), out set))
			{
				return;
			}
			this._index.RemoveAll((MarkingPrototype i) => set.Modified.ContainsKey(i.ID));
			foreach (IPrototype prototype in set.Modified.Values)
			{
				MarkingPrototype markingPrototype = (MarkingPrototype)prototype;
				this._index.Add(markingPrototype);
			}
		}

		// Token: 0x04000C79 RID: 3193
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000C7A RID: 3194
		private readonly List<MarkingPrototype> _index = new List<MarkingPrototype>();

		// Token: 0x04000C7B RID: 3195
		private readonly Dictionary<MarkingCategories, Dictionary<string, MarkingPrototype>> _markingDict = new Dictionary<MarkingCategories, Dictionary<string, MarkingPrototype>>();

		// Token: 0x04000C7C RID: 3196
		private readonly Dictionary<string, MarkingPrototype> _markings = new Dictionary<string, MarkingPrototype>();
	}
}
