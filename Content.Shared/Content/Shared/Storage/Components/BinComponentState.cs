using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000133 RID: 307
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class BinComponentState : ComponentState
	{
		// Token: 0x06000389 RID: 905 RVA: 0x0000F2DA File Offset: 0x0000D4DA
		public BinComponentState(List<EntityUid> items, [Nullable(2)] EntityWhitelist whitelist, int maxItems)
		{
			this.Items = items;
			this.Whitelist = whitelist;
			this.MaxItems = maxItems;
		}

		// Token: 0x040003A6 RID: 934
		public List<EntityUid> Items;

		// Token: 0x040003A7 RID: 935
		[Nullable(2)]
		public EntityWhitelist Whitelist;

		// Token: 0x040003A8 RID: 936
		public int MaxItems;
	}
}
