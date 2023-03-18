using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006C8 RID: 1736
	[NetSerializable]
	[Serializable]
	public sealed class GasVolumePumpToggleStatusMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001511 RID: 5393 RVA: 0x0004540E File Offset: 0x0004360E
		public bool Enabled { get; }

		// Token: 0x06001512 RID: 5394 RVA: 0x00045416 File Offset: 0x00043616
		public GasVolumePumpToggleStatusMessage(bool enabled)
		{
			this.Enabled = enabled;
		}
	}
}
