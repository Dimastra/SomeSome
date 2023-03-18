using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Teleportation.Components
{
	// Token: 0x020000E7 RID: 231
	[NetSerializable]
	[Serializable]
	public sealed class PortalTimeoutComponentState : ComponentState
	{
		// Token: 0x06000286 RID: 646 RVA: 0x0000C27C File Offset: 0x0000A47C
		public PortalTimeoutComponentState(EntityUid? enteredPortal)
		{
			this.EnteredPortal = enteredPortal;
		}

		// Token: 0x040002ED RID: 749
		public EntityUid? EnteredPortal;
	}
}
