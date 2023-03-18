using System;
using System.Runtime.CompilerServices;
using Content.Server.Interaction;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.Preconditions
{
	// Token: 0x02000365 RID: 869
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TargetInLOSPrecondition : HTNPrecondition
	{
		// Token: 0x060011F8 RID: 4600 RVA: 0x0005E637 File Offset: 0x0005C837
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._interaction = sysManager.GetEntitySystem<InteractionSystem>();
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x0005E64C File Offset: 0x0005C84C
		public override bool IsMet(NPCBlackboard blackboard)
		{
			EntityUid owner = blackboard.GetValue<EntityUid>("Owner");
			EntityUid target;
			if (!blackboard.TryGetValue<EntityUid>(this.TargetKey, out target, this._entManager))
			{
				return false;
			}
			float range = blackboard.GetValueOrDefault<float>(this.RangeKey, this._entManager);
			return this._interaction.InRangeUnobstructed(owner, target, range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false);
		}

		// Token: 0x04000AEF RID: 2799
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AF0 RID: 2800
		private InteractionSystem _interaction;

		// Token: 0x04000AF1 RID: 2801
		[DataField("targetKey", false, 1, false, false, null)]
		public string TargetKey = "CombatTarget";

		// Token: 0x04000AF2 RID: 2802
		[DataField("rangeKey", false, 1, false, false, null)]
		public string RangeKey = "RangeKey";
	}
}
