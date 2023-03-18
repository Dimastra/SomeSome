using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Client.Guidebook
{
	// Token: 0x020002E3 RID: 739
	[RegisterComponent]
	public sealed class GuideHelpComponent : Component
	{
		// Token: 0x0400093F RID: 2367
		[Nullable(1)]
		[DataField("guides", false, 1, true, false, typeof(PrototypeIdListSerializer<GuideEntryPrototype>))]
		public List<string> Guides = new List<string>();

		// Token: 0x04000940 RID: 2368
		[DataField("includeChildren", false, 1, false, false, null)]
		public bool IncludeChildren = true;
	}
}
