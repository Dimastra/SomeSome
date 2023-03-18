using System;
using System.Runtime.CompilerServices;
using Content.Shared.Dataset;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Humanoid
{
	// Token: 0x0200040A RID: 1034
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NamingSystem : EntitySystem
	{
		// Token: 0x06000C1F RID: 3103 RVA: 0x00028120 File Offset: 0x00026320
		public string GetName(string species, Gender? gender = null)
		{
			SpeciesPrototype speciesProto;
			if (!this._prototypeManager.TryIndex<SpeciesPrototype>(species, ref speciesProto))
			{
				speciesProto = this._prototypeManager.Index<SpeciesPrototype>("Human");
				Logger.Warning("Unable to find species " + species + " for name, falling back to Human");
			}
			switch (speciesProto.Naming)
			{
			case SpeciesNaming.FirstDashFirst:
				return Loc.GetString("namepreset-firstdashfirst", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("first1", this.GetFirstName(speciesProto, gender)),
					new ValueTuple<string, object>("first2", this.GetFirstName(speciesProto, gender))
				});
			case SpeciesNaming.TheFirstofLast:
				return Loc.GetString("namepreset-thefirstoflast", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("first", this.GetFirstName(speciesProto, gender)),
					new ValueTuple<string, object>("last", this.GetLastName(speciesProto, gender))
				});
			}
			return Loc.GetString("namepreset-firstlast", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("first", this.GetFirstName(speciesProto, gender)),
				new ValueTuple<string, object>("last", this.GetLastName(speciesProto, gender))
			});
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x0002824C File Offset: 0x0002644C
		public string GetFirstName(SpeciesPrototype speciesProto, Gender? gender = null)
		{
			if (gender != null)
			{
				Gender valueOrDefault = gender.GetValueOrDefault();
				if (valueOrDefault == 2)
				{
					return RandomExtensions.Pick<string>(this._random, this._prototypeManager.Index<DatasetPrototype>(speciesProto.FemaleFirstNames).Values);
				}
				if (valueOrDefault == 3)
				{
					return RandomExtensions.Pick<string>(this._random, this._prototypeManager.Index<DatasetPrototype>(speciesProto.MaleFirstNames).Values);
				}
			}
			if (RandomExtensions.Prob(this._random, 0.5f))
			{
				return RandomExtensions.Pick<string>(this._random, this._prototypeManager.Index<DatasetPrototype>(speciesProto.MaleFirstNames).Values);
			}
			return RandomExtensions.Pick<string>(this._random, this._prototypeManager.Index<DatasetPrototype>(speciesProto.FemaleFirstNames).Values);
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0002830C File Offset: 0x0002650C
		public string GetLastName(SpeciesPrototype speciesProto, Gender? gender = null)
		{
			if (gender != null)
			{
				Gender valueOrDefault = gender.GetValueOrDefault();
				if (valueOrDefault == 2)
				{
					return RandomExtensions.Pick<string>(this._random, this._prototypeManager.Index<DatasetPrototype>(speciesProto.FemaleLastNames).Values);
				}
				if (valueOrDefault == 3)
				{
					return RandomExtensions.Pick<string>(this._random, this._prototypeManager.Index<DatasetPrototype>(speciesProto.MaleLastNames).Values);
				}
			}
			if (RandomExtensions.Prob(this._random, 0.5f))
			{
				return RandomExtensions.Pick<string>(this._random, this._prototypeManager.Index<DatasetPrototype>(speciesProto.MaleLastNames).Values);
			}
			return RandomExtensions.Pick<string>(this._random, this._prototypeManager.Index<DatasetPrototype>(speciesProto.FemaleLastNames).Values);
		}

		// Token: 0x04000C23 RID: 3107
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000C24 RID: 3108
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;
	}
}
