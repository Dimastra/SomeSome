using System;
using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Explosion
{
	// Token: 0x020004A2 RID: 1186
	public sealed class GetExplosionResistanceEvent : EntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06000E5E RID: 3678 RVA: 0x0002E1DF File Offset: 0x0002C3DF
		SlotFlags IInventoryRelayEvent.TargetSlots
		{
			get
			{
				return ~SlotFlags.POCKET;
			}
		}

		// Token: 0x06000E5F RID: 3679 RVA: 0x0002E1E6 File Offset: 0x0002C3E6
		[NullableContext(1)]
		public GetExplosionResistanceEvent(string id)
		{
			this.ExplotionPrototype = id;
		}

		// Token: 0x04000D7C RID: 3452
		public float DamageCoefficient = 1f;

		// Token: 0x04000D7D RID: 3453
		[Nullable(1)]
		public readonly string ExplotionPrototype;
	}
}
