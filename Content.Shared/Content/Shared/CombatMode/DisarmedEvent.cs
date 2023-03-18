using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.CombatMode
{
	// Token: 0x02000599 RID: 1433
	public sealed class DisarmedEvent : HandledEntityEventArgs
	{
		// Token: 0x17000385 RID: 901
		// (get) Token: 0x0600117C RID: 4476 RVA: 0x0003929F File Offset: 0x0003749F
		// (set) Token: 0x0600117D RID: 4477 RVA: 0x000392A7 File Offset: 0x000374A7
		public EntityUid Target { get; set; }

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x0600117E RID: 4478 RVA: 0x000392B0 File Offset: 0x000374B0
		// (set) Token: 0x0600117F RID: 4479 RVA: 0x000392B8 File Offset: 0x000374B8
		public EntityUid Source { get; set; }

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06001180 RID: 4480 RVA: 0x000392C1 File Offset: 0x000374C1
		// (set) Token: 0x06001181 RID: 4481 RVA: 0x000392C9 File Offset: 0x000374C9
		public float PushProbability { get; set; }
	}
}
