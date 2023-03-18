using System;
using System.Runtime.CompilerServices;
using Content.Shared.DoAfter;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Resist
{
	// Token: 0x02000238 RID: 568
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ResistLockerSystem)
	})]
	public sealed class ResistLockerComponent : Component
	{
		// Token: 0x040006FE RID: 1790
		[DataField("resistTime", false, 1, false, false, null)]
		public float ResistTime = 120f;

		// Token: 0x040006FF RID: 1791
		[ViewVariables]
		public bool IsResisting;

		// Token: 0x04000700 RID: 1792
		[Nullable(2)]
		public DoAfter DoAfter;
	}
}
