using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Vehicle.Components
{
	// Token: 0x020000A5 RID: 165
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class RiderComponent : Component
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000A0BA File Offset: 0x000082BA
		public override bool SendOnlyToOwner
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04000245 RID: 581
		[ViewVariables]
		public EntityUid? Vehicle;
	}
}
