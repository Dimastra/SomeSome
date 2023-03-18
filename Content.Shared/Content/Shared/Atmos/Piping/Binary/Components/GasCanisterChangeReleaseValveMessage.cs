using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006C1 RID: 1729
	[NetSerializable]
	[Serializable]
	public sealed class GasCanisterChangeReleaseValveMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001503 RID: 5379 RVA: 0x0004535F File Offset: 0x0004355F
		public bool Valve { get; }

		// Token: 0x06001504 RID: 5380 RVA: 0x00045367 File Offset: 0x00043567
		public GasCanisterChangeReleaseValveMessage(bool valve)
		{
			this.Valve = valve;
		}
	}
}
