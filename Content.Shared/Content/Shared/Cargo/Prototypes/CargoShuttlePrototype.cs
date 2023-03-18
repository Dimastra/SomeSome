using System;
using System.Runtime.CompilerServices;
using Content.Shared.Dataset;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cargo.Prototypes
{
	// Token: 0x0200062D RID: 1581
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("cargoShuttle", 1)]
	public sealed class CargoShuttlePrototype : IPrototype
	{
		// Token: 0x170003DC RID: 988
		// (get) Token: 0x0600131D RID: 4893 RVA: 0x0003FC2D File Offset: 0x0003DE2D
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x0400130C RID: 4876
		[DataField("path", false, 1, false, false, null)]
		public ResourcePath Path;

		// Token: 0x0400130D RID: 4877
		[DataField("nameDataset", false, 1, false, false, typeof(PrototypeIdSerializer<DatasetPrototype>))]
		public string NameDataset = "CargoShuttleNames";
	}
}
