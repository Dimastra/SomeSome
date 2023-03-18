using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Tools.Systems
{
	// Token: 0x02000115 RID: 277
	public sealed class WeldableChangedEvent : EntityEventArgs
	{
		// Token: 0x0600050C RID: 1292 RVA: 0x00018925 File Offset: 0x00016B25
		public WeldableChangedEvent(bool isWelded)
		{
			this.IsWelded = isWelded;
		}

		// Token: 0x040002EE RID: 750
		public readonly bool IsWelded;
	}
}
