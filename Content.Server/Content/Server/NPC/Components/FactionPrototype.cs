using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Components
{
	// Token: 0x0200036A RID: 874
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("faction", 1)]
	public sealed class FactionPrototype : IPrototype
	{
		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06001208 RID: 4616 RVA: 0x0005EA47 File Offset: 0x0005CC47
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000B00 RID: 2816
		[ViewVariables]
		[DataField("friendly", false, 1, false, false, typeof(PrototypeIdListSerializer<FactionPrototype>))]
		public List<string> Friendly = new List<string>();

		// Token: 0x04000B01 RID: 2817
		[ViewVariables]
		[DataField("hostile", false, 1, false, false, typeof(PrototypeIdListSerializer<FactionPrototype>))]
		public List<string> Hostile = new List<string>();
	}
}
