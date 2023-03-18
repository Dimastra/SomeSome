using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Cooldown
{
	// Token: 0x020005E6 RID: 1510
	public sealed class RefreshItemCooldownEvent : EntityEventArgs
	{
		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x0600203B RID: 8251 RVA: 0x000A8060 File Offset: 0x000A6260
		public TimeSpan LastAttackTime { get; }

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x0600203C RID: 8252 RVA: 0x000A8068 File Offset: 0x000A6268
		public TimeSpan CooldownEnd { get; }

		// Token: 0x0600203D RID: 8253 RVA: 0x000A8070 File Offset: 0x000A6270
		public RefreshItemCooldownEvent(TimeSpan lastAttackTime, TimeSpan cooldownEnd)
		{
			this.LastAttackTime = lastAttackTime;
			this.CooldownEnd = cooldownEnd;
		}
	}
}
