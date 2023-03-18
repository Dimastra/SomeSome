using System;
using Content.Shared.Atmos.Miasma;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Server.Atmos.Miasma
{
	// Token: 0x0200078A RID: 1930
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class FliesComponent : SharedFliesComponent
	{
		// Token: 0x04001982 RID: 6530
		public EntityUid VirtFlies;
	}
}
