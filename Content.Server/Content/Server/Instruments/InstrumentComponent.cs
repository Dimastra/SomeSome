using System;
using System.Runtime.CompilerServices;
using Content.Server.UserInterface;
using Content.Shared.Instruments;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Server.Instruments
{
	// Token: 0x02000449 RID: 1097
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedInstrumentComponent))]
	public sealed class InstrumentComponent : SharedInstrumentComponent
	{
		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06001618 RID: 5656 RVA: 0x00074DCB File Offset: 0x00072FCB
		public IPlayerSession InstrumentPlayer
		{
			get
			{
				ActivatableUIComponent componentOrNull = EntityManagerExt.GetComponentOrNull<ActivatableUIComponent>(this._entMan, base.Owner);
				IPlayerSession result;
				if ((result = ((componentOrNull != null) ? componentOrNull.CurrentSingleUser : null)) == null)
				{
					ActorComponent componentOrNull2 = EntityManagerExt.GetComponentOrNull<ActorComponent>(this._entMan, base.Owner);
					if (componentOrNull2 == null)
					{
						return null;
					}
					result = componentOrNull2.PlayerSession;
				}
				return result;
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06001619 RID: 5657 RVA: 0x00074E0A File Offset: 0x0007300A
		[ViewVariables]
		public BoundUserInterface UserInterface
		{
			get
			{
				return base.Owner.GetUIOrNull(InstrumentUiKey.Key);
			}
		}

		// Token: 0x04000DDD RID: 3549
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000DDE RID: 3550
		[ViewVariables]
		public float Timer;

		// Token: 0x04000DDF RID: 3551
		[ViewVariables]
		public int BatchesDropped;

		// Token: 0x04000DE0 RID: 3552
		[ViewVariables]
		public int LaggedBatches;

		// Token: 0x04000DE1 RID: 3553
		[ViewVariables]
		public int MidiEventCount;

		// Token: 0x04000DE2 RID: 3554
		[ViewVariables]
		public uint LastSequencerTick;
	}
}
