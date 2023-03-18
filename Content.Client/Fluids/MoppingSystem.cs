using System;
using System.Runtime.CompilerServices;
using Content.Client.Fluids.UI;
using Content.Client.Items;
using Content.Shared.Fluids;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Fluids
{
	// Token: 0x0200030F RID: 783
	public sealed class MoppingSystem : SharedMoppingSystem
	{
		// Token: 0x060013C6 RID: 5062 RVA: 0x000745B6 File Offset: 0x000727B6
		public override void Initialize()
		{
			base.Initialize();
			base.Subs.ItemStatus(new Func<EntityUid, Control>(this.GetAbsorbent));
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x000745D5 File Offset: 0x000727D5
		[NullableContext(1)]
		private Control GetAbsorbent(EntityUid arg)
		{
			return new AbsorbentItemStatus(arg, this.EntityManager);
		}
	}
}
