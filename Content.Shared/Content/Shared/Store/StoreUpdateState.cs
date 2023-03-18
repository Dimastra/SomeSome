using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Store
{
	// Token: 0x02000125 RID: 293
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StoreUpdateState : BoundUserInterfaceState
	{
		// Token: 0x06000366 RID: 870 RVA: 0x0000E73A File Offset: 0x0000C93A
		public StoreUpdateState(HashSet<ListingData> listings, Dictionary<string, FixedPoint2> balance)
		{
			this.Listings = listings;
			this.Balance = balance;
		}

		// Token: 0x04000385 RID: 901
		public readonly HashSet<ListingData> Listings;

		// Token: 0x04000386 RID: 902
		public readonly Dictionary<string, FixedPoint2> Balance;
	}
}
