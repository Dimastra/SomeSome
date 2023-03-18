using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules.Configurations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004B8 RID: 1208
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("gameRule", 1)]
	public sealed class GameRulePrototype : IPrototype
	{
		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06001897 RID: 6295 RVA: 0x0007FCD8 File Offset: 0x0007DED8
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06001898 RID: 6296 RVA: 0x0007FCE0 File Offset: 0x0007DEE0
		[DataField("config", false, 1, true, false, null)]
		public GameRuleConfiguration Configuration { get; }
	}
}
