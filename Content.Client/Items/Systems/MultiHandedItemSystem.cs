using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands;
using Content.Shared.Item;
using Robust.Shared.GameObjects;

namespace Content.Client.Items.Systems
{
	// Token: 0x0200029E RID: 670
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MultiHandedItemSystem : SharedMultiHandedItemSystem
	{
		// Token: 0x060010EE RID: 4334 RVA: 0x0001B008 File Offset: 0x00019208
		protected override void OnEquipped(EntityUid uid, MultiHandedItemComponent component, GotEquippedHandEvent args)
		{
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x0001B008 File Offset: 0x00019208
		protected override void OnUnequipped(EntityUid uid, MultiHandedItemComponent component, GotUnequippedHandEvent args)
		{
		}
	}
}
