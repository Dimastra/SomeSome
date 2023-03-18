using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.StatusEffect
{
	// Token: 0x0200015B RID: 347
	[NullableContext(1)]
	[Nullable(0)]
	public readonly struct StatusEffectAddedEvent
	{
		// Token: 0x06000437 RID: 1079 RVA: 0x00011168 File Offset: 0x0000F368
		public StatusEffectAddedEvent(EntityUid uid, string key)
		{
			this.Uid = uid;
			this.Key = key;
		}

		// Token: 0x04000400 RID: 1024
		public readonly EntityUid Uid;

		// Token: 0x04000401 RID: 1025
		public readonly string Key;
	}
}
