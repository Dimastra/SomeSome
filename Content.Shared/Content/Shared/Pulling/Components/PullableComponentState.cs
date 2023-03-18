using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Pulling.Components
{
	// Token: 0x0200023D RID: 573
	[NetSerializable]
	[Serializable]
	public sealed class PullableComponentState : ComponentState
	{
		// Token: 0x06000677 RID: 1655 RVA: 0x00017152 File Offset: 0x00015352
		public PullableComponentState(EntityUid? puller)
		{
			this.Puller = puller;
		}

		// Token: 0x0400066F RID: 1647
		public readonly EntityUid? Puller;
	}
}
