using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components
{
	// Token: 0x020006BA RID: 1722
	[NetSerializable]
	[Serializable]
	public sealed class GasMixerChangeOutputPressureMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x060014F2 RID: 5362 RVA: 0x0004526F File Offset: 0x0004346F
		public float Pressure { get; }

		// Token: 0x060014F3 RID: 5363 RVA: 0x00045277 File Offset: 0x00043477
		public GasMixerChangeOutputPressureMessage(float pressure)
		{
			this.Pressure = pressure;
		}
	}
}
