using System;
using System.Runtime.CompilerServices;
using Content.Server.Hands.Systems;
using Content.Shared.Hands;
using Content.Shared.Item;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Item
{
	// Token: 0x0200043F RID: 1087
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MultiHandedItemSystem : SharedMultiHandedItemSystem
	{
		// Token: 0x060015FB RID: 5627 RVA: 0x00074388 File Offset: 0x00072588
		protected override void OnEquipped(EntityUid uid, MultiHandedItemComponent component, GotEquippedHandEvent args)
		{
			for (int i = 0; i < component.HandsNeeded - 1; i++)
			{
				this._virtualItem.TrySpawnVirtualItemInHand(uid, args.User);
			}
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x000743BB File Offset: 0x000725BB
		protected override void OnUnequipped(EntityUid uid, MultiHandedItemComponent component, GotUnequippedHandEvent args)
		{
			this._virtualItem.DeleteInHandsMatching(args.User, uid);
		}

		// Token: 0x04000DC7 RID: 3527
		[Dependency]
		private readonly HandVirtualItemSystem _virtualItem;
	}
}
