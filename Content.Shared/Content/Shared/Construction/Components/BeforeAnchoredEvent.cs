using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components
{
	// Token: 0x02000589 RID: 1417
	public sealed class BeforeAnchoredEvent : BaseAnchoredEvent
	{
		// Token: 0x06001168 RID: 4456 RVA: 0x0003913F File Offset: 0x0003733F
		public BeforeAnchoredEvent(EntityUid user, EntityUid tool) : base(user, tool)
		{
		}
	}
}
