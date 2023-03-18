using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Instruments
{
	// Token: 0x020003E8 RID: 1000
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedInstrumentSystem : EntitySystem
	{
		// Token: 0x06000BC0 RID: 3008 RVA: 0x0002657F File Offset: 0x0002477F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedInstrumentComponent, ComponentGetState>(new ComponentEventRefHandler<SharedInstrumentComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<SharedInstrumentComponent, ComponentHandleState>(new ComponentEventRefHandler<SharedInstrumentComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x000265AF File Offset: 0x000247AF
		[NullableContext(2)]
		public virtual void SetupRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent instrument = null)
		{
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x000265B1 File Offset: 0x000247B1
		[NullableContext(2)]
		public virtual void EndRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent instrument = null)
		{
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x000265B3 File Offset: 0x000247B3
		public void SetInstrumentProgram(SharedInstrumentComponent component, byte program, byte bank)
		{
			component.InstrumentBank = bank;
			component.InstrumentProgram = program;
			component.DirtyRenderer = true;
			base.Dirty(component, null);
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x000265D2 File Offset: 0x000247D2
		private void OnGetState(EntityUid uid, SharedInstrumentComponent instrument, ref ComponentGetState args)
		{
			args.State = new InstrumentState(instrument.Playing, instrument.InstrumentProgram, instrument.InstrumentBank, instrument.AllowPercussion, instrument.AllowProgramChange, instrument.RespectMidiLimits);
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x00026604 File Offset: 0x00024804
		private void OnHandleState(EntityUid uid, SharedInstrumentComponent instrument, ref ComponentHandleState args)
		{
			InstrumentState state = args.Current as InstrumentState;
			if (state == null)
			{
				return;
			}
			if (state.Playing)
			{
				this.SetupRenderer(uid, true, instrument);
			}
			else
			{
				this.EndRenderer(uid, true, instrument);
			}
			instrument.Playing = state.Playing;
			instrument.AllowPercussion = state.AllowPercussion;
			instrument.AllowProgramChange = state.AllowProgramChange;
			instrument.InstrumentBank = state.InstrumentBank;
			instrument.InstrumentProgram = state.InstrumentProgram;
			instrument.DirtyRenderer = true;
		}
	}
}
