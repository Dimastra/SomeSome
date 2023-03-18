using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005F1 RID: 1521
	[RegisterComponent]
	public sealed class ReactiveComponent : Component
	{
		// Token: 0x04001140 RID: 4416
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("groups", true, 1, false, true, typeof(PrototypeIdDictionarySerializer<HashSet<ReactionMethod>, ReactiveGroupPrototype>))]
		public Dictionary<string, HashSet<ReactionMethod>> ReactiveGroups;

		// Token: 0x04001141 RID: 4417
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("reactions", true, 1, false, true, null)]
		public List<ReactiveReagentEffectEntry> Reactions;
	}
}
