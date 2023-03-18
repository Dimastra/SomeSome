using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Interaction;
using Content.Server.MachineLinking.Components;
using Content.Server.Radio.EntitySystems;
using Content.Server.UserInterface;
using Content.Shared.Access.Systems;
using Content.Shared.MachineLinking;
using Content.Shared.Physics;
using Content.Shared.Radio;
using Content.Shared.TextScreen;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.MachineLinking.System
{
	// Token: 0x020003F3 RID: 1011
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SignalTimerSystem : EntitySystem
	{
		// Token: 0x060014B3 RID: 5299 RVA: 0x0006C564 File Offset: 0x0006A764
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SignalTimerComponent, ComponentInit>(new ComponentEventHandler<SignalTimerComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SignalTimerComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<SignalTimerComponent, AfterActivatableUIOpenEvent>(this.OnAfterActivatableUIOpen), null, null);
			base.SubscribeLocalEvent<SignalTimerComponent, SignalTimerTextChangedMessage>(new ComponentEventHandler<SignalTimerComponent, SignalTimerTextChangedMessage>(this.OnTextChangedMessage), null, null);
			base.SubscribeLocalEvent<SignalTimerComponent, SignalTimerDelayChangedMessage>(new ComponentEventHandler<SignalTimerComponent, SignalTimerDelayChangedMessage>(this.OnDelayChangedMessage), null, null);
			base.SubscribeLocalEvent<SignalTimerComponent, SignalTimerStartMessage>(new ComponentEventHandler<SignalTimerComponent, SignalTimerStartMessage>(this.OnTimerStartMessage), null, null);
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x0006C5DC File Offset: 0x0006A7DC
		private void Report(EntityUid source, string channelName, string messageKey, [Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})] params ValueTuple<string, object>[] args)
		{
			string message = (args.Length == 0) ? Loc.GetString(messageKey) : Loc.GetString(messageKey, args);
			RadioChannelPrototype channel = this._prototypeManager.Index<RadioChannelPrototype>(channelName);
			this._radioSystem.SendRadioMessage(source, message, channel, null);
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x0006C624 File Offset: 0x0006A824
		private void OnInit(EntityUid uid, SignalTimerComponent component, ComponentInit args)
		{
			SignalTransmitterComponent comp;
			if (base.TryComp<SignalTransmitterComponent>(uid, ref comp))
			{
				comp.Outputs.TryAdd(component.TriggerPort, new List<PortIdentifier>());
				comp.Outputs.TryAdd(component.StartPort, new List<PortIdentifier>());
			}
			this._appearanceSystem.SetData(uid, TextScreenVisuals.ScreenText, component.Label, null);
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x0006C684 File Offset: 0x0006A884
		private void OnAfterActivatableUIOpen(EntityUid uid, SignalTimerComponent component, AfterActivatableUIOpenEvent args)
		{
			ActiveSignalTimerComponent active;
			TimeSpan time = base.TryComp<ActiveSignalTimerComponent>(uid, ref active) ? active.TriggerTime : TimeSpan.Zero;
			BoundUserInterface bui;
			if (this._ui.TryGetUi(uid, SignalTimerUiKey.Key, ref bui, null))
			{
				this._ui.SetUiState(bui, new SignalTimerBoundUserInterfaceState(component.Label, TimeSpan.FromSeconds(component.Delay).Minutes.ToString("D2"), TimeSpan.FromSeconds(component.Delay).Seconds.ToString("D2"), component.CanEditLabel, time, active != null, new bool?(this._accessReader.IsAllowed(args.User, uid, null))), null, true);
			}
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x0006C740 File Offset: 0x0006A940
		public bool Trigger(EntityUid uid, SignalTimerComponent signalTimer)
		{
			base.RemComp<ActiveSignalTimerComponent>(uid);
			this._signalSystem.InvokePort(uid, signalTimer.TriggerPort, null);
			string announceMessage = signalTimer.Label;
			if (string.IsNullOrWhiteSpace(announceMessage))
			{
				announceMessage = Loc.GetString("label-none");
			}
			if (signalTimer.TimerCanAnnounce)
			{
				this.Report(uid, SignalTimerComponent.SecChannel, "timer-end-announcement", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("Label", announceMessage)
				});
			}
			this._appearanceSystem.SetData(uid, TextScreenVisuals.Mode, TextScreenMode.Text, null);
			BoundUserInterface bui;
			if (this._ui.TryGetUi(uid, SignalTimerUiKey.Key, ref bui, null))
			{
				this._ui.SetUiState(bui, new SignalTimerBoundUserInterfaceState(signalTimer.Label, TimeSpan.FromSeconds(signalTimer.Delay).Minutes.ToString("D2"), TimeSpan.FromSeconds(signalTimer.Delay).Seconds.ToString("D2"), signalTimer.CanEditLabel, TimeSpan.Zero, base.RemComp<ActiveSignalTimerComponent>(uid), null), null, true);
			}
			return true;
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x0006C859 File Offset: 0x0006AA59
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateTimer();
		}

		// Token: 0x060014B9 RID: 5305 RVA: 0x0006C868 File Offset: 0x0006AA68
		private void UpdateTimer()
		{
			foreach (ValueTuple<ActiveSignalTimerComponent, SignalTimerComponent> valueTuple in base.EntityQuery<ActiveSignalTimerComponent, SignalTimerComponent>(false))
			{
				ActiveSignalTimerComponent active = valueTuple.Item1;
				SignalTimerComponent timer = valueTuple.Item2;
				if (active.TriggerTime <= this._gameTiming.CurTime)
				{
					this.Trigger(timer.Owner, timer);
					if (timer.DoneSound != null)
					{
						Filter filter = Filter.Pvs(timer.Owner, 2f, this.EntityManager, null, null);
						this._audio.Play(timer.DoneSound, filter, timer.Owner, false, new AudioParams?(timer.SoundParams));
					}
				}
			}
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x0006C92C File Offset: 0x0006AB2C
		private bool IsMessageValid(EntityUid uid, BoundUserInterfaceMessage message)
		{
			EntityUid? attachedEntity = message.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid mob = attachedEntity.GetValueOrDefault();
				if (mob.Valid)
				{
					return this._interaction.InRangeUnobstructed(mob, uid, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && this._accessReader.IsAllowed(mob, uid, null);
				}
			}
			return false;
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x0006C994 File Offset: 0x0006AB94
		private void OnTextChangedMessage(EntityUid uid, SignalTimerComponent component, SignalTimerTextChangedMessage args)
		{
			if (!this.IsMessageValid(uid, args))
			{
				return;
			}
			component.Label = args.Text.Substring(0, Math.Min(5, args.Text.Length));
			this._appearanceSystem.SetData(uid, TextScreenVisuals.ScreenText, component.Label, null);
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x0006C9E8 File Offset: 0x0006ABE8
		private void OnDelayChangedMessage(EntityUid uid, SignalTimerComponent component, SignalTimerDelayChangedMessage args)
		{
			if (!this.IsMessageValid(uid, args))
			{
				return;
			}
			component.Delay = args.Delay.TotalSeconds;
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x0006CA14 File Offset: 0x0006AC14
		private void OnTimerStartMessage(EntityUid uid, SignalTimerComponent component, SignalTimerStartMessage args)
		{
			if (!this.IsMessageValid(uid, args))
			{
				return;
			}
			if (!base.HasComp<ActiveSignalTimerComponent>(uid))
			{
				ActiveSignalTimerComponent activeTimer = base.EnsureComp<ActiveSignalTimerComponent>(uid);
				activeTimer.TriggerTime = this._gameTiming.CurTime + TimeSpan.FromSeconds(component.Delay);
				component.User = new EntityUid?(args.User);
				this._appearanceSystem.SetData(uid, TextScreenVisuals.Mode, TextScreenMode.Timer, null);
				this._appearanceSystem.SetData(uid, TextScreenVisuals.TargetTime, activeTimer.TriggerTime, null);
				this._appearanceSystem.SetData(uid, TextScreenVisuals.ScreenText, component.Label, null);
				this._signalSystem.InvokePort(uid, component.StartPort, null);
				string announceMessage = component.Label;
				if (string.IsNullOrWhiteSpace(announceMessage))
				{
					announceMessage = Loc.GetString("label-none");
				}
				TimeSpan time = TimeSpan.FromSeconds(component.Delay);
				string format = "{0}{1}{2}";
				object arg;
				if (time.Duration().Hours <= 0)
				{
					arg = string.Empty;
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
					defaultInterpolatedStringHandler.AppendFormatted<int>(time.Hours, "0");
					defaultInterpolatedStringHandler.AppendLiteral(" час;");
					defaultInterpolatedStringHandler.AppendFormatted((time.Hours == 1) ? string.Empty : "ов");
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					arg = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				object arg2;
				if (time.Duration().Minutes <= 0)
				{
					arg2 = string.Empty;
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
					defaultInterpolatedStringHandler.AppendFormatted<int>(time.Minutes, "0");
					defaultInterpolatedStringHandler.AppendLiteral(" минут;");
					defaultInterpolatedStringHandler.AppendFormatted((time.Minutes != 1) ? string.Empty : "а");
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					arg2 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				object arg3;
				if (time.Duration().Seconds <= 0)
				{
					arg3 = string.Empty;
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
					defaultInterpolatedStringHandler.AppendFormatted<int>(time.Seconds, "0");
					defaultInterpolatedStringHandler.AppendLiteral(" секунд");
					defaultInterpolatedStringHandler.AppendFormatted((time.Seconds != 1) ? string.Empty : "а");
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					arg3 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				string timeFormatted = string.Format(format, arg, arg2, arg3);
				if (component.TimerCanAnnounce)
				{
					this.Report(uid, SignalTimerComponent.SecChannel, "timer-start-announcement", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("Label", announceMessage),
						new ValueTuple<string, object>("Time", timeFormatted)
					});
					return;
				}
			}
			else
			{
				component.User = new EntityUid?(args.User);
				base.RemComp<ActiveSignalTimerComponent>(uid);
				this._appearanceSystem.SetData(uid, TextScreenVisuals.Mode, TextScreenMode.Text, null);
				this._appearanceSystem.SetData(uid, TextScreenVisuals.ScreenText, component.Label, null);
				string announceMessage2 = component.Label;
				if (string.IsNullOrWhiteSpace(announceMessage2))
				{
					announceMessage2 = Loc.GetString("label-none");
				}
				if (component.TimerCanAnnounce)
				{
					this.Report(uid, SignalTimerComponent.SecChannel, "timer-suffer-end", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("Label", announceMessage2)
					});
				}
			}
		}

		// Token: 0x04000CC9 RID: 3273
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000CCA RID: 3274
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000CCB RID: 3275
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x04000CCC RID: 3276
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x04000CCD RID: 3277
		[Dependency]
		private readonly UserInterfaceSystem _ui;

		// Token: 0x04000CCE RID: 3278
		[Dependency]
		private readonly AccessReaderSystem _accessReader;

		// Token: 0x04000CCF RID: 3279
		[Dependency]
		private readonly InteractionSystem _interaction;

		// Token: 0x04000CD0 RID: 3280
		[Dependency]
		private readonly RadioSystem _radioSystem;

		// Token: 0x04000CD1 RID: 3281
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;
	}
}
