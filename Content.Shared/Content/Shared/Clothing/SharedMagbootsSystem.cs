using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Slippery;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A7 RID: 1447
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedMagbootsSystem : EntitySystem
	{
		// Token: 0x060011A2 RID: 4514 RVA: 0x00039A24 File Offset: 0x00037C24
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MagbootsComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<MagbootsComponent, GetVerbsEvent<ActivationVerb>>(this.AddToggleVerb), null, null);
			base.SubscribeLocalEvent<MagbootsComponent, InventoryRelayedEvent<SlipAttemptEvent>>(new ComponentEventHandler<MagbootsComponent, InventoryRelayedEvent<SlipAttemptEvent>>(this.OnSlipAttempt), null, null);
			base.SubscribeLocalEvent<MagbootsComponent, GetItemActionsEvent>(new ComponentEventHandler<MagbootsComponent, GetItemActionsEvent>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<MagbootsComponent, ToggleActionEvent>(new ComponentEventHandler<MagbootsComponent, ToggleActionEvent>(this.OnToggleAction), null, null);
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x00039A88 File Offset: 0x00037C88
		private void OnToggleAction(EntityUid uid, MagbootsComponent component, ToggleActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			component.On = !component.On;
			IContainer container;
			EntityUid? entityUid;
			if (this._sharedContainer.TryGetContainingContainer(uid, ref container, null, null) && this._inventory.TryGetSlotEntity(container.Owner, "shoes", out entityUid, null, null))
			{
				EntityUid? entityUid2 = entityUid;
				EntityUid owner = component.Owner;
				if (entityUid2 != null && (entityUid2 == null || entityUid2.GetValueOrDefault() == owner))
				{
					this.UpdateMagbootEffects(container.Owner, uid, true, component);
				}
			}
			ItemComponent item;
			if (base.TryComp<ItemComponent>(uid, ref item))
			{
				this._item.SetHeldPrefix(uid, component.On ? "on" : null, item);
				this._clothing.SetEquippedPrefix(uid, component.On ? "on" : null, null);
			}
			this._appearance.SetData(uid, ToggleVisuals.Toggled, component.Owner, null);
			this.OnChanged(component);
			base.Dirty(component, null);
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x00039B95 File Offset: 0x00037D95
		[NullableContext(2)]
		protected virtual void UpdateMagbootEffects(EntityUid parent, EntityUid uid, bool state, MagbootsComponent component)
		{
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x00039B97 File Offset: 0x00037D97
		protected void OnChanged(MagbootsComponent component)
		{
			this._sharedActions.SetToggled(component.ToggleAction, component.On);
			this._clothingSpeedModifier.SetClothingSpeedModifierEnabled(component.Owner, component.On, null);
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x00039BC8 File Offset: 0x00037DC8
		private void AddToggleVerb(EntityUid uid, MagbootsComponent component, GetVerbsEvent<ActivationVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			ActivationVerb verb = new ActivationVerb();
			verb.Text = Loc.GetString("toggle-magboots-verb-get-data-text");
			verb.Act = delegate()
			{
				component.On = !component.On;
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x00039C28 File Offset: 0x00037E28
		private void OnSlipAttempt(EntityUid uid, MagbootsComponent component, InventoryRelayedEvent<SlipAttemptEvent> args)
		{
			if (component.On)
			{
				args.Args.Cancel();
			}
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x00039C3D File Offset: 0x00037E3D
		private void OnGetActions(EntityUid uid, MagbootsComponent component, GetItemActionsEvent args)
		{
			args.Actions.Add(component.ToggleAction);
		}

		// Token: 0x04001049 RID: 4169
		[Dependency]
		private readonly ClothingSpeedModifierSystem _clothingSpeedModifier;

		// Token: 0x0400104A RID: 4170
		[Dependency]
		private readonly ClothingSystem _clothing;

		// Token: 0x0400104B RID: 4171
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x0400104C RID: 4172
		[Dependency]
		private readonly SharedActionsSystem _sharedActions;

		// Token: 0x0400104D RID: 4173
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400104E RID: 4174
		[Dependency]
		private readonly SharedContainerSystem _sharedContainer;

		// Token: 0x0400104F RID: 4175
		[Dependency]
		private readonly SharedItemSystem _item;
	}
}
