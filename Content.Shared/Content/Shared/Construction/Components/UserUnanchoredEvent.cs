using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components
{
	// Token: 0x0200058C RID: 1420
	public sealed class UserUnanchoredEvent : BaseAnchoredEvent
	{
		// Token: 0x0600116B RID: 4459 RVA: 0x0003915D File Offset: 0x0003735D
		public UserUnanchoredEvent(EntityUid user, EntityUid tool) : base(user, tool)
		{
		}
	}
}
