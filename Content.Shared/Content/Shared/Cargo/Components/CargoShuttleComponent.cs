using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cargo.Components
{
	// Token: 0x02000633 RID: 1587
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedCargoSystem)
	})]
	public sealed class CargoShuttleComponent : Component
	{
		// Token: 0x04001314 RID: 4884
		[ViewVariables]
		[DataField("nextCall", false, 1, false, false, null)]
		public TimeSpan? NextCall;

		// Token: 0x04001315 RID: 4885
		[ViewVariables]
		[DataField("cooldown", false, 1, false, false, null)]
		public float Cooldown = 30f;

		// Token: 0x04001316 RID: 4886
		[ViewVariables]
		public bool CanRecall;

		// Token: 0x04001317 RID: 4887
		[ViewVariables]
		public EntityCoordinates Coordinates;

		// Token: 0x04001318 RID: 4888
		[DataField("station", false, 1, false, false, null)]
		public EntityUid? Station;

		// Token: 0x04001319 RID: 4889
		[Nullable(1)]
		[DataField("printerOutput", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string PrinterOutput = "PaperCargoInvoice";
	}
}
