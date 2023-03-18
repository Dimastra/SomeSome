using System;
using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage
{
	// Token: 0x02000535 RID: 1333
	public sealed class DamageModifyEvent : EntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x17000334 RID: 820
		// (get) Token: 0x0600103C RID: 4156 RVA: 0x00034D5C File Offset: 0x00032F5C
		public SlotFlags TargetSlots { get; } = -4097;

		// Token: 0x0600103D RID: 4157 RVA: 0x00034D64 File Offset: 0x00032F64
		[NullableContext(1)]
		public DamageModifyEvent(DamageSpecifier damage)
		{
			this.Damage = damage;
		}

		// Token: 0x04000F49 RID: 3913
		[Nullable(1)]
		public DamageSpecifier Damage;
	}
}
