using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002E6 RID: 742
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ActiveJetpackComponent : Component
	{
		// Token: 0x0400086A RID: 2154
		public float EffectCooldown = 0.3f;

		// Token: 0x0400086B RID: 2155
		public TimeSpan TargetTime = TimeSpan.Zero;
	}
}
