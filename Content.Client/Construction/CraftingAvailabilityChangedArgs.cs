using System;

namespace Content.Client.Construction
{
	// Token: 0x02000390 RID: 912
	public sealed class CraftingAvailabilityChangedArgs : EventArgs
	{
		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06001674 RID: 5748 RVA: 0x00083F52 File Offset: 0x00082152
		public bool Available { get; }

		// Token: 0x06001675 RID: 5749 RVA: 0x00083F5A File Offset: 0x0008215A
		public CraftingAvailabilityChangedArgs(bool available)
		{
			this.Available = available;
		}
	}
}
