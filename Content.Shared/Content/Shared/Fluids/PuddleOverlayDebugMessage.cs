using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fluids
{
	// Token: 0x02000486 RID: 1158
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class PuddleOverlayDebugMessage : EntityEventArgs
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x0002D6B6 File Offset: 0x0002B8B6
		public PuddleDebugOverlayData[] OverlayData { get; }

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x0002D6BE File Offset: 0x0002B8BE
		public EntityUid GridUid { get; }

		// Token: 0x06000DF2 RID: 3570 RVA: 0x0002D6C6 File Offset: 0x0002B8C6
		public PuddleOverlayDebugMessage(EntityUid gridUid, PuddleDebugOverlayData[] overlayData)
		{
			this.GridUid = gridUid;
			this.OverlayData = overlayData;
		}
	}
}
