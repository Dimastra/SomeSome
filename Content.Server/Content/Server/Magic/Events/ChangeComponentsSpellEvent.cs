using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Magic.Events
{
	// Token: 0x020003E4 RID: 996
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChangeComponentsSpellEvent : EntityTargetActionEvent
	{
		// Token: 0x04000CAF RID: 3247
		[DataField("toAdd", false, 1, false, false, null)]
		[AlwaysPushInheritance]
		public EntityPrototype.ComponentRegistry ToAdd = new EntityPrototype.ComponentRegistry();

		// Token: 0x04000CB0 RID: 3248
		[DataField("toRemove", false, 1, false, false, null)]
		[AlwaysPushInheritance]
		public HashSet<string> ToRemove = new HashSet<string>();
	}
}
