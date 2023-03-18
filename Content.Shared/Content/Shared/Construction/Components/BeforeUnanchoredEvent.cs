using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components
{
	// Token: 0x0200058B RID: 1419
	public sealed class BeforeUnanchoredEvent : BaseAnchoredEvent
	{
		// Token: 0x0600116A RID: 4458 RVA: 0x00039153 File Offset: 0x00037353
		public BeforeUnanchoredEvent(EntityUid user, EntityUid tool) : base(user, tool)
		{
		}
	}
}
