using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Actions.Events;
using Content.Server.DoAfter;
using Content.Server.Hands.Components;
using Content.Server.Hands.Systems;
using Content.Server.Wieldable.Components;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Wieldable
{
	// Token: 0x02000079 RID: 121
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WieldableSystem : EntitySystem
	{
		// Token: 0x060001BC RID: 444 RVA: 0x00009E1C File Offset: 0x0000801C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<WieldableComponent, UseInHandEvent>(new ComponentEventHandler<WieldableComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<WieldableComponent, DoAfterEvent>(new ComponentEventHandler<WieldableComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<WieldableComponent, ItemUnwieldedEvent>(new ComponentEventHandler<WieldableComponent, ItemUnwieldedEvent>(this.OnItemUnwielded), null, null);
			base.SubscribeLocalEvent<WieldableComponent, GotUnequippedHandEvent>(new ComponentEventHandler<WieldableComponent, GotUnequippedHandEvent>(this.OnItemLeaveHand), null, null);
			base.SubscribeLocalEvent<WieldableComponent, VirtualItemDeletedEvent>(new ComponentEventHandler<WieldableComponent, VirtualItemDeletedEvent>(this.OnVirtualItemDeleted), null, null);
			base.SubscribeLocalEvent<WieldableComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<WieldableComponent, GetVerbsEvent<InteractionVerb>>(this.AddToggleWieldVerb), null, null);
			base.SubscribeLocalEvent<WieldableComponent, DisarmAttemptEvent>(new ComponentEventHandler<WieldableComponent, DisarmAttemptEvent>(this.OnDisarmAttemptEvent), null, null);
			base.SubscribeLocalEvent<IncreaseDamageOnWieldComponent, MeleeHitEvent>(new ComponentEventHandler<IncreaseDamageOnWieldComponent, MeleeHitEvent>(this.OnMeleeHit), null, null);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00009ECF File Offset: 0x000080CF
		private void OnDisarmAttemptEvent(EntityUid uid, WieldableComponent component, DisarmAttemptEvent args)
		{
			if (component.Wielded)
			{
				args.Cancel();
			}
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00009EE0 File Offset: 0x000080E0
		private void AddToggleWieldVerb(EntityUid uid, WieldableComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (args.Hands == null || !args.CanAccess || !args.CanInteract)
			{
				return;
			}
			Hand hand;
			if (!this._handsSystem.IsHolding(args.User, new EntityUid?(uid), out hand, args.Hands))
			{
				return;
			}
			InteractionVerb verb = new InteractionVerb
			{
				Text = (component.Wielded ? Loc.GetString("wieldable-verb-text-unwield") : Loc.GetString("wieldable-verb-text-wield")),
				Act = (component.Wielded ? delegate()
				{
					this.AttemptUnwield(component.Owner, component, args.User);
				} : delegate()
				{
					this.AttemptWield(component.Owner, component, args.User);
				})
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00009FCA File Offset: 0x000081CA
		private void OnUseInHand(EntityUid uid, WieldableComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!component.Wielded)
			{
				this.AttemptWield(uid, component, args.User);
				return;
			}
			this.AttemptUnwield(uid, component, args.User);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00009FFC File Offset: 0x000081FC
		public bool CanWield(EntityUid uid, WieldableComponent component, EntityUid user, bool quiet = false)
		{
			HandsComponent hands;
			if (!this.EntityManager.TryGetComponent<HandsComponent>(user, ref hands))
			{
				if (!quiet)
				{
					this._popupSystem.PopupEntity(Loc.GetString("wieldable-component-no-hands"), user, user, PopupType.Small);
				}
				return false;
			}
			Hand hand;
			if (!this._handsSystem.IsHolding(user, new EntityUid?(uid), out hand, hands))
			{
				if (!quiet)
				{
					this._popupSystem.PopupEntity(Loc.GetString("wieldable-component-not-in-hands", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("item", uid)
					}), user, user, PopupType.Small);
				}
				return false;
			}
			if (hands.CountFreeHands() < component.FreeHandsRequired)
			{
				if (!quiet)
				{
					string message = Loc.GetString("wieldable-component-not-enough-free-hands", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("number", component.FreeHandsRequired),
						new ValueTuple<string, object>("item", uid)
					});
					this._popupSystem.PopupEntity(message, user, user, PopupType.Small);
				}
				return false;
			}
			return true;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000A0F4 File Offset: 0x000082F4
		public void AttemptWield(EntityUid used, WieldableComponent component, EntityUid user)
		{
			if (!this.CanWield(used, component, user, false))
			{
				return;
			}
			BeforeWieldEvent ev = new BeforeWieldEvent();
			base.RaiseLocalEvent<BeforeWieldEvent>(used, ev, false);
			if (ev.Cancelled)
			{
				return;
			}
			float wieldTime = component.WieldTime;
			EntityUid? used2 = new EntityUid?(used);
			DoAfterEventArgs doargs = new DoAfterEventArgs(user, wieldTime, default(CancellationToken), null, used2)
			{
				BreakOnUserMove = false,
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnTargetMove = true
			};
			this._doAfter.DoAfter(doargs);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000A178 File Offset: 0x00008378
		public void AttemptUnwield(EntityUid used, WieldableComponent component, EntityUid user)
		{
			BeforeUnwieldEvent ev = new BeforeUnwieldEvent();
			base.RaiseLocalEvent<BeforeUnwieldEvent>(used, ev, false);
			if (ev.Cancelled)
			{
				return;
			}
			ItemUnwieldedEvent targEv = new ItemUnwieldedEvent(new EntityUid?(user), false);
			base.RaiseLocalEvent<ItemUnwieldedEvent>(used, targEv, false);
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000A1B4 File Offset: 0x000083B4
		private void OnDoAfter(EntityUid uid, WieldableComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || !this.CanWield(uid, component, args.Args.User, false) || component.Wielded)
			{
				return;
			}
			ItemComponent item;
			if (base.TryComp<ItemComponent>(uid, ref item))
			{
				component.OldInhandPrefix = item.HeldPrefix;
				this._itemSystem.SetHeldPrefix(uid, component.WieldedInhandPrefix, item);
			}
			component.Wielded = true;
			if (component.WieldSound != null)
			{
				this._audioSystem.PlayPvs(component.WieldSound, uid, null);
			}
			for (int i = 0; i < component.FreeHandsRequired; i++)
			{
				this._virtualItemSystem.TrySpawnVirtualItemInHand(uid, args.Args.User);
			}
			this._popupSystem.PopupEntity(Loc.GetString("wieldable-component-successful-wield", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("item", uid)
			}), args.Args.User, args.Args.User, PopupType.Small);
			this._popupSystem.PopupEntity(Loc.GetString("wieldable-component-successful-wield-other", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("user", args.Args.User),
				new ValueTuple<string, object>("item", uid)
			}), args.Args.User, Filter.PvsExcept(args.Args.User, 2f, null), true, PopupType.Small);
			args.Handled = true;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000A334 File Offset: 0x00008534
		private void OnItemUnwielded(EntityUid uid, WieldableComponent component, ItemUnwieldedEvent args)
		{
			if (args.User == null)
			{
				return;
			}
			if (!component.Wielded)
			{
				return;
			}
			ItemComponent item;
			if (base.TryComp<ItemComponent>(uid, ref item))
			{
				this._itemSystem.SetHeldPrefix(uid, component.OldInhandPrefix, item);
			}
			component.Wielded = false;
			if (!args.Force)
			{
				if (component.UnwieldSound != null)
				{
					this._audioSystem.PlayPvs(component.UnwieldSound, uid, null);
				}
				this._popupSystem.PopupEntity(Loc.GetString("wieldable-component-failed-wield", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("item", uid)
				}), args.User.Value, args.User.Value, PopupType.Small);
				this._popupSystem.PopupEntity(Loc.GetString("wieldable-component-failed-wield-other", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", args.User.Value),
					new ValueTuple<string, object>("item", uid)
				}), args.User.Value, Filter.PvsExcept(args.User.Value, 2f, null), true, PopupType.Small);
			}
			this._virtualItemSystem.DeleteInHandsMatching(args.User.Value, uid);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000A482 File Offset: 0x00008682
		private void OnItemLeaveHand(EntityUid uid, WieldableComponent component, GotUnequippedHandEvent args)
		{
			if (!component.Wielded || component.Owner != args.Unequipped)
			{
				return;
			}
			base.RaiseLocalEvent<ItemUnwieldedEvent>(uid, new ItemUnwieldedEvent(new EntityUid?(args.User), true), true);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000A4B9 File Offset: 0x000086B9
		private void OnVirtualItemDeleted(EntityUid uid, WieldableComponent component, VirtualItemDeletedEvent args)
		{
			if (args.BlockingEntity == uid && component.Wielded)
			{
				this.AttemptUnwield(args.BlockingEntity, component, args.User);
			}
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000A4E4 File Offset: 0x000086E4
		private void OnMeleeHit(EntityUid uid, IncreaseDamageOnWieldComponent component, MeleeHitEvent args)
		{
			WieldableComponent wield;
			if (this.EntityManager.TryGetComponent<WieldableComponent>(uid, ref wield) && !wield.Wielded)
			{
				return;
			}
			if (args.Handled)
			{
				return;
			}
			args.BonusDamage += component.BonusDamage;
		}

		// Token: 0x04000145 RID: 325
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000146 RID: 326
		[Dependency]
		private readonly HandVirtualItemSystem _virtualItemSystem;

		// Token: 0x04000147 RID: 327
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000148 RID: 328
		[Dependency]
		private readonly SharedItemSystem _itemSystem;

		// Token: 0x04000149 RID: 329
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x0400014A RID: 330
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;
	}
}
