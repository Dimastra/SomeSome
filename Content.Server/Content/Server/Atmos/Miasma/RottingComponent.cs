using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Miasma
{
	// Token: 0x0200078D RID: 1933
	[RegisterComponent]
	public sealed class RottingComponent : Component
	{
		// Token: 0x04001994 RID: 6548
		[ViewVariables]
		public bool DealDamage = true;
	}
}
