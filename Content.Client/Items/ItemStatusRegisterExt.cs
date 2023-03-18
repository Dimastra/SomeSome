using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Items
{
	// Token: 0x0200029B RID: 667
	public static class ItemStatusRegisterExt
	{
		// Token: 0x060010E6 RID: 4326 RVA: 0x0006522C File Offset: 0x0006342C
		[NullableContext(1)]
		public static void ItemStatus<[Nullable(0)] TComp>(this EntitySystem.Subscriptions subs, Func<EntityUid, Control> createControl) where TComp : IComponent
		{
			subs.SubscribeLocalEvent<TComp, ItemStatusCollectMessage>(delegate(EntityUid uid, TComp _, [Nullable(1)] ItemStatusCollectMessage args)
			{
				args.Controls.Add(createControl(uid));
			}, null, null);
		}
	}
}
