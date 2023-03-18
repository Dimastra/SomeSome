using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Nutrition
{
	// Token: 0x02000309 RID: 777
	public sealed class IngestionAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x04000935 RID: 2357
		public EntityUid? Blocker;
	}
}
