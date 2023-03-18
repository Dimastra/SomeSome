using System;
using System.Runtime.CompilerServices;
using Content.Shared.Pulling;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.HTN.Preconditions
{
	// Token: 0x02000364 RID: 868
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PulledPrecondition : HTNPrecondition
	{
		// Token: 0x060011F5 RID: 4597 RVA: 0x0005E5C5 File Offset: 0x0005C7C5
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._pulling = sysManager.GetEntitySystem<SharedPullingSystem>();
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x0005E5DC File Offset: 0x0005C7DC
		public override bool IsMet(NPCBlackboard blackboard)
		{
			EntityUid owner = blackboard.GetValue<EntityUid>("Owner");
			return (this.IsPulled && this._pulling.IsPulled(owner, null)) || (!this.IsPulled && !this._pulling.IsPulled(owner, null));
		}

		// Token: 0x04000AED RID: 2797
		private SharedPullingSystem _pulling;

		// Token: 0x04000AEE RID: 2798
		[ViewVariables]
		[DataField("isPulled", false, 1, false, false, null)]
		public bool IsPulled = true;
	}
}
