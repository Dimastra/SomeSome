using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Instruments;
using Robust.Client.Audio.Midi;
using Robust.Shared.Audio.Midi;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Instruments
{
	// Token: 0x020002AA RID: 682
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedInstrumentComponent))]
	public sealed class InstrumentComponent : SharedInstrumentComponent
	{
		// Token: 0x14000062 RID: 98
		// (add) Token: 0x06001142 RID: 4418 RVA: 0x000668E8 File Offset: 0x00064AE8
		// (remove) Token: 0x06001143 RID: 4419 RVA: 0x00066920 File Offset: 0x00064B20
		public event Action OnMidiPlaybackEnded;

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06001144 RID: 4420 RVA: 0x00066955 File Offset: 0x00064B55
		// (set) Token: 0x06001145 RID: 4421 RVA: 0x0006695D File Offset: 0x00064B5D
		[ViewVariables]
		public bool LoopMidi { get; set; }

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06001146 RID: 4422 RVA: 0x00066966 File Offset: 0x00064B66
		// (set) Token: 0x06001147 RID: 4423 RVA: 0x0006696E File Offset: 0x00064B6E
		[DataField("handheld", false, 1, false, false, null)]
		public bool Handheld { get; set; }

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06001148 RID: 4424 RVA: 0x00066977 File Offset: 0x00064B77
		[ViewVariables]
		public bool IsMidiOpen
		{
			get
			{
				IMidiRenderer renderer = this.Renderer;
				return renderer != null && renderer.Status == 2;
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06001149 RID: 4425 RVA: 0x0006698D File Offset: 0x00064B8D
		[ViewVariables]
		public bool IsInputOpen
		{
			get
			{
				IMidiRenderer renderer = this.Renderer;
				return renderer != null && renderer.Status == 1;
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x0600114A RID: 4426 RVA: 0x000669A3 File Offset: 0x00064BA3
		[ViewVariables]
		public bool IsRendererAlive
		{
			get
			{
				return this.Renderer != null;
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x0600114B RID: 4427 RVA: 0x000669AE File Offset: 0x00064BAE
		[ViewVariables]
		public int PlayerTotalTick
		{
			get
			{
				IMidiRenderer renderer = this.Renderer;
				if (renderer == null)
				{
					return 0;
				}
				return renderer.PlayerTotalTick;
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x0600114C RID: 4428 RVA: 0x000669C1 File Offset: 0x00064BC1
		[ViewVariables]
		public int PlayerTick
		{
			get
			{
				IMidiRenderer renderer = this.Renderer;
				if (renderer == null)
				{
					return 0;
				}
				return renderer.PlayerTick;
			}
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x000669D4 File Offset: 0x00064BD4
		public void PlaybackEndedInvoke()
		{
			Action onMidiPlaybackEnded = this.OnMidiPlaybackEnded;
			if (onMidiPlaybackEnded == null)
			{
				return;
			}
			onMidiPlaybackEnded();
		}

		// Token: 0x04000878 RID: 2168
		[ViewVariables]
		public IMidiRenderer Renderer;

		// Token: 0x04000879 RID: 2169
		[ViewVariables]
		public uint SequenceDelay;

		// Token: 0x0400087A RID: 2170
		[ViewVariables]
		public uint SequenceStartTick;

		// Token: 0x0400087B RID: 2171
		[ViewVariables]
		public TimeSpan LastMeasured = TimeSpan.MinValue;

		// Token: 0x0400087C RID: 2172
		[ViewVariables]
		public int SentWithinASec;

		// Token: 0x0400087D RID: 2173
		[Nullable(1)]
		[ViewVariables]
		public readonly List<RobustMidiEvent> MidiEventBuffer = new List<RobustMidiEvent>();
	}
}
