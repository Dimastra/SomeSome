using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.GameTicking.Rules.Configurations
{
	// Token: 0x020004C8 RID: 1224
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MaxTimeRestartRuleConfiguration : GameRuleConfiguration
	{
		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x0600195B RID: 6491 RVA: 0x00085F6A File Offset: 0x0008416A
		public override string Id
		{
			get
			{
				return "MaxTimeRestart";
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x0600195C RID: 6492 RVA: 0x00085F71 File Offset: 0x00084171
		[DataField("roundMaxTime", false, 1, true, false, null)]
		public TimeSpan RoundMaxTime { get; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x0600195D RID: 6493 RVA: 0x00085F79 File Offset: 0x00084179
		[DataField("roundEndDelay", false, 1, true, false, null)]
		public TimeSpan RoundEndDelay { get; }
	}
}
