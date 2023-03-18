using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Respawn
{
	// Token: 0x020001FC RID: 508
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class SpecialRespawnComponent : Component
	{
		// Token: 0x040005C5 RID: 1477
		[ViewVariables]
		[DataField("stationMap", false, 1, false, false, null)]
		public ValueTuple<EntityUid?, EntityUid?> StationMap;

		// Token: 0x040005C6 RID: 1478
		[ViewVariables]
		[DataField("respawn", false, 1, false, false, null)]
		public bool Respawn = true;

		// Token: 0x040005C7 RID: 1479
		[Nullable(1)]
		[ViewVariables]
		[DataField("prototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype = "";
	}
}
