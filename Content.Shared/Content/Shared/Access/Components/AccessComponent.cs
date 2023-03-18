using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Access.Components
{
	// Token: 0x02000779 RID: 1913
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedAccessSystem)
	})]
	public sealed class AccessComponent : Component
	{
		// Token: 0x04001757 RID: 5975
		[DataField("tags", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<AccessLevelPrototype>))]
		[Access]
		public HashSet<string> Tags = new HashSet<string>();

		// Token: 0x04001758 RID: 5976
		[DataField("groups", true, 1, false, false, typeof(PrototypeIdHashSetSerializer<AccessGroupPrototype>))]
		public readonly HashSet<string> Groups = new HashSet<string>();
	}
}
