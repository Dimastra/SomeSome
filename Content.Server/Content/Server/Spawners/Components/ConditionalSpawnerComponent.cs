using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Spawners.Components
{
	// Token: 0x020001D5 RID: 469
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Virtual]
	public class ConditionalSpawnerComponent : Component
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060008ED RID: 2285 RVA: 0x0002D795 File Offset: 0x0002B995
		// (set) Token: 0x060008EE RID: 2286 RVA: 0x0002D79D File Offset: 0x0002B99D
		[ViewVariables]
		[DataField("prototypes", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> Prototypes { get; set; } = new List<string>();

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060008EF RID: 2287 RVA: 0x0002D7A6 File Offset: 0x0002B9A6
		// (set) Token: 0x060008F0 RID: 2288 RVA: 0x0002D7AE File Offset: 0x0002B9AE
		[ViewVariables]
		[DataField("chance", false, 1, false, false, null)]
		public float Chance { get; set; } = 1f;

		// Token: 0x04000566 RID: 1382
		[ViewVariables]
		[DataField("gameRules", false, 1, false, false, typeof(PrototypeIdListSerializer<GameRulePrototype>))]
		public readonly List<string> GameRules = new List<string>();
	}
}
