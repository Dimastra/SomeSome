using System;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Botany.Components
{
	// Token: 0x02000703 RID: 1795
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(BotanySystem)
	})]
	public sealed class SeedComponent : Component
	{
		// Token: 0x0400174F RID: 5967
		[DataField("seed", false, 1, false, false, null)]
		public SeedData Seed;

		// Token: 0x04001750 RID: 5968
		[DataField("seedId", false, 1, false, false, typeof(PrototypeIdSerializer<SeedPrototype>))]
		public readonly string SeedId;
	}
}
