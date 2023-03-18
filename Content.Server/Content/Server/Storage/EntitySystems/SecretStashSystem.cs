using System;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.Storage.Components;
using Content.Shared.Destructible;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x02000163 RID: 355
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SecretStashSystem : EntitySystem
	{
		// Token: 0x060006DD RID: 1757 RVA: 0x00021F30 File Offset: 0x00020130
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SecretStashComponent, ComponentInit>(new ComponentEventHandler<SecretStashComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SecretStashComponent, DestructionEventArgs>(new ComponentEventHandler<SecretStashComponent, DestructionEventArgs>(this.OnDestroyed), null, null);
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x00021F60 File Offset: 0x00020160
		private void OnInit(EntityUid uid, SecretStashComponent component, ComponentInit args)
		{
			bool flag;
			component.ItemContainer = this._containerSystem.EnsureContainer<ContainerSlot>(uid, "stash", ref flag, null);
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x00021F88 File Offset: 0x00020188
		private void OnDestroyed(EntityUid uid, SecretStashComponent component, DestructionEventArgs args)
		{
			this._containerSystem.EmptyContainer(component.ItemContainer, false, null, true, null);
		}

		// Token: 0x060006E0 RID: 1760 RVA: 0x00021FB4 File Offset: 0x000201B4
		[NullableContext(2)]
		public bool HasItemInside(EntityUid uid, SecretStashComponent component = null)
		{
			return base.Resolve<SecretStashComponent>(uid, ref component, true) && component.ItemContainer.ContainedEntity != null;
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x00021FE4 File Offset: 0x000201E4
		[NullableContext(2)]
		public bool TryHideItem(EntityUid uid, EntityUid userUid, EntityUid itemToHideUid, SecretStashComponent component = null, ItemComponent item = null, MetaDataComponent itemMeta = null, SharedHandsComponent hands = null)
		{
			if (!base.Resolve<SecretStashComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!base.Resolve<ItemComponent, MetaDataComponent>(itemToHideUid, ref item, ref itemMeta, true))
			{
				return false;
			}
			if (!base.Resolve<SharedHandsComponent>(userUid, ref hands, true))
			{
				return false;
			}
			ContainerSlot container = component.ItemContainer;
			if (container.ContainedEntity != null)
			{
				string msg = Loc.GetString("comp-secret-stash-action-hide-container-not-empty");
				this._popupSystem.PopupEntity(msg, uid, userUid, PopupType.Small);
				return false;
			}
			string itemName = itemMeta.EntityName;
			if (item.Size > component.MaxItemSize)
			{
				string msg2 = Loc.GetString("comp-secret-stash-action-hide-item-too-big", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("item", itemName),
					new ValueTuple<string, object>("stash", this.GetSecretPartName(uid, component))
				});
				this._popupSystem.PopupEntity(msg2, uid, userUid, PopupType.Small);
				return false;
			}
			if (!this._handsSystem.TryDropIntoContainer(userUid, itemToHideUid, container, true, null))
			{
				return false;
			}
			string successMsg = Loc.GetString("comp-secret-stash-action-hide-success", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("item", itemName),
				new ValueTuple<string, object>("this", this.GetSecretPartName(uid, component))
			});
			this._popupSystem.PopupEntity(successMsg, uid, userUid, PopupType.Small);
			return true;
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x0002211C File Offset: 0x0002031C
		[NullableContext(2)]
		public bool TryGetItem(EntityUid uid, EntityUid userUid, SecretStashComponent component = null, SharedHandsComponent hands = null)
		{
			if (!base.Resolve<SecretStashComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!base.Resolve<SharedHandsComponent>(userUid, ref hands, true))
			{
				return false;
			}
			ContainerSlot container = component.ItemContainer;
			if (container.ContainedEntity == null)
			{
				return false;
			}
			this._handsSystem.PickupOrDrop(new EntityUid?(userUid), container.ContainedEntity.Value, true, false, hands, null);
			string successMsg = Loc.GetString("comp-secret-stash-action-get-item-found-something", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("stash", this.GetSecretPartName(uid, component))
			});
			this._popupSystem.PopupEntity(successMsg, uid, userUid, PopupType.Small);
			return true;
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x000221BC File Offset: 0x000203BC
		private string GetSecretPartName(EntityUid uid, SecretStashComponent stash)
		{
			if (stash.SecretPartName != "")
			{
				return Loc.GetString(stash.SecretPartName);
			}
			MetaDataComponent meta = this.EntityManager.GetComponent<MetaDataComponent>(uid);
			return Loc.GetString("comp-secret-stash-secret-part-name", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", meta.EntityName)
			});
		}

		// Token: 0x040003F2 RID: 1010
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040003F3 RID: 1011
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x040003F4 RID: 1012
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;
	}
}
