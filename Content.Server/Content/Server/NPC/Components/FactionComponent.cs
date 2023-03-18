using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NPC.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Components
{
	// Token: 0x02000369 RID: 873
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FactionSystem)
	})]
	public sealed class FactionComponent : Component
	{
		// Token: 0x04000AFC RID: 2812
		[ViewVariables]
		[DataField("factions", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<FactionPrototype>))]
		public HashSet<string> Factions = new HashSet<string>();

		// Token: 0x04000AFD RID: 2813
		[ViewVariables]
		public readonly HashSet<string> FriendlyFactions = new HashSet<string>();

		// Token: 0x04000AFE RID: 2814
		[ViewVariables]
		public readonly HashSet<string> HostileFactions = new HashSet<string>();
	}
}
