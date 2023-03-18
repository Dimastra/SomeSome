using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Body.Systems
{
	// Token: 0x0200070B RID: 1803
	public sealed class ApplyMetabolicMultiplierEvent : EntityEventArgs
	{
		// Token: 0x04001776 RID: 6006
		public EntityUid Uid;

		// Token: 0x04001777 RID: 6007
		public float Multiplier;

		// Token: 0x04001778 RID: 6008
		public bool Apply;
	}
}
