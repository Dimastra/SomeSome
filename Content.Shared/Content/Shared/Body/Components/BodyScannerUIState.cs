using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Body.Components
{
	// Token: 0x0200066E RID: 1646
	[NetSerializable]
	[Serializable]
	public sealed class BodyScannerUIState : BoundUserInterfaceState
	{
		// Token: 0x06001422 RID: 5154 RVA: 0x0004349E File Offset: 0x0004169E
		public BodyScannerUIState(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x040013CA RID: 5066
		public readonly EntityUid Uid;
	}
}
