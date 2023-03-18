using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Tools
{
	// Token: 0x02000112 RID: 274
	public sealed class WelderToggledEvent : EntityEventArgs
	{
		// Token: 0x060004FF RID: 1279 RVA: 0x0001854B File Offset: 0x0001674B
		public WelderToggledEvent(bool welderOn)
		{
			this.WelderOn = welderOn;
		}

		// Token: 0x040002E8 RID: 744
		public bool WelderOn;
	}
}
