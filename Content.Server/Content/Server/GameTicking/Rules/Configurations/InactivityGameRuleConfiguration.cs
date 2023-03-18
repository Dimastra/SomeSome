using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.GameTicking.Rules.Configurations
{
	// Token: 0x020004C7 RID: 1223
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InactivityGameRuleConfiguration : GameRuleConfiguration
	{
		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06001957 RID: 6487 RVA: 0x00085F4B File Offset: 0x0008414B
		public override string Id
		{
			get
			{
				return "InactivityTimeRestart";
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06001958 RID: 6488 RVA: 0x00085F52 File Offset: 0x00084152
		[DataField("inactivityMaxTime", false, 1, true, false, null)]
		public TimeSpan InactivityMaxTime { get; }

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06001959 RID: 6489 RVA: 0x00085F5A File Offset: 0x0008415A
		[DataField("roundEndDelay", false, 1, true, false, null)]
		public TimeSpan RoundEndDelay { get; }
	}
}
