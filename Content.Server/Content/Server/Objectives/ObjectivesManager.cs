using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Objectives
{
	// Token: 0x020002F3 RID: 755
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ObjectivesManager : IObjectivesManager
	{
		// Token: 0x06000F8F RID: 3983 RVA: 0x0004FFD8 File Offset: 0x0004E1D8
		[return: Nullable(2)]
		public ObjectivePrototype GetRandomObjective(Mind mind, string objectiveGroupProto)
		{
			WeightedRandomPrototype groups;
			if (!this._prototypeManager.TryIndex<WeightedRandomPrototype>(objectiveGroupProto, ref groups))
			{
				Logger.Error("Tried to get a random objective, but can't index WeightedRandomPrototype " + objectiveGroupProto);
				return null;
			}
			for (int tries = 0; tries < 20; tries++)
			{
				string groupName = groups.Pick(this._random);
				WeightedRandomPrototype group;
				if (!this._prototypeManager.TryIndex<WeightedRandomPrototype>(groupName, ref group))
				{
					Logger.Error("Couldn't index objective group prototype" + groupName);
					return null;
				}
				ObjectivePrototype objective;
				if (this._prototypeManager.TryIndex<ObjectivePrototype>(group.Pick(this._random), ref objective) && objective.CanBeAssigned(mind))
				{
					return objective;
				}
			}
			return null;
		}

		// Token: 0x04000920 RID: 2336
		[Dependency]
		private IPrototypeManager _prototypeManager;

		// Token: 0x04000921 RID: 2337
		[Dependency]
		private IRobustRandom _random;
	}
}
