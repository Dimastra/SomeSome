using System;
using System.Runtime.CompilerServices;
using Content.Shared.CombatMode;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared.Item
{
	// Token: 0x020003A8 RID: 936
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedItemSystem : EntitySystem
	{
		// Token: 0x06000AAE RID: 2734 RVA: 0x00022CC0 File Offset: 0x00020EC0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ItemComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ItemComponent, GetVerbsEvent<InteractionVerb>>(this.AddPickupVerb), null, null);
			base.SubscribeLocalEvent<SharedSpriteComponent, GotEquippedEvent>(new ComponentEventHandler<SharedSpriteComponent, GotEquippedEvent>(this.OnEquipped), null, null);
			base.SubscribeLocalEvent<SharedSpriteComponent, GotUnequippedEvent>(new ComponentEventHandler<SharedSpriteComponent, GotUnequippedEvent>(this.OnUnequipped), null, null);
			base.SubscribeLocalEvent<ItemComponent, InteractHandEvent>(new ComponentEventHandler<ItemComponent, InteractHandEvent>(this.OnHandInteract), null, null);
			base.SubscribeLocalEvent<ItemComponent, ComponentGetState>(new ComponentEventRefHandler<ItemComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<ItemComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x00022D4B File Offset: 0x00020F4B
		[NullableContext(2)]
		public void SetSize(EntityUid uid, int size, ItemComponent component = null)
		{
			if (!base.Resolve<ItemComponent>(uid, ref component, true))
			{
				return;
			}
			component.Size = size;
			base.Dirty(component, null);
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x00022D69 File Offset: 0x00020F69
		[NullableContext(2)]
		public void SetHeldPrefix(EntityUid uid, string heldPrefix, ItemComponent component = null)
		{
			if (!base.Resolve<ItemComponent>(uid, ref component, false))
			{
				return;
			}
			if (component.HeldPrefix == heldPrefix)
			{
				return;
			}
			component.HeldPrefix = heldPrefix;
			base.Dirty(component, null);
			this.VisualsChanged(uid);
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x00022DA0 File Offset: 0x00020FA0
		public void CopyVisuals(EntityUid uid, ItemComponent otherItem, [Nullable(2)] ItemComponent item = null)
		{
			if (!base.Resolve<ItemComponent>(uid, ref item, true))
			{
				return;
			}
			item.RsiPath = otherItem.RsiPath;
			item.InhandVisuals = otherItem.InhandVisuals;
			item.HeldPrefix = otherItem.HeldPrefix;
			base.Dirty(item, null);
			this.VisualsChanged(uid);
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x00022DF0 File Offset: 0x00020FF0
		private void OnHandInteract(EntityUid uid, ItemComponent component, InteractHandEvent args)
		{
			if (args.Handled || this._combatMode.IsInCombatMode(new EntityUid?(args.User), null))
			{
				return;
			}
			args.Handled = this._handsSystem.TryPickup(args.User, uid, null, true, false, null, null);
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x00022E3C File Offset: 0x0002103C
		private void OnHandleState(EntityUid uid, ItemComponent component, ref ComponentHandleState args)
		{
			ItemComponentState state = args.Current as ItemComponentState;
			if (state == null)
			{
				return;
			}
			component.Size = state.Size;
			this.SetHeldPrefix(uid, state.HeldPrefix, component);
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x00022E73 File Offset: 0x00021073
		private void OnGetState(EntityUid uid, ItemComponent component, ref ComponentGetState args)
		{
			args.State = new ItemComponentState(component.Size, component.HeldPrefix);
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00022E8C File Offset: 0x0002108C
		private void OnUnequipped(EntityUid uid, SharedSpriteComponent component, GotUnequippedEvent args)
		{
			component.Visible = true;
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x00022E95 File Offset: 0x00021095
		private void OnEquipped(EntityUid uid, SharedSpriteComponent component, GotEquippedEvent args)
		{
			component.Visible = false;
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x00022EA0 File Offset: 0x000210A0
		private void AddPickupVerb(EntityUid uid, ItemComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (args.Hands == null || args.Using != null || !args.CanAccess || !args.CanInteract || !this._handsSystem.CanPickupAnyHand(args.User, args.Target, true, args.Hands, component))
			{
				return;
			}
			InteractionVerb verb = new InteractionVerb();
			verb.Act = delegate()
			{
				this._handsSystem.TryPickupAnyHand(args.User, args.Target, false, false, args.Hands, component);
			};
			verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/pickup.svg.192dpi.png", "/"));
			IContainer userContainer;
			this.Container.TryGetContainingContainer(args.User, ref userContainer, null, null);
			IContainer container;
			if (this.Container.TryGetContainingContainer(args.Target, ref container, null, null) && container != userContainer)
			{
				verb.Text = Loc.GetString("pick-up-verb-get-data-text-inventory");
			}
			else
			{
				verb.Text = Loc.GetString("pick-up-verb-get-data-text");
			}
			args.Verbs.Add(verb);
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x00022FD8 File Offset: 0x000211D8
		public virtual void VisualsChanged(EntityUid owner)
		{
		}

		// Token: 0x04000AAD RID: 2733
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000AAE RID: 2734
		[Dependency]
		private readonly SharedCombatModeSystem _combatMode;

		// Token: 0x04000AAF RID: 2735
		[Dependency]
		protected readonly SharedContainerSystem Container;
	}
}
