using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.White.TTS
{
	// Token: 0x0200002E RID: 46
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class PlayTTSEvent : EntityEventArgs
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002B60 File Offset: 0x00000D60
		public EntityUid Uid { get; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002B68 File Offset: 0x00000D68
		public byte[] Data { get; }

		// Token: 0x0600003F RID: 63 RVA: 0x00002B70 File Offset: 0x00000D70
		public PlayTTSEvent(EntityUid uid, byte[] data)
		{
			this.Uid = uid;
			this.Data = data;
		}
	}
}
