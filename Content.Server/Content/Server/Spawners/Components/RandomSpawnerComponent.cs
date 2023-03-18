using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Spawners.Components
{
	// Token: 0x020001D6 RID: 470
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class RandomSpawnerComponent : ConditionalSpawnerComponent
	{
		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060008F2 RID: 2290 RVA: 0x0002D7E0 File Offset: 0x0002B9E0
		// (set) Token: 0x060008F3 RID: 2291 RVA: 0x0002D7E8 File Offset: 0x0002B9E8
		[ViewVariables]
		[DataField("rarePrototypes", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> RarePrototypes { get; set; } = new List<string>();

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x0002D7F1 File Offset: 0x0002B9F1
		// (set) Token: 0x060008F5 RID: 2293 RVA: 0x0002D7F9 File Offset: 0x0002B9F9
		[ViewVariables]
		[DataField("rareChance", false, 1, false, false, null)]
		public float RareChance { get; set; } = 0.05f;

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x0002D802 File Offset: 0x0002BA02
		// (set) Token: 0x060008F7 RID: 2295 RVA: 0x0002D80A File Offset: 0x0002BA0A
		[ViewVariables]
		[DataField("offset", false, 1, false, false, null)]
		public float Offset { get; set; } = 0.2f;
	}
}
