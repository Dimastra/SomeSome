using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Construction
{
	// Token: 0x020005EF RID: 1519
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RefreshPartsEvent : EntityEventArgs
	{
		// Token: 0x04001420 RID: 5152
		public IReadOnlyList<MachinePartComponent> Parts = new List<MachinePartComponent>();

		// Token: 0x04001421 RID: 5153
		public Dictionary<string, float> PartRatings = new Dictionary<string, float>();
	}
}
