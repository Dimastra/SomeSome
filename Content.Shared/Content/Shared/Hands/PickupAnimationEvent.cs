using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands
{
	// Token: 0x0200042D RID: 1069
	[NetSerializable]
	[Serializable]
	public sealed class PickupAnimationEvent : EntityEventArgs
	{
		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x0002A4A1 File Offset: 0x000286A1
		public EntityUid ItemUid { get; }

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000CD4 RID: 3284 RVA: 0x0002A4A9 File Offset: 0x000286A9
		public EntityCoordinates InitialPosition { get; }

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000CD5 RID: 3285 RVA: 0x0002A4B1 File Offset: 0x000286B1
		public Vector2 FinalPosition { get; }

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0002A4B9 File Offset: 0x000286B9
		public PickupAnimationEvent(EntityUid itemUid, EntityCoordinates initialPosition, Vector2 finalPosition)
		{
			this.ItemUid = itemUid;
			this.FinalPosition = finalPosition;
			this.InitialPosition = initialPosition;
		}
	}
}
