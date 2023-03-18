using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Server.Sticky.Components;
using Content.Server.Sticky.Events;
using Content.Shared.DoAfter;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Sticky.Components;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Sticky.Systems
{
	// Token: 0x02000171 RID: 369
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StickySystem : EntitySystem
	{
		// Token: 0x06000753 RID: 1875 RVA: 0x00024444 File Offset: 0x00022644
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StickyComponent, DoAfterEvent>(new ComponentEventHandler<StickyComponent, DoAfterEvent>(this.OnStickSuccessful), null, null);
			base.SubscribeLocalEvent<StickyComponent, AfterInteractEvent>(new ComponentEventHandler<StickyComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<StickyComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<StickyComponent, GetVerbsEvent<Verb>>(this.AddUnstickVerb), null, null);
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x00024494 File Offset: 0x00022694
		private void OnAfterInteract(EntityUid uid, StickyComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach || args.Target == null)
			{
				return;
			}
			args.Handled = this.StartSticking(uid, args.User, args.Target.Value, component);
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x000244E4 File Offset: 0x000226E4
		private void AddUnstickVerb(EntityUid uid, StickyComponent component, GetVerbsEvent<Verb> args)
		{
			if (component.StuckTo == null || !component.CanUnstick || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			if (!this._interactionSystem.InRangeUnobstructed(uid, args.User, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, delegate(EntityUid entity)
			{
				EntityUid? stuckTo = component.StuckTo;
				return stuckTo != null && (stuckTo == null || stuckTo.GetValueOrDefault() == entity);
			}, false))
			{
				return;
			}
			args.Verbs.Add(new Verb
			{
				DoContactInteraction = new bool?(true),
				Text = Loc.GetString("comp-sticky-unstick-verb-text"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/eject.svg.192dpi.png", "/")),
				Act = delegate()
				{
					this.StartUnsticking(uid, args.User, component);
				}
			});
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x000245E4 File Offset: 0x000227E4
		[NullableContext(2)]
		private bool StartSticking(EntityUid uid, EntityUid user, EntityUid target, StickyComponent component = null)
		{
			if (!base.Resolve<StickyComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.Whitelist != null && !component.Whitelist.IsValid(target, null))
			{
				return false;
			}
			if (component.Blacklist != null && component.Blacklist.IsValid(target, null))
			{
				return false;
			}
			float delay = (float)component.StickDelay.TotalSeconds;
			if (delay > 0f)
			{
				if (component.StickPopupStart != null)
				{
					string msg = Loc.GetString(component.StickPopupStart);
					this._popupSystem.PopupEntity(msg, user, user, PopupType.Small);
				}
				component.Stick = true;
				SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
				float delay2 = delay;
				EntityUid? target2 = new EntityUid?(target);
				EntityUid? used = new EntityUid?(uid);
				doAfterSystem.DoAfter(new DoAfterEventArgs(user, delay2, default(CancellationToken), target2, used)
				{
					BreakOnStun = true,
					BreakOnTargetMove = true,
					BreakOnUserMove = true,
					NeedHand = true
				});
			}
			else
			{
				this.StickToEntity(uid, target, user, component);
			}
			return true;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x000246D4 File Offset: 0x000228D4
		private void OnStickSuccessful(EntityUid uid, StickyComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null)
			{
				return;
			}
			if (component.Stick)
			{
				this.StickToEntity(uid, args.Args.Target.Value, args.Args.User, component);
			}
			else
			{
				this.UnstickFromEntity(uid, args.Args.User, component);
			}
			args.Handled = true;
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x0002474C File Offset: 0x0002294C
		[NullableContext(2)]
		private void StartUnsticking(EntityUid uid, EntityUid user, StickyComponent component = null)
		{
			if (!base.Resolve<StickyComponent>(uid, ref component, true))
			{
				return;
			}
			float delay = (float)component.UnstickDelay.TotalSeconds;
			if (delay > 0f)
			{
				if (component.UnstickPopupStart != null)
				{
					string msg = Loc.GetString(component.UnstickPopupStart);
					this._popupSystem.PopupEntity(msg, user, user, PopupType.Small);
				}
				component.Stick = false;
				SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
				float delay2 = delay;
				EntityUid? target = new EntityUid?(uid);
				doAfterSystem.DoAfter(new DoAfterEventArgs(user, delay2, default(CancellationToken), target, null)
				{
					BreakOnStun = true,
					BreakOnTargetMove = true,
					BreakOnUserMove = true,
					NeedHand = true
				});
				return;
			}
			this.UnstickFromEntity(uid, user, component);
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x000247FC File Offset: 0x000229FC
		[NullableContext(2)]
		public void StickToEntity(EntityUid uid, EntityUid target, EntityUid user, StickyComponent component = null)
		{
			if (!base.Resolve<StickyComponent>(uid, ref component, true))
			{
				return;
			}
			Container container = this._containerSystem.EnsureContainer<Container>(target, "stickers_container", null);
			container.ShowContents = true;
			if (!container.Insert(uid, null, null, null, null, null))
			{
				return;
			}
			if (component.StickPopupSuccess != null)
			{
				string msg = Loc.GetString(component.StickPopupSuccess);
				this._popupSystem.PopupEntity(msg, user, user, PopupType.Small);
			}
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, StickyVisuals.IsStuck, true, appearance);
			}
			component.StuckTo = new EntityUid?(target);
			base.RaiseLocalEvent<EntityStuckEvent>(uid, new EntityStuckEvent(target, user), true);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x000248A4 File Offset: 0x00022AA4
		[NullableContext(2)]
		public void UnstickFromEntity(EntityUid uid, EntityUid user, StickyComponent component = null)
		{
			if (!base.Resolve<StickyComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.StuckTo == null)
			{
				return;
			}
			EntityUid target = component.StuckTo.Value;
			IContainer container;
			if (!this._containerSystem.TryGetContainer(target, "stickers_container", ref container, null) || !container.Remove(uid, null, null, null, true, false, null, null))
			{
				return;
			}
			if (container.ContainedEntities.Count == 0)
			{
				container.Shutdown(null, null);
			}
			this._handsSystem.PickupOrDrop(new EntityUid?(user), uid, true, false, null, null);
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, StickyVisuals.IsStuck, false, appearance);
			}
			if (component.UnstickPopupSuccess != null)
			{
				string msg = Loc.GetString(component.UnstickPopupSuccess);
				this._popupSystem.PopupEntity(msg, user, user, PopupType.Small);
			}
			component.StuckTo = null;
			base.RaiseLocalEvent<EntityUnstuckEvent>(uid, new EntityUnstuckEvent(target, user), true);
		}

		// Token: 0x0400045E RID: 1118
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x0400045F RID: 1119
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000460 RID: 1120
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04000461 RID: 1121
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000462 RID: 1122
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000463 RID: 1123
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000464 RID: 1124
		private const string StickerSlotId = "stickers_container";
	}
}
