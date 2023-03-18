using System;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Botany.Components
{
	// Token: 0x02000702 RID: 1794
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(BotanySystem)
	})]
	public sealed class ProduceComponent : Component
	{
		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x060025BE RID: 9662 RVA: 0x000C6B4B File Offset: 0x000C4D4B
		// (set) Token: 0x060025BF RID: 9663 RVA: 0x000C6B53 File Offset: 0x000C4D53
		[DataField("targetSolution", false, 1, false, false, null)]
		public string SolutionName { get; set; } = "food";

		// Token: 0x0400174D RID: 5965
		[Nullable(2)]
		[DataField("seed", false, 1, false, false, null)]
		public SeedData Seed;

		// Token: 0x0400174E RID: 5966
		[Nullable(2)]
		[DataField("seedId", false, 1, false, false, typeof(PrototypeIdSerializer<SeedPrototype>))]
		public readonly string SeedId;
	}
}
