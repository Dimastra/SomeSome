using System;
using System.Runtime.CompilerServices;
using Content.Shared.Cooldown;
using Robust.Shared.GameObjects;

namespace Content.Server.Cooldown
{
	// Token: 0x020005E5 RID: 1509
	public sealed class ItemCooldownSystem : EntitySystem
	{
		// Token: 0x06002038 RID: 8248 RVA: 0x000A8018 File Offset: 0x000A6218
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ItemCooldownComponent, RefreshItemCooldownEvent>(new ComponentEventHandler<ItemCooldownComponent, RefreshItemCooldownEvent>(this.OnItemCooldownRefreshed), null, null);
		}

		// Token: 0x06002039 RID: 8249 RVA: 0x000A8034 File Offset: 0x000A6234
		[NullableContext(1)]
		public void OnItemCooldownRefreshed(EntityUid uid, ItemCooldownComponent comp, RefreshItemCooldownEvent args)
		{
			comp.CooldownStart = new TimeSpan?(args.LastAttackTime);
			comp.CooldownEnd = new TimeSpan?(args.CooldownEnd);
		}
	}
}
