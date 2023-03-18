using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Cargo;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Cargo.Components
{
	// Token: 0x020006EF RID: 1775
	[RegisterComponent]
	public sealed class StationCargoOrderDatabaseComponent : Component
	{
		// Token: 0x040016C8 RID: 5832
		[ViewVariables]
		[DataField("capacity", false, 1, false, false, null)]
		public int Capacity = 20;

		// Token: 0x040016C9 RID: 5833
		[Nullable(1)]
		[ViewVariables]
		[DataField("orders", false, 1, false, false, null)]
		public Dictionary<int, CargoOrderData> Orders = new Dictionary<int, CargoOrderData>();

		// Token: 0x040016CA RID: 5834
		public int Index;

		// Token: 0x040016CB RID: 5835
		[Nullable(2)]
		[DataField("cargoShuttleProto", false, 1, false, false, typeof(PrototypeIdSerializer<CargoShuttlePrototype>))]
		public string CargoShuttleProto = "CargoShuttle";

		// Token: 0x040016CC RID: 5836
		[DataField("shuttle", false, 1, false, false, null)]
		public EntityUid? Shuttle;
	}
}
