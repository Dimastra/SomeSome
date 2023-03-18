using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Components
{
	// Token: 0x020001D0 RID: 464
	[NetSerializable]
	[Serializable]
	public enum ThrusterVisualState : byte
	{
		// Token: 0x0400053A RID: 1338
		State,
		// Token: 0x0400053B RID: 1339
		Thrusting
	}
}
