using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components
{
	// Token: 0x02000587 RID: 1415
	public sealed class UnanchorAttemptEvent : BaseAnchoredAttemptEvent
	{
		// Token: 0x06001164 RID: 4452 RVA: 0x0003910F File Offset: 0x0003730F
		public UnanchorAttemptEvent(EntityUid user, EntityUid tool) : base(user, tool)
		{
		}
	}
}
