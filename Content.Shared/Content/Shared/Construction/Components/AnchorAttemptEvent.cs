using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components
{
	// Token: 0x02000586 RID: 1414
	public sealed class AnchorAttemptEvent : BaseAnchoredAttemptEvent
	{
		// Token: 0x06001163 RID: 4451 RVA: 0x00039105 File Offset: 0x00037305
		public AnchorAttemptEvent(EntityUid user, EntityUid tool) : base(user, tool)
		{
		}
	}
}
