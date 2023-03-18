using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Kitchen.EntitySystems;
using Content.Shared.Kitchen.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Server.Kitchen.Components
{
	// Token: 0x02000434 RID: 1076
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(KitchenSpikeSystem)
	})]
	[ComponentReference(typeof(SharedKitchenSpikeComponent))]
	public sealed class KitchenSpikeComponent : SharedKitchenSpikeComponent
	{
		// Token: 0x04000D9D RID: 3485
		[Nullable(2)]
		public List<string> PrototypesToSpawn;

		// Token: 0x04000D9E RID: 3486
		public string MeatSource1p = "?";

		// Token: 0x04000D9F RID: 3487
		public string MeatSource0 = "?";

		// Token: 0x04000DA0 RID: 3488
		public string Victim = "?";

		// Token: 0x04000DA1 RID: 3489
		public bool InUse;
	}
}
