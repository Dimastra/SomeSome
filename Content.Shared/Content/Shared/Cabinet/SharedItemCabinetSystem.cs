using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Cabinet
{
	// Token: 0x0200063F RID: 1599
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedItemCabinetSystem : EntitySystem
	{
		// Token: 0x0600133A RID: 4922 RVA: 0x0003FFB8 File Offset: 0x0003E1B8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ItemCabinetComponent, ComponentGetState>(new ComponentEventRefHandler<ItemCabinetComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemCabinetComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, ComponentInit>(new ComponentEventHandler<ItemCabinetComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, ComponentRemove>(new ComponentEventHandler<ItemCabinetComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, ComponentStartup>(new ComponentEventHandler<ItemCabinetComponent, ComponentStartup>(this.OnComponentStartup), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, ActivateInWorldEvent>(new ComponentEventHandler<ItemCabinetComponent, ActivateInWorldEvent>(this.OnActivateInWorld), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<ItemCabinetComponent, GetVerbsEvent<AlternativeVerb>>(this.AddToggleOpenVerb), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ItemCabinetComponent, EntInsertedIntoContainerMessage>(this.OnContainerModified), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ItemCabinetComponent, EntRemovedFromContainerMessage>(this.OnContainerModified), null, null);
			base.SubscribeLocalEvent<ItemCabinetComponent, LockToggleAttemptEvent>(new ComponentEventRefHandler<ItemCabinetComponent, LockToggleAttemptEvent>(this.OnLockToggleAttempt), null, null);
		}

		// Token: 0x0600133B RID: 4923 RVA: 0x0004008D File Offset: 0x0003E28D
		private void OnGetState(EntityUid uid, ItemCabinetComponent component, ref ComponentGetState args)
		{
			args.State = new ItemCabinetComponentState(component.DoorSound, component.Opened, component.OpenState, component.ClosedState);
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x000400B4 File Offset: 0x0003E2B4
		private void OnHandleState(EntityUid uid, ItemCabinetComponent component, ref ComponentHandleState args)
		{
			ItemCabinetComponentState state = args.Current as ItemCabinetComponentState;
			if (state == null)
			{
				return;
			}
			component.DoorSound = state.DoorSound;
			component.Opened = state.Opened;
			component.OpenState = state.OpenState;
			component.ClosedState = state.ClosedState;
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x00040101 File Offset: 0x0003E301
		private void OnComponentInit(EntityUid uid, ItemCabinetComponent cabinet, ComponentInit args)
		{
			this._itemSlots.AddItemSlot(uid, "ItemCabinet", cabinet.CabinetSlot, null);
		}

		// Token: 0x0600133E RID: 4926 RVA: 0x0004011B File Offset: 0x0003E31B
		private void OnComponentRemove(EntityUid uid, ItemCabinetComponent cabinet, ComponentRemove args)
		{
			this._itemSlots.RemoveItemSlot(uid, cabinet.CabinetSlot, null);
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x00040130 File Offset: 0x0003E330
		private void OnComponentStartup(EntityUid uid, ItemCabinetComponent cabinet, ComponentStartup args)
		{
			this.UpdateAppearance(uid, cabinet);
			this._itemSlots.SetLock(uid, cabinet.CabinetSlot, !cabinet.Opened, null);
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x00040156 File Offset: 0x0003E356
		[NullableContext(2)]
		protected virtual void UpdateAppearance(EntityUid uid, ItemCabinetComponent cabinet = null)
		{
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x00040158 File Offset: 0x0003E358
		private void OnContainerModified(EntityUid uid, ItemCabinetComponent cabinet, ContainerModifiedMessage args)
		{
			if (!cabinet.Initialized)
			{
				return;
			}
			if (args.Container.ID == cabinet.CabinetSlot.ID)
			{
				this.UpdateAppearance(uid, cabinet);
			}
		}

		// Token: 0x06001342 RID: 4930 RVA: 0x00040188 File Offset: 0x0003E388
		private void OnLockToggleAttempt(EntityUid uid, ItemCabinetComponent cabinet, ref LockToggleAttemptEvent args)
		{
			if (cabinet.Opened)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x0004019C File Offset: 0x0003E39C
		private void AddToggleOpenVerb(EntityUid uid, ItemCabinetComponent cabinet, GetVerbsEvent<AlternativeVerb> args)
		{
			if (args.Hands == null || !args.CanAccess || !args.CanInteract)
			{
				return;
			}
			LockComponent lockComponent;
			if (base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked)
			{
				return;
			}
			AlternativeVerb toggleVerb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.ToggleItemCabinet(uid, new EntityUid?(args.User), cabinet);
				}
			};
			if (cabinet.Opened)
			{
				toggleVerb.Text = Loc.GetString("verb-common-close");
				toggleVerb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/close.svg.192dpi.png", "/"));
			}
			else
			{
				toggleVerb.Text = Loc.GetString("verb-common-open");
				toggleVerb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/open.svg.192dpi.png", "/"));
			}
			args.Verbs.Add(toggleVerb);
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x00040299 File Offset: 0x0003E499
		private void OnActivateInWorld(EntityUid uid, ItemCabinetComponent comp, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			this.ToggleItemCabinet(uid, new EntityUid?(args.User), comp);
		}

		// Token: 0x06001345 RID: 4933 RVA: 0x000402C0 File Offset: 0x0003E4C0
		[NullableContext(2)]
		public void ToggleItemCabinet(EntityUid uid, EntityUid? user = null, ItemCabinetComponent cabinet = null)
		{
			if (!base.Resolve<ItemCabinetComponent>(uid, ref cabinet, true))
			{
				return;
			}
			LockComponent lockComponent;
			if (base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked)
			{
				return;
			}
			cabinet.Opened = !cabinet.Opened;
			base.Dirty(cabinet, null);
			this._itemSlots.SetLock(uid, cabinet.CabinetSlot, !cabinet.Opened, null);
			if (this._timing.IsFirstTimePredicted)
			{
				this.UpdateAppearance(uid, cabinet);
				this._audio.PlayPredicted(cabinet.DoorSound, uid, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.15f))));
			}
		}

		// Token: 0x0400133F RID: 4927
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04001340 RID: 4928
		[Dependency]
		private readonly ItemSlotsSystem _itemSlots;

		// Token: 0x04001341 RID: 4929
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
