using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Interaction
{
	// Token: 0x020003CF RID: 975
	[NetSerializable]
	[Serializable]
	public sealed class InteractInventorySlotEvent : EntityEventArgs
	{
		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000B5A RID: 2906 RVA: 0x00025E7F File Offset: 0x0002407F
		public EntityUid ItemUid { get; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000B5B RID: 2907 RVA: 0x00025E87 File Offset: 0x00024087
		public bool AltInteract { get; }

		// Token: 0x06000B5C RID: 2908 RVA: 0x00025E8F File Offset: 0x0002408F
		public InteractInventorySlotEvent(EntityUid itemUid, bool altInteract = false)
		{
			this.ItemUid = itemUid;
			this.AltInteract = altInteract;
		}
	}
}
