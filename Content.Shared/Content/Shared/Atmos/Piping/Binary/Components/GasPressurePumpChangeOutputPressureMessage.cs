using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006C5 RID: 1733
	[NetSerializable]
	[Serializable]
	public sealed class GasPressurePumpChangeOutputPressureMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x0600150B RID: 5387 RVA: 0x000453C2 File Offset: 0x000435C2
		public float Pressure { get; }

		// Token: 0x0600150C RID: 5388 RVA: 0x000453CA File Offset: 0x000435CA
		public GasPressurePumpChangeOutputPressureMessage(float pressure)
		{
			this.Pressure = pressure;
		}
	}
}
