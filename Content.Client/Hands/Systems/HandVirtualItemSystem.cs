using System;
using Content.Client.Hands.UI;
using Content.Client.Items;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Client.Hands.Systems
{
	// Token: 0x020002E0 RID: 736
	public sealed class HandVirtualItemSystem : SharedHandVirtualItemSystem
	{
		// Token: 0x060012AB RID: 4779 RVA: 0x0006F71D File Offset: 0x0006D91D
		public override void Initialize()
		{
			base.Initialize();
			base.Subs.ItemStatus((EntityUid _) => new HandVirtualItemStatus());
		}
	}
}
