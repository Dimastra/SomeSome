using System;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Follower.Components
{
	// Token: 0x0200047C RID: 1148
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class OrbitVisualsComponent : Component
	{
		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06000DDD RID: 3549 RVA: 0x0002D419 File Offset: 0x0002B619
		// (set) Token: 0x06000DDE RID: 3550 RVA: 0x0002D421 File Offset: 0x0002B621
		[Animatable]
		public float Orbit { get; set; }

		// Token: 0x04000D2B RID: 3371
		public float OrbitLength = 2f;

		// Token: 0x04000D2C RID: 3372
		public float OrbitDistance = 1f;

		// Token: 0x04000D2D RID: 3373
		public float OrbitStopLength = 1f;
	}
}
