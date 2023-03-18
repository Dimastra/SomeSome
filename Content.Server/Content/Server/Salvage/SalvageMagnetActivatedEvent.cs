using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Salvage
{
	// Token: 0x0200021B RID: 539
	public sealed class SalvageMagnetActivatedEvent : EntityEventArgs
	{
		// Token: 0x06000AB8 RID: 2744 RVA: 0x00038165 File Offset: 0x00036365
		public SalvageMagnetActivatedEvent(EntityUid magnet)
		{
			this.Magnet = magnet;
		}

		// Token: 0x04000693 RID: 1683
		public EntityUid Magnet;
	}
}
