using System;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Clothing.Components;
using Content.Server.Disease.Components;
using Content.Server.IdentityManagement;
using Content.Server.Nutrition.EntitySystems;
using Content.Server.Popups;
using Content.Server.VoiceMask;
using Content.Shared.Actions;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Clothing
{
	// Token: 0x02000638 RID: 1592
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MaskSystem : EntitySystem
	{
		// Token: 0x060021DC RID: 8668 RVA: 0x000B08A4 File Offset: 0x000AEAA4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MaskComponent, ToggleMaskEvent>(new ComponentEventHandler<MaskComponent, ToggleMaskEvent>(this.OnToggleMask), null, null);
			base.SubscribeLocalEvent<MaskComponent, GetItemActionsEvent>(new ComponentEventHandler<MaskComponent, GetItemActionsEvent>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<MaskComponent, GotUnequippedEvent>(new ComponentEventHandler<MaskComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x000B08F3 File Offset: 0x000AEAF3
		private void OnGetActions(EntityUid uid, MaskComponent component, GetItemActionsEvent args)
		{
			if (component.ToggleAction != null && !args.InHands)
			{
				args.Actions.Add(component.ToggleAction);
			}
		}

		// Token: 0x060021DE RID: 8670 RVA: 0x000B0918 File Offset: 0x000AEB18
		private void OnToggleMask(EntityUid uid, MaskComponent mask, ToggleMaskEvent args)
		{
			if (mask.ToggleAction == null)
			{
				return;
			}
			EntityUid? existing;
			if (!this._inventorySystem.TryGetSlotEntity(args.Performer, "mask", out existing, null, null) || !mask.Owner.Equals(existing))
			{
				return;
			}
			mask.IsToggled = !mask.IsToggled;
			this._actionSystem.SetToggled(mask.ToggleAction, mask.IsToggled);
			this._identity.QueueIdentityUpdate(args.Performer);
			if (mask.IsToggled)
			{
				this._popupSystem.PopupEntity(Loc.GetString("action-mask-pull-down-popup-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mask", mask.Owner)
				}), args.Performer, args.Performer, PopupType.Small);
			}
			else
			{
				this._popupSystem.PopupEntity(Loc.GetString("action-mask-pull-up-popup-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mask", mask.Owner)
				}), args.Performer, args.Performer, PopupType.Small);
			}
			this.ToggleMaskComponents(uid, mask, args.Performer, false);
		}

		// Token: 0x060021DF RID: 8671 RVA: 0x000B0A3F File Offset: 0x000AEC3F
		private void OnGotUnequipped(EntityUid uid, MaskComponent mask, GotUnequippedEvent args)
		{
			if (mask.ToggleAction == null)
			{
				return;
			}
			mask.IsToggled = false;
			this._actionSystem.SetToggled(mask.ToggleAction, mask.IsToggled);
			this.ToggleMaskComponents(uid, mask, args.Equipee, true);
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x000B0A78 File Offset: 0x000AEC78
		private void ToggleMaskComponents(EntityUid uid, MaskComponent mask, EntityUid wearer, bool isEquip = false)
		{
			ClothingComponent clothing;
			if (base.TryComp<ClothingComponent>(mask.Owner, ref clothing))
			{
				this._clothing.SetEquippedPrefix(uid, mask.IsToggled ? "toggled" : null, clothing);
			}
			IngestionBlockerComponent blocker;
			if (base.TryComp<IngestionBlockerComponent>(uid, ref blocker))
			{
				blocker.Enabled = !mask.IsToggled;
			}
			DiseaseProtectionComponent diseaseProtection;
			if (base.TryComp<DiseaseProtectionComponent>(uid, ref diseaseProtection))
			{
				diseaseProtection.IsActive = !mask.IsToggled;
			}
			IdentityBlockerComponent identity;
			if (base.TryComp<IdentityBlockerComponent>(uid, ref identity))
			{
				identity.Enabled = !mask.IsToggled;
			}
			VoiceMaskComponent voiceMask;
			if (base.TryComp<VoiceMaskComponent>(wearer, ref voiceMask))
			{
				voiceMask.Enabled = !mask.IsToggled;
			}
			BreathToolComponent breathTool;
			if (isEquip || !base.TryComp<BreathToolComponent>(uid, ref breathTool))
			{
				return;
			}
			if (mask.IsToggled)
			{
				this._atmos.DisconnectInternals(breathTool);
				return;
			}
			breathTool.IsFunctional = true;
			InternalsComponent internals;
			if (base.TryComp<InternalsComponent>(wearer, ref internals))
			{
				breathTool.ConnectedInternalsEntity = new EntityUid?(wearer);
				this._internals.ConnectBreathTool(internals, uid);
			}
		}

		// Token: 0x040014BF RID: 5311
		[Dependency]
		private readonly ActionsSystem _actionSystem;

		// Token: 0x040014C0 RID: 5312
		[Dependency]
		private readonly AtmosphereSystem _atmos;

		// Token: 0x040014C1 RID: 5313
		[Dependency]
		private readonly InternalsSystem _internals;

		// Token: 0x040014C2 RID: 5314
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x040014C3 RID: 5315
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040014C4 RID: 5316
		[Dependency]
		private readonly IdentitySystem _identity;

		// Token: 0x040014C5 RID: 5317
		[Dependency]
		private readonly ClothingSystem _clothing;
	}
}
