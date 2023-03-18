using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.StationEvents.Events;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Random;
using Robust.Shared.ViewVariables;

namespace Content.Server.StationEvents
{
	// Token: 0x02000180 RID: 384
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RampingStationEventSchedulerSystem : GameRuleSystem
	{
		// Token: 0x1700014C RID: 332
		// (get) Token: 0x0600079C RID: 1948 RVA: 0x0002583A File Offset: 0x00023A3A
		public override string Prototype
		{
			get
			{
				return "RampingStationEventScheduler";
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x00025844 File Offset: 0x00023A44
		[ViewVariables]
		public float ChaosModifier
		{
			get
			{
				float roundTime = (float)this._gameTicker.RoundDuration().TotalSeconds;
				if (roundTime > this._endTime)
				{
					return this._maxChaos;
				}
				return this._maxChaos / this._endTime * roundTime + this._startingChaos;
			}
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x0002588C File Offset: 0x00023A8C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GetSeverityModifierEvent>(new EntityEventHandler<GetSeverityModifierEvent>(this.OnGetSeverityModifier), null, null);
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x000258A8 File Offset: 0x00023AA8
		public override void Started()
		{
			float avgChaos = this._cfg.GetCVar<float>(CCVars.EventsRampingAverageChaos);
			float avgTime = this._cfg.GetCVar<float>(CCVars.EventsRampingAverageEndTime);
			this._maxChaos = this._random.NextFloat(avgChaos - avgChaos / 4f, avgChaos + avgChaos / 4f);
			this._endTime = this._random.NextFloat(avgTime - avgTime / 4f, avgTime + avgTime / 4f) * 60f;
			this._startingChaos = this._maxChaos / 10f;
			this.PickNextEventTime();
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0002593B File Offset: 0x00023B3B
		public override void Ended()
		{
			this._endTime = 0f;
			this._maxChaos = 0f;
			this._startingChaos = 0f;
			this._timeUntilNextEvent = 0f;
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0002596C File Offset: 0x00023B6C
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
			this.PickNextEventTime();
			this._event.RunRandomEvent();
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x000259C4 File Offset: 0x00023BC4
		private void OnGetSeverityModifier(GetSeverityModifierEvent ev)
		{
			if (!base.RuleStarted)
			{
				return;
			}
			ev.Modifier *= this.ChaosModifier;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Ramping set modifier to ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ev.Modifier);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x00025A1C File Offset: 0x00023C1C
		private void PickNextEventTime()
		{
			float mod = this.ChaosModifier;
			this._timeUntilNextEvent = this._random.NextFloat(240f / mod, 720f / mod);
		}

		// Token: 0x04000494 RID: 1172
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000495 RID: 1173
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000496 RID: 1174
		[Dependency]
		private readonly EventManagerSystem _event;

		// Token: 0x04000497 RID: 1175
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x04000498 RID: 1176
		[ViewVariables]
		private float _endTime;

		// Token: 0x04000499 RID: 1177
		[ViewVariables]
		private float _maxChaos;

		// Token: 0x0400049A RID: 1178
		[ViewVariables]
		private float _startingChaos;

		// Token: 0x0400049B RID: 1179
		[ViewVariables]
		private float _timeUntilNextEvent;
	}
}
