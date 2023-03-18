using System;
using System.Runtime.CompilerServices;
using Content.Server.Buckle.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.HTN.Preconditions
{
	// Token: 0x0200035F RID: 863
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BuckledPrecondition : HTNPrecondition
	{
		// Token: 0x060011E9 RID: 4585 RVA: 0x0005E455 File Offset: 0x0005C655
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._buckle = sysManager.GetEntitySystem<BuckleSystem>();
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x0005E46C File Offset: 0x0005C66C
		public override bool IsMet(NPCBlackboard blackboard)
		{
			EntityUid owner = blackboard.GetValue<EntityUid>("Owner");
			return (this.IsBuckled && this._buckle.IsBuckled(owner, null)) || (!this.IsBuckled && !this._buckle.IsBuckled(owner, null));
		}

		// Token: 0x04000AE4 RID: 2788
		private BuckleSystem _buckle;

		// Token: 0x04000AE5 RID: 2789
		[ViewVariables]
		[DataField("isBuckled", false, 1, false, false, null)]
		public bool IsBuckled = true;
	}
}
