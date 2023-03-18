using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;

namespace Content.Server.GameTicking
{
	// Token: 0x020004A6 RID: 1190
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GameRuleAddedEvent
	{
		// Token: 0x1700035D RID: 861
		// (get) Token: 0x0600185D RID: 6237 RVA: 0x0007F6F2 File Offset: 0x0007D8F2
		public GameRulePrototype Rule { get; }

		// Token: 0x0600185E RID: 6238 RVA: 0x0007F6FA File Offset: 0x0007D8FA
		public GameRuleAddedEvent(GameRulePrototype rule)
		{
			this.Rule = rule;
		}
	}
}
