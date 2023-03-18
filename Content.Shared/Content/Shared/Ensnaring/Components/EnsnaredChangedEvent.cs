using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ensnaring.Components
{
	// Token: 0x020004BD RID: 1213
	public sealed class EnsnaredChangedEvent : EntityEventArgs
	{
		// Token: 0x06000EAE RID: 3758 RVA: 0x0002F474 File Offset: 0x0002D674
		public EnsnaredChangedEvent(bool isEnsnared)
		{
			this.IsEnsnared = isEnsnared;
		}

		// Token: 0x04000DCF RID: 3535
		public readonly bool IsEnsnared;
	}
}
