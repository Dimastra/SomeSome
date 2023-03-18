using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components
{
	// Token: 0x020006B4 RID: 1716
	[NetSerializable]
	[Serializable]
	public sealed class GasFilterToggleStatusMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x060014E5 RID: 5349 RVA: 0x000451CE File Offset: 0x000433CE
		public bool Enabled { get; }

		// Token: 0x060014E6 RID: 5350 RVA: 0x000451D6 File Offset: 0x000433D6
		public GasFilterToggleStatusMessage(bool enabled)
		{
			this.Enabled = enabled;
		}
	}
}
