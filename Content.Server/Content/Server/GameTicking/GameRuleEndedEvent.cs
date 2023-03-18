using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;

namespace Content.Server.GameTicking
{
	// Token: 0x020004A8 RID: 1192
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GameRuleEndedEvent
	{
		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06001861 RID: 6241 RVA: 0x0007F720 File Offset: 0x0007D920
		public GameRulePrototype Rule { get; }

		// Token: 0x06001862 RID: 6242 RVA: 0x0007F728 File Offset: 0x0007D928
		public GameRuleEndedEvent(GameRulePrototype rule)
		{
			this.Rule = rule;
		}
	}
}
