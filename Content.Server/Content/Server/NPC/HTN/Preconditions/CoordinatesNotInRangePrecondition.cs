using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.Preconditions
{
	// Token: 0x02000361 RID: 865
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CoordinatesNotInRangePrecondition : HTNPrecondition
	{
		// Token: 0x060011EE RID: 4590 RVA: 0x0005E52C File Offset: 0x0005C72C
		public override bool IsMet(NPCBlackboard blackboard)
		{
			EntityCoordinates coordinates;
			EntityCoordinates target;
			return blackboard.TryGetValue<EntityCoordinates>("OwnerCoordinates", out coordinates, this._entManager) && blackboard.TryGetValue<EntityCoordinates>(this.TargetKey, out target, this._entManager) && !coordinates.InRange(this._entManager, target, blackboard.GetValueOrDefault<float>(this.RangeKey, this._entManager));
		}

		// Token: 0x04000AE9 RID: 2793
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AEA RID: 2794
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey;

		// Token: 0x04000AEB RID: 2795
		[DataField("rangeKey", false, 1, true, false, null)]
		public string RangeKey;
	}
}
