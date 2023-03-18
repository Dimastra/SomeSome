using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.Preconditions
{
	// Token: 0x02000360 RID: 864
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CoordinatesInRangePrecondition : HTNPrecondition
	{
		// Token: 0x060011EC RID: 4588 RVA: 0x0005E4C8 File Offset: 0x0005C6C8
		public override bool IsMet(NPCBlackboard blackboard)
		{
			EntityCoordinates coordinates;
			EntityCoordinates target;
			return blackboard.TryGetValue<EntityCoordinates>("OwnerCoordinates", out coordinates, this._entManager) && blackboard.TryGetValue<EntityCoordinates>(this.TargetKey, out target, this._entManager) && coordinates.InRange(this._entManager, target, blackboard.GetValueOrDefault<float>(this.RangeKey, this._entManager));
		}

		// Token: 0x04000AE6 RID: 2790
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AE7 RID: 2791
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey;

		// Token: 0x04000AE8 RID: 2792
		[DataField("rangeKey", false, 1, true, false, null)]
		public string RangeKey;
	}
}
