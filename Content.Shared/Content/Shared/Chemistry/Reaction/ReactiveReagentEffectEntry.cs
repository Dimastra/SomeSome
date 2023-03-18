using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005F2 RID: 1522
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ReactiveReagentEffectEntry
	{
		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x0600127B RID: 4731 RVA: 0x0003C2BA File Offset: 0x0003A4BA
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("groups", true, 1, false, true, typeof(PrototypeIdDictionarySerializer<HashSet<ReactionMethod>, ReactiveGroupPrototype>))]
		public Dictionary<string, HashSet<ReactionMethod>> ReactiveGroups { [return: Nullable(new byte[]
		{
			2,
			1,
			1
		})] get; }

		// Token: 0x04001142 RID: 4418
		[DataField("methods", false, 1, false, false, null)]
		public HashSet<ReactionMethod> Methods;

		// Token: 0x04001143 RID: 4419
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("reagents", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<ReagentPrototype>))]
		public HashSet<string> Reagents;

		// Token: 0x04001144 RID: 4420
		[DataField("effects", false, 1, true, false, null)]
		public List<ReagentEffect> Effects;
	}
}
