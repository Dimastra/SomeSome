using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CharacterInfo
{
	// Token: 0x0200060F RID: 1551
	[NetSerializable]
	[Serializable]
	public sealed class RequestCharacterInfoEvent : EntityEventArgs
	{
		// Token: 0x060012F2 RID: 4850 RVA: 0x0003E1A7 File Offset: 0x0003C3A7
		public RequestCharacterInfoEvent(EntityUid entityUid)
		{
			this.EntityUid = entityUid;
		}

		// Token: 0x040011BC RID: 4540
		public readonly EntityUid EntityUid;
	}
}
