using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.GameTicking.Rules.Configurations
{
	// Token: 0x020004C5 RID: 1221
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class GameRuleConfiguration
	{
		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06001953 RID: 6483
		public abstract string Id { get; }
	}
}
