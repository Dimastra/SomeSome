using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006C4 RID: 1732
	[NetSerializable]
	[Serializable]
	public sealed class GasPressurePumpToggleStatusMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001509 RID: 5385 RVA: 0x000453AB File Offset: 0x000435AB
		public bool Enabled { get; }

		// Token: 0x0600150A RID: 5386 RVA: 0x000453B3 File Offset: 0x000435B3
		public GasPressurePumpToggleStatusMessage(bool enabled)
		{
			this.Enabled = enabled;
		}
	}
}
