using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Tools.Innate
{
	// Token: 0x02000116 RID: 278
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class InnateToolComponent : Component
	{
		// Token: 0x040002EF RID: 751
		[DataField("tools", false, 1, false, false, null)]
		public List<EntitySpawnEntry> Tools = new List<EntitySpawnEntry>();

		// Token: 0x040002F0 RID: 752
		public List<EntityUid> ToolUids = new List<EntityUid>();
	}
}
