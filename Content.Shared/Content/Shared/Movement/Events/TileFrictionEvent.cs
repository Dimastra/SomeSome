using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events
{
	// Token: 0x020002E2 RID: 738
	[ByRefEvent]
	public struct TileFrictionEvent
	{
		// Token: 0x06000854 RID: 2132 RVA: 0x0001C881 File Offset: 0x0001AA81
		public TileFrictionEvent(float modifier)
		{
			this.Modifier = modifier;
		}

		// Token: 0x04000867 RID: 2151
		public float Modifier;
	}
}
