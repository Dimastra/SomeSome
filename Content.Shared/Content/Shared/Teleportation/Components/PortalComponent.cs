using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Teleportation.Components
{
	// Token: 0x020000E5 RID: 229
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class PortalComponent : Component
	{
		// Token: 0x040002E9 RID: 745
		[DataField("arrivalSound", false, 1, false, false, null)]
		public SoundSpecifier ArrivalSound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg", null);

		// Token: 0x040002EA RID: 746
		[DataField("departureSound", false, 1, false, false, null)]
		public SoundSpecifier DepartureSound = new SoundPathSpecifier("/Audio/Effects/teleport_departure.ogg", null);

		// Token: 0x040002EB RID: 747
		[DataField("maxRandomRadius", false, 1, false, false, null)]
		public float MaxRandomRadius = 7f;
	}
}
