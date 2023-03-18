using System;
using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Radio.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radio.EntitySystems
{
	// Token: 0x02000222 RID: 546
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedHeadsetSystem : EntitySystem
	{
		// Token: 0x06000619 RID: 1561 RVA: 0x0001595C File Offset: 0x00013B5C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HeadsetComponent, InventoryRelayedEvent<GetDefaultRadioChannelEvent>>(new ComponentEventHandler<HeadsetComponent, InventoryRelayedEvent<GetDefaultRadioChannelEvent>>(this.OnGetDefault), null, null);
			base.SubscribeLocalEvent<HeadsetComponent, GotEquippedEvent>(new ComponentEventHandler<HeadsetComponent, GotEquippedEvent>(this.OnGotEquipped), null, null);
			base.SubscribeLocalEvent<HeadsetComponent, GotUnequippedEvent>(new ComponentEventHandler<HeadsetComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x000159B0 File Offset: 0x00013BB0
		private void OnGetDefault(EntityUid uid, HeadsetComponent component, InventoryRelayedEvent<GetDefaultRadioChannelEvent> args)
		{
			if (!component.Enabled || !component.IsEquipped)
			{
				return;
			}
			EncryptionKeyHolderComponent keyHolder;
			if (base.TryComp<EncryptionKeyHolderComponent>(uid, ref keyHolder))
			{
				GetDefaultRadioChannelEvent args2 = args.Args;
				if (args2.Channel == null)
				{
					args2.Channel = keyHolder.DefaultChannel;
				}
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x000159F4 File Offset: 0x00013BF4
		protected virtual void OnGotEquipped(EntityUid uid, HeadsetComponent component, GotEquippedEvent args)
		{
			component.IsEquipped = args.SlotFlags.HasFlag(component.RequiredSlot);
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00015A17 File Offset: 0x00013C17
		protected virtual void OnGotUnequipped(EntityUid uid, HeadsetComponent component, GotUnequippedEvent args)
		{
			component.IsEquipped = false;
		}
	}
}
