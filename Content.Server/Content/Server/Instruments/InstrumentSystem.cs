using System;
using System.Runtime.CompilerServices;
using Content.Server.Stunnable;
using Content.Server.UserInterface;
using Content.Shared.CCVar;
using Content.Shared.Instruments;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Instruments
{
	// Token: 0x0200044B RID: 1099
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InstrumentSystem : SharedInstrumentSystem
	{
		// Token: 0x0600161C RID: 5660 RVA: 0x00074E30 File Offset: 0x00073030
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeCVars();
			base.SubscribeNetworkEvent<InstrumentMidiEventEvent>(new EntitySessionEventHandler<InstrumentMidiEventEvent>(this.OnMidiEventRx), null, null);
			base.SubscribeNetworkEvent<InstrumentStartMidiEvent>(new EntitySessionEventHandler<InstrumentStartMidiEvent>(this.OnMidiStart), null, null);
			base.SubscribeNetworkEvent<InstrumentStopMidiEvent>(new EntitySessionEventHandler<InstrumentStopMidiEvent>(this.OnMidiStop), null, null);
			base.SubscribeLocalEvent<InstrumentComponent, BoundUIClosedEvent>(new ComponentEventHandler<InstrumentComponent, BoundUIClosedEvent>(this.OnBoundUIClosed), null, null);
			base.SubscribeLocalEvent<InstrumentComponent, BoundUIOpenedEvent>(new ComponentEventHandler<InstrumentComponent, BoundUIOpenedEvent>(this.OnBoundUIOpened), null, null);
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x00074EB0 File Offset: 0x000730B0
		private void OnMidiStart(InstrumentStartMidiEvent msg, EntitySessionEventArgs args)
		{
			EntityUid uid = msg.Uid;
			InstrumentComponent instrument;
			if (!this.EntityManager.TryGetComponent<InstrumentComponent>(uid, ref instrument))
			{
				return;
			}
			if (args.SenderSession != instrument.InstrumentPlayer)
			{
				return;
			}
			instrument.Playing = true;
			instrument.Dirty(null);
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x00074EF4 File Offset: 0x000730F4
		private void OnMidiStop(InstrumentStopMidiEvent msg, EntitySessionEventArgs args)
		{
			EntityUid uid = msg.Uid;
			InstrumentComponent instrument;
			if (!this.EntityManager.TryGetComponent<InstrumentComponent>(uid, ref instrument))
			{
				return;
			}
			if (args.SenderSession != instrument.InstrumentPlayer)
			{
				return;
			}
			this.Clean(uid, instrument);
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x00074F31 File Offset: 0x00073131
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownCVars();
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x00074F40 File Offset: 0x00073140
		private void OnBoundUIClosed(EntityUid uid, InstrumentComponent component, BoundUIClosedEvent args)
		{
			if (!(args.UiKey is InstrumentUiKey))
			{
				return;
			}
			BoundUserInterface bui;
			if (base.HasComp<ActiveInstrumentComponent>(uid) && this._userInterfaceSystem.TryGetUi(uid, args.UiKey, ref bui, null) && bui.SubscribedSessions.Count == 0)
			{
				base.RemComp<ActiveInstrumentComponent>(uid);
			}
			this.Clean(uid, component);
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x00074F98 File Offset: 0x00073198
		private void OnBoundUIOpened(EntityUid uid, InstrumentComponent component, BoundUIOpenedEvent args)
		{
			if (!(args.UiKey is InstrumentUiKey))
			{
				return;
			}
			base.EnsureComp<ActiveInstrumentComponent>(uid);
			this.Clean(uid, component);
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x00074FB8 File Offset: 0x000731B8
		[NullableContext(2)]
		public void Clean(EntityUid uid, InstrumentComponent instrument = null)
		{
			if (!base.Resolve<InstrumentComponent>(uid, ref instrument, true))
			{
				return;
			}
			if (instrument.Playing)
			{
				base.RaiseNetworkEvent(new InstrumentStopMidiEvent(uid));
			}
			instrument.Playing = false;
			instrument.LastSequencerTick = 0U;
			instrument.BatchesDropped = 0;
			instrument.LaggedBatches = 0;
			instrument.Dirty(null);
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x0007500C File Offset: 0x0007320C
		private void OnMidiEventRx(InstrumentMidiEventEvent msg, EntitySessionEventArgs args)
		{
			EntityUid uid = msg.Uid;
			InstrumentComponent instrument;
			if (!base.TryComp<InstrumentComponent>(uid, ref instrument))
			{
				return;
			}
			if (instrument.Playing && args.SenderSession == instrument.InstrumentPlayer && instrument.InstrumentPlayer != null)
			{
				EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid attached = attachedEntity.GetValueOrDefault();
					bool send = true;
					uint minTick = uint.MaxValue;
					uint maxTick = 0U;
					for (int i = 0; i < msg.MidiEvent.Length; i++)
					{
						uint tick = msg.MidiEvent[i].Tick;
						if (tick < minTick)
						{
							minTick = tick;
						}
						if (tick > maxTick)
						{
							maxTick = tick;
						}
					}
					if (instrument.LastSequencerTick > minTick)
					{
						instrument.LaggedBatches++;
						if (instrument.RespectMidiLimits)
						{
							if (instrument.LaggedBatches == (int)((double)this.MaxMidiLaggedBatches * 0.3333333333333333 + 1.0))
							{
								attached.PopupMessage(Loc.GetString("instrument-component-finger-cramps-light-message"));
							}
							else if (instrument.LaggedBatches == (int)((double)this.MaxMidiLaggedBatches * 0.6666666666666666 + 1.0))
							{
								attached.PopupMessage(Loc.GetString("instrument-component-finger-cramps-serious-message"));
							}
						}
						if (instrument.LaggedBatches > this.MaxMidiLaggedBatches)
						{
							send = false;
						}
					}
					InstrumentComponent instrumentComponent = instrument;
					int num = instrumentComponent.MidiEventCount + 1;
					instrumentComponent.MidiEventCount = num;
					if (num > this.MaxMidiEventsPerSecond || msg.MidiEvent.Length > this.MaxMidiEventsPerBatch)
					{
						instrument.BatchesDropped++;
						send = false;
					}
					if (send || !instrument.RespectMidiLimits)
					{
						base.RaiseNetworkEvent(msg);
					}
					instrument.LastSequencerTick = Math.Max(maxTick, minTick);
					return;
				}
			}
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x000751B0 File Offset: 0x000733B0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveInstrumentComponent, InstrumentComponent> valueTuple in this.EntityManager.EntityQuery<ActiveInstrumentComponent, InstrumentComponent>(true))
			{
				InstrumentComponent instrument = valueTuple.Item2;
				if (instrument.DirtyRenderer)
				{
					base.Dirty(instrument, null);
					instrument.DirtyRenderer = false;
				}
				if (instrument.RespectMidiLimits && (instrument.BatchesDropped >= this.MaxMidiBatchesDropped || instrument.LaggedBatches >= this.MaxMidiLaggedBatches))
				{
					IPlayerSession instrumentPlayer = instrument.InstrumentPlayer;
					EntityUid? entityUid = (instrumentPlayer != null) ? instrumentPlayer.AttachedEntity : null;
					if (entityUid != null)
					{
						EntityUid mob = entityUid.GetValueOrDefault();
						if (mob.Valid)
						{
							this._stunSystem.TryParalyze(mob, TimeSpan.FromSeconds(1.0), true, null);
							instrument.Owner.PopupMessage(mob, Loc.GetString("instrument-component-finger-cramps-max-message"));
						}
					}
					this.Clean(instrument.Owner, null);
					BoundUserInterface userInterface = instrument.UserInterface;
					if (userInterface != null)
					{
						userInterface.CloseAll();
					}
				}
				instrument.Timer += frameTime;
				if (instrument.Timer >= 1f)
				{
					instrument.Timer = 0f;
					instrument.MidiEventCount = 0;
					instrument.LaggedBatches = 0;
					instrument.BatchesDropped = 0;
				}
			}
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x00075320 File Offset: 0x00073520
		public void ToggleInstrumentUi(EntityUid uid, IPlayerSession session, [Nullable(2)] InstrumentComponent component = null)
		{
			if (!base.Resolve<InstrumentComponent>(uid, ref component, true))
			{
				return;
			}
			BoundUserInterface uiorNull = uid.GetUIOrNull(InstrumentUiKey.Key);
			if (uiorNull == null)
			{
				return;
			}
			uiorNull.Toggle(session);
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06001626 RID: 5670 RVA: 0x00075346 File Offset: 0x00073546
		// (set) Token: 0x06001627 RID: 5671 RVA: 0x0007534E File Offset: 0x0007354E
		public int MaxMidiEventsPerSecond { get; private set; }

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06001628 RID: 5672 RVA: 0x00075357 File Offset: 0x00073557
		// (set) Token: 0x06001629 RID: 5673 RVA: 0x0007535F File Offset: 0x0007355F
		public int MaxMidiEventsPerBatch { get; private set; }

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x0600162A RID: 5674 RVA: 0x00075368 File Offset: 0x00073568
		// (set) Token: 0x0600162B RID: 5675 RVA: 0x00075370 File Offset: 0x00073570
		public int MaxMidiBatchesDropped { get; private set; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x0600162C RID: 5676 RVA: 0x00075379 File Offset: 0x00073579
		// (set) Token: 0x0600162D RID: 5677 RVA: 0x00075381 File Offset: 0x00073581
		public int MaxMidiLaggedBatches { get; private set; }

		// Token: 0x0600162E RID: 5678 RVA: 0x0007538C File Offset: 0x0007358C
		private void InitializeCVars()
		{
			this._cfg.OnValueChanged<int>(CCVars.MaxMidiEventsPerSecond, new Action<int>(this.OnMaxMidiEventsPerSecondChanged), true);
			this._cfg.OnValueChanged<int>(CCVars.MaxMidiEventsPerBatch, new Action<int>(this.OnMaxMidiEventsPerBatchChanged), true);
			this._cfg.OnValueChanged<int>(CCVars.MaxMidiBatchesDropped, new Action<int>(this.OnMaxMidiBatchesDroppedChanged), true);
			this._cfg.OnValueChanged<int>(CCVars.MaxMidiLaggedBatches, new Action<int>(this.OnMaxMidiLaggedBatchesChanged), true);
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x00075410 File Offset: 0x00073610
		private void ShutdownCVars()
		{
			this._cfg.UnsubValueChanged<int>(CCVars.MaxMidiEventsPerSecond, new Action<int>(this.OnMaxMidiEventsPerSecondChanged));
			this._cfg.UnsubValueChanged<int>(CCVars.MaxMidiEventsPerBatch, new Action<int>(this.OnMaxMidiEventsPerBatchChanged));
			this._cfg.UnsubValueChanged<int>(CCVars.MaxMidiBatchesDropped, new Action<int>(this.OnMaxMidiBatchesDroppedChanged));
			this._cfg.UnsubValueChanged<int>(CCVars.MaxMidiLaggedBatches, new Action<int>(this.OnMaxMidiLaggedBatchesChanged));
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x0007548D File Offset: 0x0007368D
		private void OnMaxMidiLaggedBatchesChanged(int obj)
		{
			this.MaxMidiLaggedBatches = obj;
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x00075496 File Offset: 0x00073696
		private void OnMaxMidiBatchesDroppedChanged(int obj)
		{
			this.MaxMidiBatchesDropped = obj;
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x0007549F File Offset: 0x0007369F
		private void OnMaxMidiEventsPerBatchChanged(int obj)
		{
			this.MaxMidiEventsPerBatch = obj;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x000754A8 File Offset: 0x000736A8
		private void OnMaxMidiEventsPerSecondChanged(int obj)
		{
			this.MaxMidiEventsPerSecond = obj;
		}

		// Token: 0x04000DE3 RID: 3555
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000DE4 RID: 3556
		[Dependency]
		private readonly StunSystem _stunSystem;

		// Token: 0x04000DE5 RID: 3557
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;
	}
}
