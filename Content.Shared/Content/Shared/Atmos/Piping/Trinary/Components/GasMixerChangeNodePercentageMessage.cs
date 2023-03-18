using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components
{
	// Token: 0x020006BB RID: 1723
	[NetSerializable]
	[Serializable]
	public sealed class GasMixerChangeNodePercentageMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x060014F4 RID: 5364 RVA: 0x00045286 File Offset: 0x00043486
		public float NodeOne { get; }

		// Token: 0x060014F5 RID: 5365 RVA: 0x0004528E File Offset: 0x0004348E
		public GasMixerChangeNodePercentageMessage(float nodeOne)
		{
			this.NodeOne = nodeOne;
		}
	}
}
