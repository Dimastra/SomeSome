using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Cuffs.Components;
using Content.Server.DoAfter;
using Content.Server.Ensnaring;
using Content.Server.Hands.Components;
using Content.Shared.CombatMode;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Strip;
using Content.Shared.Strip.Components;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Strip
{
	// Token: 0x02000150 RID: 336
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StrippableSystem : SharedStrippableSystem
	{
		// Token: 0x06000661 RID: 1633 RVA: 0x0001E9C4 File Offset: 0x0001CBC4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StrippableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<StrippableComponent, GetVerbsEvent<Verb>>(this.AddStripVerb), null, null);
			base.SubscribeLocalEvent<StrippableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<StrippableComponent, GetVerbsEvent<ExamineVerb>>(this.AddStripExamineVerb), null, null);
			base.SubscribeLocalEvent<StrippableComponent, ActivateInWorldEvent>(new ComponentEventHandler<StrippableComponent, ActivateInWorldEvent>(this.OnActivateInWorld), null, null);
			base.SubscribeLocalEvent<StrippableComponent, StrippingSlotButtonPressed>(new ComponentEventHandler<StrippableComponent, StrippingSlotButtonPressed>(this.OnStripButtonPressed), null, null);
			base.SubscribeLocalEvent<EnsnareableComponent, StrippingEnsnareButtonPressed>(new ComponentEventHandler<EnsnareableComponent, StrippingEnsnareButtonPressed>(this.OnStripEnsnareMessage), null, null);
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0001EA3C File Offset: 0x0001CC3C
		private void OnStripEnsnareMessage(EntityUid uid, EnsnareableComponent component, StrippingEnsnareButtonPressed args)
		{
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid user = attachedEntity.GetValueOrDefault();
				if (user.Valid)
				{
					foreach (EntityUid entity in component.Container.ContainedEntities)
					{
						EnsnaringComponent ensnaring;
						if (base.TryComp<EnsnaringComponent>(entity, ref ensnaring))
						{
							this._ensnaring.TryFree(uid, entity, ensnaring, new EntityUid?(user));
							break;
						}
					}
					return;
				}
			}
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0001EAD0 File Offset: 0x0001CCD0
		private void OnStripButtonPressed(EntityUid uid, StrippableComponent component, StrippingSlotButtonPressed args)
		{
			EntityUid? entityUid = args.Session.AttachedEntity;
			if (entityUid != null)
			{
				EntityUid user = entityUid.GetValueOrDefault();
				HandsComponent userHands;
				if (user.Valid && base.TryComp<HandsComponent>(user, ref userHands))
				{
					if (args.IsHand)
					{
						this.StripHand(uid, user, args.Slot, component, userHands);
						return;
					}
					InventoryComponent inventory;
					if (!base.TryComp<InventoryComponent>(component.Owner, ref inventory))
					{
						return;
					}
					bool hasEnt = this._inventorySystem.TryGetSlotEntity(component.Owner, args.Slot, out entityUid, inventory, null);
					entityUid = userHands.ActiveHandEntity;
					if (entityUid != null && !hasEnt)
					{
						this.PlaceActiveHandItemInInventory(user, args.Slot, component);
						return;
					}
					entityUid = userHands.ActiveHandEntity;
					if (entityUid == null && hasEnt)
					{
						this.TakeItemFromInventory(user, args.Slot, component);
					}
					return;
				}
			}
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x0001EBA0 File Offset: 0x0001CDA0
		private void StripHand(EntityUid target, EntityUid user, string handId, StrippableComponent component, HandsComponent userHands)
		{
			HandsComponent targetHands;
			Hand hand;
			if (!base.TryComp<HandsComponent>(target, ref targetHands) || !targetHands.Hands.TryGetValue(handId, out hand))
			{
				return;
			}
			HandVirtualItemComponent virt;
			CuffableComponent cuff;
			if (base.TryComp<HandVirtualItemComponent>(hand.HeldEntity, ref virt) && base.TryComp<CuffableComponent>(target, ref cuff) && cuff.Container.Contains(virt.BlockingEntity))
			{
				cuff.TryUncuff(user, new EntityUid?(virt.BlockingEntity));
				return;
			}
			if (hand.IsEmpty && userHands.ActiveHandEntity != null)
			{
				this.PlaceActiveHandItemInHands(user, handId, component);
				return;
			}
			if (!hand.IsEmpty && userHands.ActiveHandEntity == null)
			{
				this.TakeItemFromHands(user, handId, component);
			}
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x0001EC54 File Offset: 0x0001CE54
		public override void StartOpeningStripper(EntityUid user, StrippableComponent component, bool openInCombat = false)
		{
			base.StartOpeningStripper(user, component, openInCombat);
			SharedCombatModeComponent mode;
			if (base.TryComp<SharedCombatModeComponent>(user, ref mode) && mode.IsInCombatMode && !openInCombat)
			{
				return;
			}
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(user, ref actor))
			{
				if (this._userInterfaceSystem.SessionHasOpenUi(component.Owner, StrippingUiKey.Key, actor.PlayerSession, null))
				{
					return;
				}
				this._userInterfaceSystem.TryOpen(component.Owner, StrippingUiKey.Key, actor.PlayerSession, null);
			}
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x0001ECCC File Offset: 0x0001CECC
		private void AddStripVerb(EntityUid uid, StrippableComponent component, GetVerbsEvent<Verb> args)
		{
			if (args.Hands == null || !args.CanAccess || !args.CanInteract || args.Target == args.User)
			{
				return;
			}
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			Verb verb = new Verb
			{
				Text = Loc.GetString("strip-verb-get-data-text"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png", "/")),
				Act = delegate()
				{
					this.StartOpeningStripper(args.User, component, true);
				}
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0001EDA8 File Offset: 0x0001CFA8
		private void AddStripExamineVerb(EntityUid uid, StrippableComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (args.Hands == null || !args.CanAccess || !args.CanInteract || args.Target == args.User)
			{
				return;
			}
			if (!base.HasComp<ActorComponent>(args.User))
			{
				return;
			}
			ExamineVerb verb = new ExamineVerb
			{
				Text = Loc.GetString("strip-verb-get-data-text"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png", "/")),
				Act = delegate()
				{
					this.StartOpeningStripper(args.User, component, true);
				},
				Category = VerbCategory.Examine
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0001EE88 File Offset: 0x0001D088
		private void OnActivateInWorld(EntityUid uid, StrippableComponent component, ActivateInWorldEvent args)
		{
			if (args.Target == args.User)
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			this.StartOpeningStripper(args.User, component, false);
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0001EEC8 File Offset: 0x0001D0C8
		private void PlaceActiveHandItemInInventory(EntityUid user, string slot, StrippableComponent component)
		{
			StrippableSystem.<PlaceActiveHandItemInInventory>d__15 <PlaceActiveHandItemInInventory>d__;
			<PlaceActiveHandItemInInventory>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PlaceActiveHandItemInInventory>d__.<>4__this = this;
			<PlaceActiveHandItemInInventory>d__.user = user;
			<PlaceActiveHandItemInInventory>d__.slot = slot;
			<PlaceActiveHandItemInInventory>d__.component = component;
			<PlaceActiveHandItemInInventory>d__.<>1__state = -1;
			<PlaceActiveHandItemInInventory>d__.<>t__builder.Start<StrippableSystem.<PlaceActiveHandItemInInventory>d__15>(ref <PlaceActiveHandItemInInventory>d__);
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0001EF18 File Offset: 0x0001D118
		private void PlaceActiveHandItemInHands(EntityUid user, string handName, StrippableComponent component)
		{
			StrippableSystem.<PlaceActiveHandItemInHands>d__16 <PlaceActiveHandItemInHands>d__;
			<PlaceActiveHandItemInHands>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PlaceActiveHandItemInHands>d__.<>4__this = this;
			<PlaceActiveHandItemInHands>d__.user = user;
			<PlaceActiveHandItemInHands>d__.handName = handName;
			<PlaceActiveHandItemInHands>d__.component = component;
			<PlaceActiveHandItemInHands>d__.<>1__state = -1;
			<PlaceActiveHandItemInHands>d__.<>t__builder.Start<StrippableSystem.<PlaceActiveHandItemInHands>d__16>(ref <PlaceActiveHandItemInHands>d__);
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0001EF68 File Offset: 0x0001D168
		private void TakeItemFromInventory(EntityUid user, string slot, StrippableComponent component)
		{
			StrippableSystem.<TakeItemFromInventory>d__17 <TakeItemFromInventory>d__;
			<TakeItemFromInventory>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<TakeItemFromInventory>d__.<>4__this = this;
			<TakeItemFromInventory>d__.user = user;
			<TakeItemFromInventory>d__.slot = slot;
			<TakeItemFromInventory>d__.component = component;
			<TakeItemFromInventory>d__.<>1__state = -1;
			<TakeItemFromInventory>d__.<>t__builder.Start<StrippableSystem.<TakeItemFromInventory>d__17>(ref <TakeItemFromInventory>d__);
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x0001EFB8 File Offset: 0x0001D1B8
		private void TakeItemFromHands(EntityUid user, string handName, StrippableComponent component)
		{
			StrippableSystem.<TakeItemFromHands>d__18 <TakeItemFromHands>d__;
			<TakeItemFromHands>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<TakeItemFromHands>d__.<>4__this = this;
			<TakeItemFromHands>d__.user = user;
			<TakeItemFromHands>d__.handName = handName;
			<TakeItemFromHands>d__.component = component;
			<TakeItemFromHands>d__.<>1__state = -1;
			<TakeItemFromHands>d__.<>t__builder.Start<StrippableSystem.<TakeItemFromHands>d__18>(ref <TakeItemFromHands>d__);
		}

		// Token: 0x040003B8 RID: 952
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x040003B9 RID: 953
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x040003BA RID: 954
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x040003BB RID: 955
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x040003BC RID: 956
		[Dependency]
		private readonly EnsnareableSystem _ensnaring;

		// Token: 0x040003BD RID: 957
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x040003BE RID: 958
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
