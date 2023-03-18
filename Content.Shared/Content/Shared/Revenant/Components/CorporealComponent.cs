using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Revenant.Components
{
	// Token: 0x020001F8 RID: 504
	[RegisterComponent]
	public sealed class CorporealComponent : Component
	{
		// Token: 0x040005A0 RID: 1440
		[ViewVariables]
		public float MovementSpeedDebuff = 0.66f;
	}
}
