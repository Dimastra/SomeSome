using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components
{
	// Token: 0x020006B9 RID: 1721
	[NetSerializable]
	[Serializable]
	public sealed class GasMixerToggleStatusMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x060014F0 RID: 5360 RVA: 0x00045258 File Offset: 0x00043458
		public bool Enabled { get; }

		// Token: 0x060014F1 RID: 5361 RVA: 0x00045260 File Offset: 0x00043460
		public GasMixerToggleStatusMessage(bool enabled)
		{
			this.Enabled = enabled;
		}
	}
}
