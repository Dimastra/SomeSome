using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Store
{
	// Token: 0x02000128 RID: 296
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StoreBuyListingMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000369 RID: 873 RVA: 0x0000E767 File Offset: 0x0000C967
		public StoreBuyListingMessage(ListingData listing)
		{
			this.Listing = listing;
		}

		// Token: 0x04000388 RID: 904
		public ListingData Listing;
	}
}
