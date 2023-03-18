using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.StatusEffect
{
	// Token: 0x0200015C RID: 348
	[NullableContext(1)]
	[Nullable(0)]
	public readonly struct StatusEffectEndedEvent
	{
		// Token: 0x06000438 RID: 1080 RVA: 0x00011178 File Offset: 0x0000F378
		public StatusEffectEndedEvent(EntityUid uid, string key)
		{
			this.Uid = uid;
			this.Key = key;
		}

		// Token: 0x04000402 RID: 1026
		public readonly EntityUid Uid;

		// Token: 0x04000403 RID: 1027
		public readonly string Key;
	}
}
