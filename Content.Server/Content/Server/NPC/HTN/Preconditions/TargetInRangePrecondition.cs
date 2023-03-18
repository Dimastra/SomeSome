using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.Preconditions
{
	// Token: 0x02000366 RID: 870
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TargetInRangePrecondition : HTNPrecondition
	{
		// Token: 0x060011FB RID: 4603 RVA: 0x0005E6C4 File Offset: 0x0005C8C4
		public override bool IsMet(NPCBlackboard blackboard)
		{
			EntityCoordinates coordinates;
			EntityUid target;
			TransformComponent targetXform;
			return blackboard.TryGetValue<EntityCoordinates>("OwnerCoordinates", out coordinates, this._entManager) && blackboard.TryGetValue<EntityUid>(this.TargetKey, out target, this._entManager) && this._entManager.TryGetComponent<TransformComponent>(target, ref targetXform) && coordinates.InRange(this._entManager, targetXform.Coordinates, blackboard.GetValueOrDefault<float>(this.RangeKey, this._entManager));
		}

		// Token: 0x04000AF3 RID: 2803
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AF4 RID: 2804
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey;

		// Token: 0x04000AF5 RID: 2805
		[DataField("rangeKey", false, 1, true, false, null)]
		public string RangeKey;
	}
}
