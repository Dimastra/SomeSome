using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store
{
	// Token: 0x02000121 RID: 289
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("listing", 1)]
	[NetSerializable]
	[DataDefinition]
	[Serializable]
	public sealed class ListingPrototype : ListingData, IPrototype
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000359 RID: 857 RVA: 0x0000E69A File Offset: 0x0000C89A
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
