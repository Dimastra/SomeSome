using System;

namespace Content.Shared.Strip.Components
{
	// Token: 0x0200011A RID: 282
	public sealed class BeforeGettingStrippedEvent : BaseBeforeStripEvent
	{
		// Token: 0x0600033B RID: 827 RVA: 0x0000E14D File Offset: 0x0000C34D
		public BeforeGettingStrippedEvent(float initialTime, bool stealth = false) : base(initialTime, stealth)
		{
		}
	}
}
