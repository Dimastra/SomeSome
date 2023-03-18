using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.ViewVariables;

namespace Content.Server.StationEvents
{
	// Token: 0x0200017C RID: 380
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BasicStationEventSchedulerSystem : GameRuleSystem
	{
		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000777 RID: 1911 RVA: 0x00024EA5 File Offset: 0x000230A5
		public override string Prototype
		{
			get
			{
				return "BasicStationEventScheduler";
			}
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x00024EAC File Offset: 0x000230AC
		public override void Started()
		{
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00024EAE File Offset: 0x000230AE
		public override void Ended()
		{
			this._timeUntilNextEvent = 300f;
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x00024EBC File Offset: 0x000230BC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!base.RuleStarted || !this._event.EventsEnabled)
			{
				return;
			}
			if (this._timeUntilNextEvent > 0f)
			{
				this._timeUntilNextEvent -= frameTime;
				return;
			}
			this._event.RunRandomEvent();
			this.ResetTimer();
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x00024F14 File Offset: 0x00023114
		private void ResetTimer()
		{
			this._timeUntilNextEvent = (float)this._random.Next(300, 1500);
		}

		// Token: 0x0400047E RID: 1150
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400047F RID: 1151
		[Dependency]
		private readonly EventManagerSystem _event;

		// Token: 0x04000480 RID: 1152
		private const float MinimumTimeUntilFirstEvent = 300f;

		// Token: 0x04000481 RID: 1153
		[ViewVariables]
		private float _timeUntilNextEvent = 300f;
	}
}
