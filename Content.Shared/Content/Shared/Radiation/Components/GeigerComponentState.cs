using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Components
{
	// Token: 0x0200022E RID: 558
	[NetSerializable]
	[Serializable]
	public sealed class GeigerComponentState : ComponentState
	{
		// Token: 0x0400063B RID: 1595
		public float CurrentRadiation;

		// Token: 0x0400063C RID: 1596
		public GeigerDangerLevel DangerLevel;

		// Token: 0x0400063D RID: 1597
		public bool IsEnabled;

		// Token: 0x0400063E RID: 1598
		public EntityUid? User;
	}
}
