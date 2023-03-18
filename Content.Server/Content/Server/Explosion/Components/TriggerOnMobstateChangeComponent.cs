using System;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
	// Token: 0x0200051E RID: 1310
	[RegisterComponent]
	public sealed class TriggerOnMobstateChangeComponent : Component
	{
		// Token: 0x04001180 RID: 4480
		[ViewVariables]
		[DataField("mobState", false, 1, true, false, null)]
		public MobState MobState = MobState.Alive;

		// Token: 0x04001181 RID: 4481
		[ViewVariables]
		[DataField("preventSuicide", false, 1, false, false, null)]
		public bool PreventSuicide;
	}
}
