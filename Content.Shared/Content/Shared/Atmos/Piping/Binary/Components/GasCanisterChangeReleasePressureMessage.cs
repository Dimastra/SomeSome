using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006C0 RID: 1728
	[NetSerializable]
	[Serializable]
	public sealed class GasCanisterChangeReleasePressureMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001501 RID: 5377 RVA: 0x00045348 File Offset: 0x00043548
		public float Pressure { get; }

		// Token: 0x06001502 RID: 5378 RVA: 0x00045350 File Offset: 0x00043550
		public GasCanisterChangeReleasePressureMessage(float pressure)
		{
			this.Pressure = pressure;
		}
	}
}
