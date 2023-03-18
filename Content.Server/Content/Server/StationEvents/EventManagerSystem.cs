using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.StationEvents
{
	// Token: 0x0200017D RID: 381
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EventManagerSystem : EntitySystem
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x00024F45 File Offset: 0x00023145
		// (set) Token: 0x0600077E RID: 1918 RVA: 0x00024F4D File Offset: 0x0002314D
		public bool EventsEnabled { get; private set; }

		// Token: 0x0600077F RID: 1919 RVA: 0x00024F56 File Offset: 0x00023156
		private void SetEnabled(bool value)
		{
			this.EventsEnabled = value;
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x00024F5F File Offset: 0x0002315F
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("events");
			this._configurationManager.OnValueChanged<bool>(CCVars.EventsEnabled, new Action<bool>(this.SetEnabled), true);
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x00024F94 File Offset: 0x00023194
		public override void Shutdown()
		{
			base.Shutdown();
			this._configurationManager.UnsubValueChanged<bool>(CCVars.EventsEnabled, new Action<bool>(this.SetEnabled));
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x00024FB8 File Offset: 0x000231B8
		public string RunRandomEvent()
		{
			StationEventRuleConfiguration randomEvent = this.PickRandomEvent();
			GameRulePrototype proto;
			if (randomEvent == null || !this._prototype.TryIndex<GameRulePrototype>(randomEvent.Id, ref proto))
			{
				string errStr = Loc.GetString("station-event-system-run-random-event-no-valid-events");
				this._sawmill.Error(errStr);
				return errStr;
			}
			this.GameTicker.AddGameRule(proto);
			string str = Loc.GetString("station-event-system-run-event", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("eventName", randomEvent.Id)
			});
			this._sawmill.Info(str);
			return str;
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x00025040 File Offset: 0x00023240
		[NullableContext(2)]
		public StationEventRuleConfiguration PickRandomEvent()
		{
			List<StationEventRuleConfiguration> availableEvents = this.AvailableEvents(false);
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Picking from ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(availableEvents.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" total available events");
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return this.FindEvent(availableEvents);
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x000250A0 File Offset: 0x000232A0
		public void RunCertainEvent(string eventId)
		{
			StationEventRuleConfiguration certainEvent = this.PickCertainEvent(eventId);
			GameRulePrototype proto;
			if (certainEvent == null || !this._prototype.TryIndex<GameRulePrototype>(certainEvent.Id, ref proto))
			{
				string errStr = Loc.GetString("station-event-system-run-random-event-no-valid-events");
				this._sawmill.Error(errStr);
				return;
			}
			this.GameTicker.AddGameRule(proto);
			string str = Loc.GetString("station-event-system-run-event", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("eventName", certainEvent.Id)
			});
			this._sawmill.Info(str);
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x00025128 File Offset: 0x00023328
		[return: Nullable(2)]
		public StationEventRuleConfiguration PickCertainEvent(string eventId)
		{
			List<StationEventRuleConfiguration> availableEvent = this.AvailableCertainEvent(eventId);
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Picking from ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(availableEvent.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" total available events");
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return this.FindEvent(availableEvent);
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x00025188 File Offset: 0x00023388
		[return: Nullable(2)]
		private StationEventRuleConfiguration FindEvent(List<StationEventRuleConfiguration> availableEvents)
		{
			if (availableEvents.Count == 0)
			{
				this._sawmill.Warning("No events were available to run!");
				return null;
			}
			int sumOfWeights = 0;
			foreach (StationEventRuleConfiguration stationEvent in availableEvents)
			{
				sumOfWeights += (int)stationEvent.Weight;
			}
			sumOfWeights = this._random.Next(sumOfWeights);
			foreach (StationEventRuleConfiguration stationEvent2 in availableEvents)
			{
				sumOfWeights -= (int)stationEvent2.Weight;
				if (sumOfWeights <= 0)
				{
					return stationEvent2;
				}
			}
			this._sawmill.Error("Event was not found after weighted pick process!");
			return null;
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x00025260 File Offset: 0x00023460
		private List<StationEventRuleConfiguration> AvailableEvents(bool ignoreEarliestStart = false)
		{
			int playerCount = this._playerManager.PlayerCount;
			TimeSpan currentTime;
			if (!ignoreEarliestStart)
			{
				currentTime = this.GameTicker.RoundDuration();
			}
			else
			{
				currentTime = TimeSpan.Zero;
			}
			List<StationEventRuleConfiguration> result = new List<StationEventRuleConfiguration>();
			foreach (StationEventRuleConfiguration stationEvent in this.AllEvents())
			{
				if (this.CanRun(stationEvent, playerCount, currentTime))
				{
					this._sawmill.Debug("Adding event " + stationEvent.Id + " to possibilities");
					result.Add(stationEvent);
				}
			}
			return result;
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x00025308 File Offset: 0x00023508
		private IEnumerable<StationEventRuleConfiguration> AllEvents()
		{
			return from p in this._prototype.EnumeratePrototypes<GameRulePrototype>()
			where p.Configuration is StationEventRuleConfiguration
			select (StationEventRuleConfiguration)p.Configuration;
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x00025368 File Offset: 0x00023568
		private int GetOccurrences(StationEventRuleConfiguration stationEvent)
		{
			return this.GameTicker.AllPreviousGameRules.Count((ValueTuple<TimeSpan, GameRulePrototype> p) => p.Item2.ID == stationEvent.Id);
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x000253A0 File Offset: 0x000235A0
		[NullableContext(2)]
		public TimeSpan TimeSinceLastEvent(StationEventRuleConfiguration stationEvent)
		{
			foreach (ValueTuple<TimeSpan, GameRulePrototype> valueTuple in this.GameTicker.AllPreviousGameRules.Reverse<ValueTuple<TimeSpan, GameRulePrototype>>())
			{
				TimeSpan time = valueTuple.Item1;
				GameRulePrototype rule = valueTuple.Item2;
				if (rule.Configuration is StationEventRuleConfiguration && (stationEvent == null || rule.ID == stationEvent.Id))
				{
					return time;
				}
			}
			return TimeSpan.Zero;
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x0002542C File Offset: 0x0002362C
		private bool CanRun(StationEventRuleConfiguration stationEvent, int playerCount, TimeSpan currentTime)
		{
			if (this.GameTicker.IsGameRuleStarted(stationEvent.Id))
			{
				return false;
			}
			if (stationEvent.MaxOccurrences != null && this.GetOccurrences(stationEvent) >= stationEvent.MaxOccurrences.Value)
			{
				return false;
			}
			if (playerCount < stationEvent.MinimumPlayers)
			{
				return false;
			}
			if (currentTime != TimeSpan.Zero && currentTime.TotalMinutes < (double)stationEvent.EarliestStart)
			{
				return false;
			}
			TimeSpan lastRun = this.TimeSinceLastEvent(stationEvent);
			return !(lastRun != TimeSpan.Zero) || currentTime.TotalMinutes >= (double)stationEvent.ReoccurrenceDelay + lastRun.TotalMinutes;
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x000254CC File Offset: 0x000236CC
		private List<StationEventRuleConfiguration> AvailableCertainEvent(string eventId)
		{
			List<StationEventRuleConfiguration> result = new List<StationEventRuleConfiguration>();
			foreach (StationEventRuleConfiguration stationEvent in this.AllEvents())
			{
				if (stationEvent.Id == eventId)
				{
					this._sawmill.Debug("Adding event " + stationEvent.Id + " to possibilities");
					result.Add(stationEvent);
				}
			}
			return result;
		}

		// Token: 0x04000482 RID: 1154
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000483 RID: 1155
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000484 RID: 1156
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000485 RID: 1157
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000486 RID: 1158
		[Dependency]
		public readonly GameTicker GameTicker;

		// Token: 0x04000487 RID: 1159
		private ISawmill _sawmill;
	}
}
