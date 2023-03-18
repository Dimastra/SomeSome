using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;

namespace Content.Server.GameTicking
{
	// Token: 0x020004A7 RID: 1191
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GameRuleStartedEvent
	{
		// Token: 0x1700035E RID: 862
		// (get) Token: 0x0600185F RID: 6239 RVA: 0x0007F709 File Offset: 0x0007D909
		public GameRulePrototype Rule { get; }

		// Token: 0x06001860 RID: 6240 RVA: 0x0007F711 File Offset: 0x0007D911
		public GameRuleStartedEvent(GameRulePrototype rule)
		{
			this.Rule = rule;
		}
	}
}
