using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror
{
	// Token: 0x02000346 RID: 838
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MagicMirrorUiData : BoundUserInterfaceMessage
	{
		// Token: 0x060009D8 RID: 2520 RVA: 0x0002060F File Offset: 0x0001E80F
		public MagicMirrorUiData(string species, List<Marking> hair, int hairSlotTotal, List<Marking> facialHair, int facialHairSlotTotal)
		{
			this.Species = species;
			this.Hair = hair;
			this.HairSlotTotal = hairSlotTotal;
			this.FacialHair = facialHair;
			this.FacialHairSlotTotal = facialHairSlotTotal;
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060009D9 RID: 2521 RVA: 0x0002063C File Offset: 0x0001E83C
		public string Species { get; }

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060009DA RID: 2522 RVA: 0x00020644 File Offset: 0x0001E844
		public List<Marking> Hair { get; }

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060009DB RID: 2523 RVA: 0x0002064C File Offset: 0x0001E84C
		public int HairSlotTotal { get; }

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060009DC RID: 2524 RVA: 0x00020654 File Offset: 0x0001E854
		public List<Marking> FacialHair { get; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060009DD RID: 2525 RVA: 0x0002065C File Offset: 0x0001E85C
		public int FacialHairSlotTotal { get; }
	}
}
