using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Fluids
{
	// Token: 0x02000484 RID: 1156
	public abstract class SharedPuddleDebugOverlaySystem : EntitySystem
	{
		// Token: 0x04000D44 RID: 3396
		protected const float LocalViewRange = 16f;

		// Token: 0x04000D45 RID: 3397
		protected TimeSpan? NextTick;

		// Token: 0x04000D46 RID: 3398
		protected TimeSpan Cooldown = TimeSpan.FromSeconds(0.5);
	}
}
