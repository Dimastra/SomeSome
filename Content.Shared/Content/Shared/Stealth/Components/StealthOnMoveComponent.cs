using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Stealth.Components
{
	// Token: 0x02000154 RID: 340
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class StealthOnMoveComponent : Component
	{
		// Token: 0x040003EF RID: 1007
		[DataField("passiveVisibilityRate", false, 1, false, false, null)]
		public readonly float PassiveVisibilityRate = -0.15f;

		// Token: 0x040003F0 RID: 1008
		[DataField("movementVisibilityRate", false, 1, false, false, null)]
		public readonly float MovementVisibilityRate = 0.2f;
	}
}
