using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Cuffs.Components;
using Content.Server.Hands.Components;
using Content.Server.Hands.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Cuffs;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Cuffs
{
	// Token: 0x020005D3 RID: 1491
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CuffableSystem : SharedCuffableSystem
	{
		// Token: 0x06001FBB RID: 8123 RVA: 0x000A6188 File Offset: 0x000A4388
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HandCountChangedEvent>(new EntityEventHandler<HandCountChangedEvent>(this.OnHandCountChanged), null, null);
			base.SubscribeLocalEvent<UncuffAttemptEvent>(new EntityEventHandler<UncuffAttemptEvent>(this.OnUncuffAttempt), null, null);
			base.SubscribeLocalEvent<CuffableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<CuffableComponent, GetVerbsEvent<Verb>>(this.AddUncuffVerb), null, null);
			base.SubscribeLocalEvent<HandcuffComponent, AfterInteractEvent>(new ComponentEventHandler<HandcuffComponent, AfterInteractEvent>(this.OnCuffAfterInteract), null, null);
			base.SubscribeLocalEvent<HandcuffComponent, MeleeHitEvent>(new ComponentEventHandler<HandcuffComponent, MeleeHitEvent>(this.OnCuffMeleeHit), null, null);
			base.SubscribeLocalEvent<CuffableComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<CuffableComponent, EntRemovedFromContainerMessage>(this.OnCuffsRemoved), null, null);
		}

		// Token: 0x06001FBC RID: 8124 RVA: 0x000A6213 File Offset: 0x000A4413
		private void OnCuffsRemoved(EntityUid uid, CuffableComponent component, EntRemovedFromContainerMessage args)
		{
			if (args.Container.ID == component.Container.ID)
			{
				this._virtualSystem.DeleteInHandsMatching(uid, args.Entity);
			}
		}

		// Token: 0x06001FBD RID: 8125 RVA: 0x000A6244 File Offset: 0x000A4444
		private void AddUncuffVerb(EntityUid uid, CuffableComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || component.CuffedHandCount == 0 || args.Hands == null)
			{
				return;
			}
			if (args.User != args.Target && !args.CanInteract)
			{
				return;
			}
			Verb verb = new Verb
			{
				Act = delegate()
				{
					component.TryUncuff(args.User, null);
				},
				DoContactInteraction = new bool?(true),
				Text = Loc.GetString("uncuff-verb-get-data-text")
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001FBE RID: 8126 RVA: 0x000A6300 File Offset: 0x000A4500
		private void OnCuffAfterInteract(EntityUid uid, HandcuffComponent component, AfterInteractEvent args)
		{
			EntityUid? target = args.Target;
			if (target == null || !target.GetValueOrDefault().Valid)
			{
				return;
			}
			if (!args.CanReach)
			{
				this._popup.PopupEntity(Loc.GetString("handcuff-component-too-far-away-error"), args.User, args.User, PopupType.Small);
				return;
			}
			this.TryCuffing(uid, args.User, args.Target.Value, component);
			args.Handled = true;
		}

		// Token: 0x06001FBF RID: 8127 RVA: 0x000A6380 File Offset: 0x000A4580
		private void TryCuffing(EntityUid handcuff, EntityUid user, EntityUid target, HandcuffComponent component)
		{
			CuffableComponent cuffed;
			if (component.Cuffing || !this.EntityManager.TryGetComponent<CuffableComponent>(target, ref cuffed))
			{
				return;
			}
			HandsComponent hands;
			if (!this.EntityManager.TryGetComponent<HandsComponent>(target, ref hands))
			{
				this._popup.PopupEntity(Loc.GetString("handcuff-component-target-has-no-hands-error", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("targetName", target)
				}), user, user, PopupType.Small);
				return;
			}
			if (cuffed.CuffedHandCount >= hands.Count)
			{
				this._popup.PopupEntity(Loc.GetString("handcuff-component-target-has-no-free-hands-error", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("targetName", target)
				}), user, user, PopupType.Small);
				return;
			}
			if (target == user)
			{
				this._popup.PopupEntity(Loc.GetString("handcuff-component-target-self"), user, user, PopupType.Small);
			}
			else
			{
				this._popup.PopupEntity(Loc.GetString("handcuff-component-start-cuffing-target-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("targetName", target)
				}), user, user, PopupType.Small);
				this._popup.PopupEntity(Loc.GetString("handcuff-component-start-cuffing-by-other-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("otherName", user)
				}), target, target, PopupType.Small);
			}
			this._audio.PlayPvs(component.StartCuffSound, handcuff, null);
			component.TryUpdateCuff(user, target, cuffed);
		}

		// Token: 0x06001FC0 RID: 8128 RVA: 0x000A64E5 File Offset: 0x000A46E5
		private void OnCuffMeleeHit(EntityUid uid, HandcuffComponent component, MeleeHitEvent args)
		{
			if (!args.HitEntities.Any<EntityUid>())
			{
				return;
			}
			this.TryCuffing(uid, args.User, args.HitEntities.First<EntityUid>(), component);
			args.Handled = true;
		}

		// Token: 0x06001FC1 RID: 8129 RVA: 0x000A6518 File Offset: 0x000A4718
		private void OnUncuffAttempt(UncuffAttemptEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			if (!this.EntityManager.EntityExists(args.User))
			{
				args.Cancel();
				return;
			}
			if (args.User == args.Target)
			{
				if (this._mobState.IsIncapacitated(args.User, null))
				{
					args.Cancel();
				}
			}
			else if (!this._actionBlockerSystem.CanInteract(args.User, new EntityUid?(args.Target)))
			{
				args.Cancel();
			}
			if (args.Cancelled)
			{
				this._popup.PopupEntity(Loc.GetString("cuffable-component-cannot-interact-message"), args.Target, args.User, PopupType.Small);
			}
		}

		// Token: 0x06001FC2 RID: 8130 RVA: 0x000A65C8 File Offset: 0x000A47C8
		private void OnHandCountChanged(HandCountChangedEvent message)
		{
			EntityUid owner = message.Sender;
			CuffableComponent cuffable;
			if (!this.EntityManager.TryGetComponent<CuffableComponent>(owner, ref cuffable) || !cuffable.Initialized)
			{
				return;
			}
			bool dirty = false;
			HandsComponent componentOrNull = EntityManagerExt.GetComponentOrNull<HandsComponent>(this.EntityManager, owner);
			int handCount = (componentOrNull != null) ? componentOrNull.Count : 0;
			while (cuffable.CuffedHandCount > handCount && cuffable.CuffedHandCount > 0)
			{
				dirty = true;
				Container container = cuffable.Container;
				IReadOnlyList<EntityUid> containedEntities = container.ContainedEntities;
				EntityUid entity = containedEntities[containedEntities.Count - 1];
				container.Remove(entity, null, null, null, true, false, null, null);
				this.EntityManager.GetComponent<TransformComponent>(entity).WorldPosition = this.EntityManager.GetComponent<TransformComponent>(owner).WorldPosition;
			}
			if (dirty)
			{
				base.UpdateCuffState(owner, cuffable);
			}
		}

		// Token: 0x040013B4 RID: 5044
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x040013B5 RID: 5045
		[Dependency]
		private readonly HandVirtualItemSystem _virtualSystem;

		// Token: 0x040013B6 RID: 5046
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040013B7 RID: 5047
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x040013B8 RID: 5048
		[Dependency]
		private readonly SharedPopupSystem _popup;
	}
}
