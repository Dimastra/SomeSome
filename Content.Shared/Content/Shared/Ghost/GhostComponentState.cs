using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost
{
	// Token: 0x02000450 RID: 1104
	[NetSerializable]
	[Serializable]
	public sealed class GhostComponentState : ComponentState
	{
		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000D74 RID: 3444 RVA: 0x0002C7A4 File Offset: 0x0002A9A4
		public bool CanReturnToBody { get; }

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000D75 RID: 3445 RVA: 0x0002C7AC File Offset: 0x0002A9AC
		public bool CanGhostInteract { get; }

		// Token: 0x06000D76 RID: 3446 RVA: 0x0002C7B4 File Offset: 0x0002A9B4
		public GhostComponentState(bool canReturnToBody, bool canGhostInteract)
		{
			this.CanReturnToBody = canReturnToBody;
			this.CanGhostInteract = canGhostInteract;
		}
	}
}
