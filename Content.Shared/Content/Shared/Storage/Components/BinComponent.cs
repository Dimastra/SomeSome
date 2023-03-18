using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000132 RID: 306
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(BinSystem)
	})]
	public sealed class BinComponent : Component
	{
		// Token: 0x040003A1 RID: 929
		[ViewVariables]
		public Container ItemContainer;

		// Token: 0x040003A2 RID: 930
		[DataField("items", false, 1, false, false, null)]
		public List<EntityUid> Items = new List<EntityUid>();

		// Token: 0x040003A3 RID: 931
		[DataField("initialContents", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> InitialContents = new List<string>();

		// Token: 0x040003A4 RID: 932
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x040003A5 RID: 933
		[DataField("maxItems", false, 1, false, false, null)]
		public int MaxItems = 20;
	}
}
