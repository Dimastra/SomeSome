using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Polymorph;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Polymorph.Components
{
	// Token: 0x020002C8 RID: 712
	[RegisterComponent]
	public sealed class PolymorphableComponent : Component
	{
		// Token: 0x04000872 RID: 2162
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public Dictionary<string, InstantAction> PolymorphActions;

		// Token: 0x04000873 RID: 2163
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("innatePolymorphs", false, 1, false, false, typeof(PrototypeIdListSerializer<PolymorphPrototype>))]
		public List<string> InnatePolymorphs;
	}
}
