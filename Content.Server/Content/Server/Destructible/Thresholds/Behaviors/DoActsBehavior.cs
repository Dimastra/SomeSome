using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005A3 RID: 1443
	[DataDefinition]
	[Serializable]
	public sealed class DoActsBehavior : IThresholdBehavior
	{
		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06001E07 RID: 7687 RVA: 0x0009F034 File Offset: 0x0009D234
		// (set) Token: 0x06001E08 RID: 7688 RVA: 0x0009F03C File Offset: 0x0009D23C
		[DataField("acts", false, 1, false, false, null)]
		public ThresholdActs Acts { get; set; }

		// Token: 0x06001E09 RID: 7689 RVA: 0x0009F045 File Offset: 0x0009D245
		public bool HasAct(ThresholdActs act)
		{
			return (this.Acts & act) > ThresholdActs.None;
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x0009F052 File Offset: 0x0009D252
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			if (this.HasAct(ThresholdActs.Breakage))
			{
				system.BreakEntity(owner);
			}
			if (this.HasAct(ThresholdActs.Destruction))
			{
				system.DestroyEntity(owner);
			}
		}
	}
}
