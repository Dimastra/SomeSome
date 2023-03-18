using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.NPC
{
	// Token: 0x020002D0 RID: 720
	public abstract class SharedNPCSteeringSystem : EntitySystem
	{
		// Token: 0x0400081B RID: 2075
		public const byte InterestDirections = 12;

		// Token: 0x0400081C RID: 2076
		public const float InterestRadians = 0.5235988f;

		// Token: 0x0400081D RID: 2077
		public const float InterestDegrees = 30f;
	}
}
