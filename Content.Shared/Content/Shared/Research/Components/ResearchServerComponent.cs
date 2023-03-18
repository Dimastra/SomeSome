using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Research.Components
{
	// Token: 0x0200020B RID: 523
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ResearchServerComponent : Component
	{
		// Token: 0x040005E6 RID: 1510
		[DataField("servername", false, 1, false, false, null)]
		[ViewVariables]
		public string ServerName = "RDSERVER";

		// Token: 0x040005E7 RID: 1511
		[DataField("points", false, 1, false, false, null)]
		[ViewVariables]
		public int Points;

		// Token: 0x040005E8 RID: 1512
		[ViewVariables]
		public int Id;

		// Token: 0x040005E9 RID: 1513
		[ViewVariables]
		public List<EntityUid> Clients = new List<EntityUid>();

		// Token: 0x040005EA RID: 1514
		[DataField("nextUpdateTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan NextUpdateTime = TimeSpan.Zero;

		// Token: 0x040005EB RID: 1515
		[DataField("researchConsoleUpdateTime", false, 1, false, false, null)]
		[ViewVariables]
		public readonly TimeSpan ResearchConsoleUpdateTime = TimeSpan.FromSeconds(1.0);
	}
}
