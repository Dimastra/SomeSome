using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Throwing
{
	// Token: 0x020000CD RID: 205
	public sealed class BeforeThrowEvent : HandledEntityEventArgs
	{
		// Token: 0x06000233 RID: 563 RVA: 0x0000B093 File Offset: 0x00009293
		public BeforeThrowEvent(EntityUid itemUid, Vector2 direction, float throwStrength, EntityUid playerUid)
		{
			this.ItemUid = itemUid;
			this.Direction = direction;
			this.ThrowStrength = throwStrength;
			this.PlayerUid = playerUid;
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000234 RID: 564 RVA: 0x0000B0B8 File Offset: 0x000092B8
		// (set) Token: 0x06000235 RID: 565 RVA: 0x0000B0C0 File Offset: 0x000092C0
		public EntityUid ItemUid { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000236 RID: 566 RVA: 0x0000B0C9 File Offset: 0x000092C9
		public Vector2 Direction { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000237 RID: 567 RVA: 0x0000B0D1 File Offset: 0x000092D1
		// (set) Token: 0x06000238 RID: 568 RVA: 0x0000B0D9 File Offset: 0x000092D9
		public float ThrowStrength { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000239 RID: 569 RVA: 0x0000B0E2 File Offset: 0x000092E2
		public EntityUid PlayerUid { get; }
	}
}
