using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.CCVar;
using Content.Shared.Instruments;
using Robust.Client.Audio.Midi;
using Robust.Shared.Audio.Midi;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client.Instruments
{
	// Token: 0x020002AB RID: 683
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InstrumentSystem : SharedInstrumentSystem
	{
		// Token: 0x170003BB RID: 955
		// (get) Token: 0x0600114F RID: 4431 RVA: 0x00066A04 File Offset: 0x00064C04
		// (set) Token: 0x06001150 RID: 4432 RVA: 0x00066A0C File Offset: 0x00064C0C
		public int MaxMidiEventsPerBatch { get; private set; }

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06001151 RID: 4433 RVA: 0x00066A15 File Offset: 0x00064C15
		// (set) Token: 0x06001152 RID: 4434 RVA: 0x00066A1D File Offset: 0x00064C1D
		public int MaxMidiEventsPerSecond { get; private set; }

		// Token: 0x06001153 RID: 4435 RVA: 0x00066A28 File Offset: 0x00064C28
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
			this._cfg.OnValueChanged<int>(CCVars.MaxMidiEventsPerBatch, new Action<int>(this.OnMaxMidiEventsPerBatchChanged), true);
			this._cfg.OnValueChanged<int>(CCVars.MaxMidiEventsPerSecond, new Action<int>(this.OnMaxMidiEventsPerSecondChanged), true);
			base.SubscribeNetworkEvent<InstrumentMidiEventEvent>(new EntityEventHandler<InstrumentMidiEventEvent>(this.OnMidiEventRx), null, null);
			base.SubscribeNetworkEvent<InstrumentStartMidiEvent>(new EntityEventHandler<InstrumentStartMidiEvent>(this.OnMidiStart), null, null);
			base.SubscribeNetworkEvent<InstrumentStopMidiEvent>(new EntityEventHandler<InstrumentStopMidiEvent>(this.OnMidiStop), null, null);
			base.SubscribeLocalEvent<InstrumentComponent, ComponentShutdown>(new ComponentEventHandler<InstrumentComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x00066ACC File Offset: 0x00064CCC
		public override void Shutdown()
		{
			base.Shutdown();
			this._cfg.UnsubValueChanged<int>(CCVars.MaxMidiEventsPerBatch, new Action<int>(this.OnMaxMidiEventsPerBatchChanged));
			this._cfg.UnsubValueChanged<int>(CCVars.MaxMidiEventsPerSecond, new Action<int>(this.OnMaxMidiEventsPerSecondChanged));
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x00066B0C File Offset: 0x00064D0C
		private void OnShutdown(EntityUid uid, InstrumentComponent component, ComponentShutdown args)
		{
			this.EndRenderer(uid, false, component);
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x00066B18 File Offset: 0x00064D18
		[NullableContext(2)]
		public override void SetupRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent component = null)
		{
			if (!base.Resolve<SharedInstrumentComponent>(uid, ref component, true))
			{
				return;
			}
			InstrumentComponent instrument = component as InstrumentComponent;
			if (instrument == null || instrument.IsRendererAlive)
			{
				return;
			}
			instrument.SequenceDelay = 0U;
			instrument.SequenceStartTick = 0U;
			this._midiManager.OcclusionCollisionMask = 2;
			instrument.Renderer = this._midiManager.GetNewRenderer(true);
			if (instrument.Renderer != null)
			{
				instrument.Renderer.SendMidiEvent(RobustMidiEvent.SystemReset(instrument.Renderer.SequencerTick));
				this.UpdateRenderer(uid, instrument);
				instrument.Renderer.OnMidiPlayerFinished += delegate()
				{
					instrument.PlaybackEndedInvoke();
					this.EndRenderer(uid, fromStateChange, instrument);
				};
			}
			if (!fromStateChange)
			{
				base.RaiseNetworkEvent(new InstrumentStartMidiEvent(uid));
			}
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x00066C28 File Offset: 0x00064E28
		[NullableContext(2)]
		public void UpdateRenderer(EntityUid uid, InstrumentComponent instrument = null)
		{
			if (!base.Resolve<InstrumentComponent>(uid, ref instrument, true) || instrument.Renderer == null)
			{
				return;
			}
			instrument.Renderer.TrackingEntity = new EntityUid?(instrument.Owner);
			instrument.Renderer.DisablePercussionChannel = !instrument.AllowPercussion;
			instrument.Renderer.DisableProgramChangeEvent = !instrument.AllowProgramChange;
			if (!instrument.AllowProgramChange)
			{
				instrument.Renderer.MidiBank = instrument.InstrumentBank;
				instrument.Renderer.MidiProgram = instrument.InstrumentProgram;
			}
			instrument.Renderer.LoopMidi = instrument.LoopMidi;
			instrument.DirtyRenderer = false;
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x00066CCC File Offset: 0x00064ECC
		[NullableContext(2)]
		public override void EndRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent component = null)
		{
			if (!base.Resolve<SharedInstrumentComponent>(uid, ref component, false))
			{
				return;
			}
			InstrumentComponent instrumentComponent = component as InstrumentComponent;
			if (instrumentComponent == null)
			{
				return;
			}
			if (instrumentComponent.IsInputOpen)
			{
				this.CloseInput(uid, fromStateChange, instrumentComponent);
				return;
			}
			if (instrumentComponent.IsMidiOpen)
			{
				this.CloseMidi(uid, fromStateChange, instrumentComponent);
				return;
			}
			IMidiRenderer renderer3 = instrumentComponent.Renderer;
			if (renderer3 != null)
			{
				renderer3.StopAllNotes();
			}
			IMidiRenderer renderer = instrumentComponent.Renderer;
			Timer.Spawn(2000, delegate()
			{
				IMidiRenderer renderer2 = renderer;
				if (renderer2 == null)
				{
					return;
				}
				renderer2.Dispose();
			}, default(CancellationToken));
			instrumentComponent.Renderer = null;
			instrumentComponent.MidiEventBuffer.Clear();
			if (!fromStateChange && this._netManager.IsConnected)
			{
				base.RaiseNetworkEvent(new InstrumentStopMidiEvent(uid));
			}
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x00066D88 File Offset: 0x00064F88
		[NullableContext(2)]
		public void SetPlayerTick(EntityUid uid, int playerTick, InstrumentComponent instrument = null)
		{
			if (!base.Resolve<InstrumentComponent>(uid, ref instrument, true))
			{
				return;
			}
			IMidiRenderer renderer = instrument.Renderer;
			if (renderer == null || renderer.Status != 2)
			{
				return;
			}
			instrument.MidiEventBuffer.Clear();
			uint num = instrument.Renderer.SequencerTick - 1U;
			instrument.MidiEventBuffer.Add(RobustMidiEvent.SystemReset(num));
			instrument.Renderer.PlayerTick = playerTick;
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x00066DEC File Offset: 0x00064FEC
		[NullableContext(2)]
		public bool OpenInput(EntityUid uid, InstrumentComponent instrument = null)
		{
			if (!base.Resolve<InstrumentComponent>(uid, ref instrument, false))
			{
				return false;
			}
			this.SetupRenderer(uid, false, instrument);
			if (instrument.Renderer != null && instrument.Renderer.OpenInput())
			{
				instrument.Renderer.OnMidiEvent += instrument.MidiEventBuffer.Add;
				return true;
			}
			return false;
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x00066E48 File Offset: 0x00065048
		[NullableContext(0)]
		public bool OpenMidi(EntityUid uid, ReadOnlySpan<byte> data, [Nullable(2)] InstrumentComponent instrument = null)
		{
			if (!base.Resolve<InstrumentComponent>(uid, ref instrument, true))
			{
				return false;
			}
			this.SetupRenderer(uid, false, instrument);
			if (instrument.Renderer == null || !instrument.Renderer.OpenMidi(data))
			{
				return false;
			}
			instrument.MidiEventBuffer.Clear();
			instrument.Renderer.OnMidiEvent += instrument.MidiEventBuffer.Add;
			return true;
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x00066EAD File Offset: 0x000650AD
		[NullableContext(2)]
		public bool CloseInput(EntityUid uid, bool fromStateChange, InstrumentComponent instrument = null)
		{
			if (!base.Resolve<InstrumentComponent>(uid, ref instrument, true))
			{
				return false;
			}
			if (instrument.Renderer == null || !instrument.Renderer.CloseInput())
			{
				return false;
			}
			this.EndRenderer(uid, fromStateChange, instrument);
			return true;
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x00066EDE File Offset: 0x000650DE
		[NullableContext(2)]
		public bool CloseMidi(EntityUid uid, bool fromStateChange, InstrumentComponent instrument = null)
		{
			if (!base.Resolve<InstrumentComponent>(uid, ref instrument, true))
			{
				return false;
			}
			if (instrument.Renderer == null || !instrument.Renderer.CloseMidi())
			{
				return false;
			}
			this.EndRenderer(uid, fromStateChange, instrument);
			return true;
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x00066F0F File Offset: 0x0006510F
		private void OnMaxMidiEventsPerSecondChanged(int obj)
		{
			this.MaxMidiEventsPerSecond = obj;
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x00066F18 File Offset: 0x00065118
		private void OnMaxMidiEventsPerBatchChanged(int obj)
		{
			this.MaxMidiEventsPerBatch = obj;
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x00066F24 File Offset: 0x00065124
		private void OnMidiEventRx(InstrumentMidiEventEvent midiEv)
		{
			EntityUid uid = midiEv.Uid;
			InstrumentComponent instrumentComponent;
			if (!base.TryComp<InstrumentComponent>(uid, ref instrumentComponent))
			{
				return;
			}
			IMidiRenderer renderer = instrumentComponent.Renderer;
			if (renderer == null)
			{
				if (instrumentComponent.SequenceStartTick == 0U)
				{
					this.SetupRenderer(uid, true, instrumentComponent);
				}
				return;
			}
			if (instrumentComponent.IsInputOpen || instrumentComponent.IsMidiOpen)
			{
				return;
			}
			if (instrumentComponent.SequenceStartTick <= 0U)
			{
				instrumentComponent.SequenceStartTick = midiEv.MidiEvent.Min((RobustMidiEvent x) => x.Tick) - 1U;
			}
			INetChannel serverChannel = this._netManager.ServerChannel;
			float num = MathF.Sqrt((float)((serverChannel != null) ? serverChannel.Ping : 0) / 1000f);
			uint val = (uint)(renderer.SequencerTimeScale * (0.2 + (double)num)) - instrumentComponent.SequenceStartTick;
			instrumentComponent.SequenceDelay = Math.Max(instrumentComponent.SequenceDelay, val);
			uint sequencerTick = renderer.SequencerTick;
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)midiEv.MidiEvent.Length))
			{
				RobustMidiEvent robustMidiEvent = midiEv.MidiEvent[(int)num2];
				uint num3 = robustMidiEvent.Tick + instrumentComponent.SequenceDelay;
				if (num3 < sequencerTick)
				{
					instrumentComponent.SequenceDelay += sequencerTick - robustMidiEvent.Tick;
					num3 = robustMidiEvent.Tick + instrumentComponent.SequenceDelay;
				}
				IMidiRenderer renderer2 = instrumentComponent.Renderer;
				if (renderer2 != null)
				{
					renderer2.ScheduleMidiEvent(robustMidiEvent, num3 + num2, true);
				}
				num2 += 1U;
			}
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x00067086 File Offset: 0x00065286
		private void OnMidiStart(InstrumentStartMidiEvent ev)
		{
			this.SetupRenderer(ev.Uid, true, null);
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x00067096 File Offset: 0x00065296
		private void OnMidiStop(InstrumentStopMidiEvent ev)
		{
			this.EndRenderer(ev.Uid, true, null);
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x000670A8 File Offset: 0x000652A8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._gameTiming.IsFirstTimePredicted)
			{
				return;
			}
			foreach (InstrumentComponent instrumentComponent in this.EntityManager.EntityQuery<InstrumentComponent>(true))
			{
				if (instrumentComponent.DirtyRenderer && instrumentComponent.Renderer != null)
				{
					this.UpdateRenderer(instrumentComponent.Owner, instrumentComponent);
				}
				if (instrumentComponent.IsMidiOpen || instrumentComponent.IsInputOpen)
				{
					TimeSpan realTime = this._gameTiming.RealTime;
					TimeSpan t = realTime.Add(this.OneSecAgo);
					if (instrumentComponent.LastMeasured <= t)
					{
						instrumentComponent.LastMeasured = realTime;
						instrumentComponent.SentWithinASec = 0;
					}
					if (instrumentComponent.MidiEventBuffer.Count != 0)
					{
						int num = instrumentComponent.RespectMidiLimits ? Math.Min(this.MaxMidiEventsPerBatch, this.MaxMidiEventsPerSecond - instrumentComponent.SentWithinASec) : instrumentComponent.MidiEventBuffer.Count;
						if (num > 0)
						{
							double num2 = (instrumentComponent.IsRendererAlive && instrumentComponent.Renderer.Status != null) ? (instrumentComponent.Renderer.SequencerTimeScale * 0.20000000298023224) : 0.0;
							double bufferedTick = instrumentComponent.IsRendererAlive ? (instrumentComponent.Renderer.SequencerTick - num2) : 2147483647.0;
							RobustMidiEvent[] array = instrumentComponent.MidiEventBuffer.TakeWhile((RobustMidiEvent x) => x.Tick < bufferedTick).Take(num).ToArray<RobustMidiEvent>();
							int num3 = array.Length;
							if (num3 != 0)
							{
								base.RaiseNetworkEvent(new InstrumentMidiEventEvent(instrumentComponent.Owner, array));
								instrumentComponent.SentWithinASec += num3;
								instrumentComponent.MidiEventBuffer.RemoveRange(0, num3);
							}
						}
					}
				}
			}
		}

		// Token: 0x04000880 RID: 2176
		[Dependency]
		private readonly IClientNetManager _netManager;

		// Token: 0x04000881 RID: 2177
		[Dependency]
		private readonly IMidiManager _midiManager;

		// Token: 0x04000882 RID: 2178
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000883 RID: 2179
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000884 RID: 2180
		public readonly TimeSpan OneSecAgo = TimeSpan.FromSeconds(-1.0);
	}
}
