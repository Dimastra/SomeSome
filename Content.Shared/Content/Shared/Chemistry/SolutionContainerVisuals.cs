using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005E0 RID: 1504
	[NetSerializable]
	[Serializable]
	public enum SolutionContainerVisuals : byte
	{
		// Token: 0x040010FD RID: 4349
		Color,
		// Token: 0x040010FE RID: 4350
		FillFraction,
		// Token: 0x040010FF RID: 4351
		BaseOverride
	}
}
