using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;

namespace Content.Server.StationEvents
{
	// Token: 0x0200017F RID: 383
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NoEventsSchedulerSystem : GameRuleSystem
	{
		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x00025825 File Offset: 0x00023A25
		public override string Prototype
		{
			get
			{
				return "NoEventsScheduler";
			}
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x0002582C File Offset: 0x00023A2C
		public override void Started()
		{
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x0002582E File Offset: 0x00023A2E
		public override void Ended()
		{
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00025830 File Offset: 0x00023A30
		public override void Update(float frameTime)
		{
		}
	}
}
