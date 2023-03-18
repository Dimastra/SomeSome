using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A2 RID: 1442
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EquipmentVisualsUpdatedEvent : EntityEventArgs
	{
		// Token: 0x06001197 RID: 4503 RVA: 0x000395D1 File Offset: 0x000377D1
		public EquipmentVisualsUpdatedEvent(EntityUid equipee, string slot, HashSet<string> revealedLayers)
		{
			this.Equipee = equipee;
			this.Slot = slot;
			this.RevealedLayers = revealedLayers;
		}

		// Token: 0x0400103B RID: 4155
		public readonly EntityUid Equipee;

		// Token: 0x0400103C RID: 4156
		public readonly string Slot;

		// Token: 0x0400103D RID: 4157
		public HashSet<string> RevealedLayers;
	}
}
