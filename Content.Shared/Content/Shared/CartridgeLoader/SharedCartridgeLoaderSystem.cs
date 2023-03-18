using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x0200061C RID: 1564
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedCartridgeLoaderSystem : EntitySystem
	{
		// Token: 0x060012FD RID: 4861 RVA: 0x0003F7C8 File Offset: 0x0003D9C8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CartridgeLoaderComponent, ComponentInit>(new ComponentEventHandler<CartridgeLoaderComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<CartridgeLoaderComponent, ComponentRemove>(new ComponentEventHandler<CartridgeLoaderComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<CartridgeLoaderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<CartridgeLoaderComponent, EntInsertedIntoContainerMessage>(this.OnItemInserted), null, null);
			base.SubscribeLocalEvent<CartridgeLoaderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<CartridgeLoaderComponent, EntRemovedFromContainerMessage>(this.OnItemRemoved), null, null);
			base.SubscribeLocalEvent<CartridgeComponent, ComponentGetState>(new ComponentEventRefHandler<CartridgeComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<CartridgeComponent, ComponentHandleState>(new ComponentEventRefHandler<CartridgeComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x0003F855 File Offset: 0x0003DA55
		private void OnComponentInit(EntityUid uid, CartridgeLoaderComponent loader, ComponentInit args)
		{
			this._itemSlotsSystem.AddItemSlot(uid, "Cartridge-Slot", loader.CartridgeSlot, null);
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0003F870 File Offset: 0x0003DA70
		private void OnComponentRemove(EntityUid uid, CartridgeLoaderComponent loader, ComponentRemove args)
		{
			this._itemSlotsSystem.RemoveItemSlot(uid, loader.CartridgeSlot, null);
			foreach (EntityUid program in loader.InstalledPrograms)
			{
				this.EntityManager.QueueDeleteEntity(program);
			}
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x0003F8DC File Offset: 0x0003DADC
		protected virtual void OnItemInserted(EntityUid uid, CartridgeLoaderComponent loader, EntInsertedIntoContainerMessage args)
		{
			this.UpdateAppearanceData(uid, loader);
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0003F8E6 File Offset: 0x0003DAE6
		protected virtual void OnItemRemoved(EntityUid uid, CartridgeLoaderComponent loader, EntRemovedFromContainerMessage args)
		{
			this.UpdateAppearanceData(uid, loader);
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0003F8F0 File Offset: 0x0003DAF0
		private void OnGetState(EntityUid uid, CartridgeComponent component, ref ComponentGetState args)
		{
			args.State = new CartridgeComponentState
			{
				InstallationStatus = component.InstallationStatus
			};
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x0003F918 File Offset: 0x0003DB18
		private void OnHandleState(EntityUid uid, CartridgeComponent component, ref ComponentHandleState args)
		{
			CartridgeComponentState state = args.Current as CartridgeComponentState;
			if (state == null)
			{
				return;
			}
			component.InstallationStatus = state.InstallationStatus;
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0003F941 File Offset: 0x0003DB41
		private void UpdateAppearanceData(EntityUid uid, CartridgeLoaderComponent loader)
		{
			this._appearanceSystem.SetData(uid, CartridgeLoaderVisuals.CartridgeInserted, loader.CartridgeSlot.HasItem, null);
		}

		// Token: 0x040012E1 RID: 4833
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x040012E2 RID: 4834
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;
	}
}
