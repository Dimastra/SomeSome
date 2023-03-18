using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Disease
{
	// Token: 0x02000563 RID: 1379
	public sealed class CureDiseaseAttemptEvent : EntityEventArgs
	{
		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001D47 RID: 7495 RVA: 0x0009C365 File Offset: 0x0009A565
		public float CureChance { get; }

		// Token: 0x06001D48 RID: 7496 RVA: 0x0009C36D File Offset: 0x0009A56D
		public CureDiseaseAttemptEvent(float cureChance)
		{
			this.CureChance = cureChance;
		}
	}
}
