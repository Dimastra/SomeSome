using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components
{
	// Token: 0x020006E8 RID: 1768
	[NetSerializable]
	[Serializable]
	public sealed class GasTankSetPressureMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x0600156A RID: 5482 RVA: 0x000460D6 File Offset: 0x000442D6
		// (set) Token: 0x0600156B RID: 5483 RVA: 0x000460DE File Offset: 0x000442DE
		public float Pressure { get; set; }
	}
}
