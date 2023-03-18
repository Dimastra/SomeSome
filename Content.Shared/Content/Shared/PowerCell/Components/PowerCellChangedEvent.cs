using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.PowerCell.Components
{
	// Token: 0x02000253 RID: 595
	public sealed class PowerCellChangedEvent : EntityEventArgs
	{
		// Token: 0x060006F0 RID: 1776 RVA: 0x000183BC File Offset: 0x000165BC
		public PowerCellChangedEvent(bool ejected)
		{
			this.Ejected = ejected;
		}

		// Token: 0x040006AE RID: 1710
		public readonly bool Ejected;
	}
}
