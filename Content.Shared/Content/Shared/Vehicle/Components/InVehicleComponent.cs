using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Vehicle.Components
{
	// Token: 0x020000A4 RID: 164
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class InVehicleComponent : Component
	{
		// Token: 0x04000244 RID: 580
		[Nullable(2)]
		[ViewVariables]
		public VehicleComponent Vehicle;
	}
}
