using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Station.Components;
using Content.Server.StationEvents.Events;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Random;
using Robust.Shared.ViewVariables;

namespace Content.Server.StationEvents
{
	// Token: 0x0200017E RID: 382
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeteorStationEventSchedulerSystem : GameRuleSystem
	{
		// Token: 0x17000149 RID: 329
		// (get) Token: 0x0600078E RID: 1934 RVA: 0x00025558 File Offset: 0x00023758
		public override string Prototype
		{
			get
			{
				return "MeteorStationEventScheduler";
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x00025560 File Offset: 0x00023760
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

		// Token: 0x06000790 RID: 1936 RVA: 0x000255A8 File Offset: 0x000237A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GetSeverityModifierEvent>(new EntityEventHandler<GetSeverityModifierEvent>(this.OnGetSeverityModifier), null, null);
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x000255C4 File Offset: 0x000237C4
		public override void Started()
		{
			float avgChaos = this._cfg.GetCVar<float>(CCVars.EventsRampingAverageChaos);
			float avgTime = this._cfg.GetCVar<float>(CCVars.EventsRampingAverageEndTime);
			this._maxChaos = this._random.NextFloat(avgChaos - avgChaos / 4f, avgChaos + avgChaos / 4f);
			this._endTime = this._random.NextFloat(avgTime - avgTime / 4f, avgTime + avgTime / 4f) * 60f;
			this._startingChaos = this._maxChaos / 10f;
			this.PickNextEventTime();
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x00025657 File Offset: 0x00023857
		public override void Ended()
		{
			this._endTime = 0f;
			this._maxChaos = 0f;
			this._startingChaos = 0f;
			this._timeUntilNextEvent = 0f;
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00025688 File Offset: 0x00023888
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (base.RuleStarted && (float)this._gameTicker.RoundDuration().TotalSeconds >= this._timeUntilCallShuttle && this._shuttleAnnouncement)
			{
				foreach (StationDataComponent comp in base.EntityQuery<StationDataComponent>(true))
				{
					this._chatSystem.DispatchStationAnnouncement(comp.Owner, Loc.GetString("emergency_shuttle_meteor_available"), "Central Command", true, null, null);
				}
				this._shuttleAnnouncement = false;
			}
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
			this._event.RunCertainEvent("MeteorSwarm");
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x00025780 File Offset: 0x00023980
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

		// Token: 0x06000795 RID: 1941 RVA: 0x000257D8 File Offset: 0x000239D8
		private void PickNextEventTime()
		{
			float mod = this.ChaosModifier;
			this._timeUntilNextEvent = this._random.NextFloat(240f / mod, 720f / mod);
		}

		// Token: 0x04000489 RID: 1161
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x0400048A RID: 1162
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400048B RID: 1163
		[Dependency]
		private readonly EventManagerSystem _event;

		// Token: 0x0400048C RID: 1164
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x0400048D RID: 1165
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x0400048E RID: 1166
		[ViewVariables]
		private float _endTime;

		// Token: 0x0400048F RID: 1167
		[ViewVariables]
		private float _maxChaos;

		// Token: 0x04000490 RID: 1168
		[ViewVariables]
		private float _startingChaos;

		// Token: 0x04000491 RID: 1169
		[ViewVariables]
		private float _timeUntilNextEvent;

		// Token: 0x04000492 RID: 1170
		[ViewVariables]
		public float _timeUntilCallShuttle = 1800f;

		// Token: 0x04000493 RID: 1171
		[ViewVariables]
		private bool _shuttleAnnouncement = true;
	}
}
