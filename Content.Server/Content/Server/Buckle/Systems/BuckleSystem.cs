using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Construction.Completions;
using Content.Server.Interaction;
using Content.Server.Popups;
using Content.Server.Pulling;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Bed.Sleep;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DragDrop;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Pulling.Components;
using Content.Shared.Standing;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Stunnable;
using Content.Shared.Vehicle.Components;
using Content.Shared.Verbs;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Buckle.Systems
{
	// Token: 0x020006F3 RID: 1779
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BuckleSystem : SharedBuckleSystem
	{
		// Token: 0x06002524 RID: 9508 RVA: 0x000C23D0 File Offset: 0x000C05D0
		private void InitializeBuckle()
		{
			base.SubscribeLocalEvent<BuckleComponent, ComponentStartup>(new ComponentEventHandler<BuckleComponent, ComponentStartup>(this.OnBuckleStartup), null, null);
			base.SubscribeLocalEvent<BuckleComponent, ComponentShutdown>(new ComponentEventHandler<BuckleComponent, ComponentShutdown>(this.OnBuckleShutdown), null, null);
			base.SubscribeLocalEvent<BuckleComponent, ComponentGetState>(new ComponentEventRefHandler<BuckleComponent, ComponentGetState>(this.OnBuckleGetState), null, null);
			base.SubscribeLocalEvent<BuckleComponent, MoveEvent>(new ComponentEventRefHandler<BuckleComponent, MoveEvent>(this.MoveEvent), null, null);
			base.SubscribeLocalEvent<BuckleComponent, InteractHandEvent>(new ComponentEventHandler<BuckleComponent, InteractHandEvent>(this.HandleInteractHand), null, null);
			base.SubscribeLocalEvent<BuckleComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<BuckleComponent, GetVerbsEvent<InteractionVerb>>(this.AddUnbuckleVerb), null, null);
			base.SubscribeLocalEvent<BuckleComponent, InsertIntoEntityStorageAttemptEvent>(new ComponentEventRefHandler<BuckleComponent, InsertIntoEntityStorageAttemptEvent>(this.OnEntityStorageInsertAttempt), null, null);
			base.SubscribeLocalEvent<BuckleComponent, CanDropDraggedEvent>(new ComponentEventRefHandler<BuckleComponent, CanDropDraggedEvent>(this.OnBuckleCanDrop), null, null);
			base.SubscribeLocalEvent<BuckleComponent, DragDropDraggedEvent>(new ComponentEventRefHandler<BuckleComponent, DragDropDraggedEvent>(this.OnBuckleDragDrop), null, null);
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x000C2494 File Offset: 0x000C0694
		private void AddUnbuckleVerb(EntityUid uid, BuckleComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || !component.Buckled)
			{
				return;
			}
			InteractionVerb verb = new InteractionVerb
			{
				Act = delegate()
				{
					this.TryUnbuckle(uid, args.User, false, component);
				},
				Text = Loc.GetString("verb-categories-unbuckle"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/unbuckle.svg.192dpi.png", "/"))
			};
			if (args.Target == args.User && args.Using == null)
			{
				verb.Priority = 1;
			}
			args.Verbs.Add(verb);
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x000C2575 File Offset: 0x000C0775
		private void OnBuckleStartup(EntityUid uid, BuckleComponent component, ComponentStartup args)
		{
			this.UpdateBuckleStatus(uid, component);
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x000C257F File Offset: 0x000C077F
		private void OnBuckleShutdown(EntityUid uid, BuckleComponent component, ComponentShutdown args)
		{
			this.TryUnbuckle(uid, uid, true, component);
			component.BuckleTime = default(TimeSpan);
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x000C2598 File Offset: 0x000C0798
		private void OnBuckleGetState(EntityUid uid, BuckleComponent component, ref ComponentGetState args)
		{
			args.State = new BuckleComponentState(component.Buckled, component.LastEntityBuckledTo, component.DontCollide);
		}

		// Token: 0x06002529 RID: 9513 RVA: 0x000C25B7 File Offset: 0x000C07B7
		private void HandleInteractHand(EntityUid uid, BuckleComponent component, InteractHandEvent args)
		{
			if (this.TryUnbuckle(uid, args.User, false, component))
			{
				args.Handled = true;
			}
		}

		// Token: 0x0600252A RID: 9514 RVA: 0x000C25D4 File Offset: 0x000C07D4
		private void MoveEvent(EntityUid uid, BuckleComponent buckle, ref MoveEvent ev)
		{
			StrapComponent strap = buckle.BuckledTo;
			if (strap == null)
			{
				return;
			}
			EntityCoordinates strapPosition = base.Transform(strap.Owner).Coordinates;
			if (ev.NewPosition.InRange(this.EntityManager, strapPosition, strap.MaxBuckleDistance))
			{
				return;
			}
			this.TryUnbuckle(uid, buckle.Owner, true, buckle);
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x000C2629 File Offset: 0x000C0829
		private void OnEntityStorageInsertAttempt(EntityUid uid, BuckleComponent comp, ref InsertIntoEntityStorageAttemptEvent args)
		{
			if (comp.Buckled)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x000C263A File Offset: 0x000C083A
		private void OnBuckleCanDrop(EntityUid uid, BuckleComponent component, ref CanDropDraggedEvent args)
		{
			args.Handled = base.HasComp<StrapComponent>(args.Target);
		}

		// Token: 0x0600252D RID: 9517 RVA: 0x000C264E File Offset: 0x000C084E
		private void OnBuckleDragDrop(EntityUid uid, BuckleComponent component, ref DragDropDraggedEvent args)
		{
			args.Handled = this.TryBuckle(uid, args.User, args.Target, component);
		}

		// Token: 0x0600252E RID: 9518 RVA: 0x000C266C File Offset: 0x000C086C
		private void UpdateBuckleStatus(EntityUid uid, BuckleComponent component)
		{
			if (component.Buckled)
			{
				StrapComponent buckledTo = component.BuckledTo;
				AlertType alertType = (buckledTo != null) ? buckledTo.BuckledAlertType : AlertType.Buckled;
				this._alerts.ShowAlert(uid, alertType, null, null);
				return;
			}
			this._alerts.ClearAlertCategory(uid, AlertCategory.Buckled);
		}

		// Token: 0x0600252F RID: 9519 RVA: 0x000C26C4 File Offset: 0x000C08C4
		private void SetBuckledTo(BuckleComponent buckle, [Nullable(2)] StrapComponent strap)
		{
			buckle.BuckledTo = strap;
			buckle.LastEntityBuckledTo = ((strap != null) ? new EntityUid?(strap.Owner) : null);
			if (strap == null)
			{
				buckle.Buckled = false;
			}
			else
			{
				buckle.DontCollide = true;
				buckle.Buckled = true;
				buckle.BuckleTime = this._gameTiming.CurTime;
			}
			this._actionBlocker.UpdateCanMove(buckle.Owner, null);
			this.UpdateBuckleStatus(buckle.Owner, buckle);
			base.Dirty(buckle, null);
		}

		// Token: 0x06002530 RID: 9520 RVA: 0x000C274C File Offset: 0x000C094C
		[NullableContext(2)]
		private bool CanBuckle(EntityUid buckleId, EntityUid user, EntityUid to, [NotNullWhen(true)] out StrapComponent strap, BuckleComponent buckle = null)
		{
			BuckleSystem.<>c__DisplayClass13_0 CS$<>8__locals1 = new BuckleSystem.<>c__DisplayClass13_0();
			CS$<>8__locals1.buckleId = buckleId;
			CS$<>8__locals1.user = user;
			strap = null;
			if (CS$<>8__locals1.user == to || !base.Resolve<BuckleComponent>(CS$<>8__locals1.buckleId, ref buckle, false) || !base.Resolve<StrapComponent>(to, ref strap, false))
			{
				return false;
			}
			CS$<>8__locals1.strapUid = strap.Owner;
			if (!this._interactions.InRangeUnobstructed(CS$<>8__locals1.buckleId, CS$<>8__locals1.strapUid, buckle.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<CanBuckle>g__Ignored|0), true))
			{
				return false;
			}
			IContainer ownerContainer;
			IContainer strapContainer;
			if (this._containers.TryGetContainingContainer(CS$<>8__locals1.buckleId, ref ownerContainer, null, null) && (!this._containers.TryGetContainingContainer(strap.Owner, ref strapContainer, null, null) || ownerContainer != strapContainer))
			{
				return false;
			}
			if (!base.HasComp<SharedHandsComponent>(CS$<>8__locals1.user))
			{
				this._popups.PopupEntity(Loc.GetString("buckle-component-no-hands-message"), CS$<>8__locals1.user, CS$<>8__locals1.user, PopupType.Small);
				return false;
			}
			if (buckle.Buckled)
			{
				string message = Loc.GetString((CS$<>8__locals1.buckleId == CS$<>8__locals1.user) ? "buckle-component-already-buckled-message" : "buckle-component-other-already-buckled-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", Identity.Entity(CS$<>8__locals1.buckleId, this.EntityManager))
				});
				this._popups.PopupEntity(message, CS$<>8__locals1.user, CS$<>8__locals1.user, PopupType.Small);
				return false;
			}
			EntityUid parent = base.Transform(to).ParentUid;
			while (parent.IsValid())
			{
				if (parent == CS$<>8__locals1.user)
				{
					string message2 = Loc.GetString((CS$<>8__locals1.buckleId == CS$<>8__locals1.user) ? "buckle-component-cannot-buckle-message" : "buckle-component-other-cannot-buckle-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("owner", Identity.Entity(CS$<>8__locals1.buckleId, this.EntityManager))
					});
					this._popups.PopupEntity(message2, CS$<>8__locals1.user, CS$<>8__locals1.user, PopupType.Small);
					return false;
				}
				parent = base.Transform(parent).ParentUid;
			}
			if (!this.StrapHasSpace(to, buckle, strap))
			{
				string message3 = Loc.GetString((CS$<>8__locals1.buckleId == CS$<>8__locals1.user) ? "buckle-component-cannot-fit-message" : "buckle-component-other-cannot-fit-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", Identity.Entity(CS$<>8__locals1.buckleId, this.EntityManager))
				});
				this._popups.PopupEntity(message3, CS$<>8__locals1.user, CS$<>8__locals1.user, PopupType.Small);
				return false;
			}
			return true;
		}

		// Token: 0x06002531 RID: 9521 RVA: 0x000C29E8 File Offset: 0x000C0BE8
		[NullableContext(2)]
		public bool TryBuckle(EntityUid buckleId, EntityUid user, EntityUid to, BuckleComponent buckle = null)
		{
			if (!base.Resolve<BuckleComponent>(buckleId, ref buckle, false))
			{
				return false;
			}
			StrapComponent strap;
			if (!this.CanBuckle(buckleId, user, to, out strap, buckle))
			{
				return false;
			}
			this._audio.PlayPvs(strap.BuckleSound, buckleId, null);
			if (!this.StrapTryAdd(to, buckle, false, strap))
			{
				string message = Loc.GetString((buckleId == user) ? "buckle-component-cannot-buckle-message" : "buckle-component-other-cannot-buckle-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", Identity.Entity(buckleId, this.EntityManager))
				});
				this._popups.PopupEntity(message, user, user, PopupType.Small);
				return false;
			}
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(buckleId, ref appearance))
			{
				this._appearance.SetData(buckleId, BuckleVisuals.Buckled, true, appearance);
			}
			base.ReAttach(buckleId, strap, buckle);
			this.SetBuckledTo(buckle, strap);
			BuckleChangeEvent ev = new BuckleChangeEvent
			{
				Buckling = true,
				Strap = strap.Owner,
				BuckledEntity = buckleId
			};
			base.RaiseLocalEvent<BuckleChangeEvent>(ev.BuckledEntity, ev, false);
			base.RaiseLocalEvent<BuckleChangeEvent>(ev.Strap, ev, false);
			SharedPullableComponent ownerPullable;
			if (base.TryComp<SharedPullableComponent>(buckleId, ref ownerPullable) && ownerPullable.Puller != null)
			{
				this._pulling.TryStopPull(ownerPullable, null);
			}
			SharedPullableComponent toPullable;
			if (base.TryComp<SharedPullableComponent>(to, ref toPullable))
			{
				EntityUid? puller = toPullable.Puller;
				if (puller != null && (puller == null || puller.GetValueOrDefault() == buckleId))
				{
					this._pulling.TryStopPull(toPullable, null);
				}
			}
			if (user != buckleId)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(13, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" buckled ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(buckleId), "ToPrettyString(buckleId)");
				logStringHandler.AppendLiteral(" to ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(to), "ToPrettyString(to)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Action;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(23, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" buckled themselves to ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(to), "ToPrettyString(to)");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			return true;
		}

		// Token: 0x06002532 RID: 9522 RVA: 0x000C2C60 File Offset: 0x000C0E60
		[NullableContext(2)]
		public bool TryUnbuckle(EntityUid buckleId, EntityUid user, bool force = false, BuckleComponent buckle = null)
		{
			if (base.Resolve<BuckleComponent>(buckleId, ref buckle, false))
			{
				StrapComponent oldBuckledTo = buckle.BuckledTo;
				if (oldBuckledTo != null)
				{
					if (!force)
					{
						if (this._gameTiming.CurTime < buckle.BuckleTime + buckle.UnbuckleDelay)
						{
							return false;
						}
						if (!this._interactions.InRangeUnobstructed(user, oldBuckledTo.Owner, buckle.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true))
						{
							return false;
						}
						if (base.HasComp<SleepingComponent>(buckleId) && buckleId == user)
						{
							return false;
						}
						VehicleComponent vehicle;
						if (base.TryComp<VehicleComponent>(oldBuckledTo.Owner, ref vehicle))
						{
							EntityUid? rider = vehicle.Rider;
							if (rider == null || (rider != null && rider.GetValueOrDefault() != user))
							{
								return false;
							}
						}
					}
					if (user != buckleId)
					{
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.Action;
						LogImpact impact = LogImpact.Low;
						LogStringHandler logStringHandler = new LogStringHandler(17, 3);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
						logStringHandler.AppendLiteral(" unbuckled ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(buckleId), "ToPrettyString(buckleId)");
						logStringHandler.AppendLiteral(" from ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(oldBuckledTo.Owner), "ToPrettyString(oldBuckledTo.Owner)");
						adminLogger.Add(type, impact, ref logStringHandler);
					}
					else
					{
						ISharedAdminLogManager adminLogger2 = this._adminLogger;
						LogType type2 = LogType.Action;
						LogImpact impact2 = LogImpact.Low;
						LogStringHandler logStringHandler = new LogStringHandler(27, 2);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
						logStringHandler.AppendLiteral(" unbuckled themselves from ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(oldBuckledTo.Owner), "ToPrettyString(oldBuckledTo.Owner)");
						adminLogger2.Add(type2, impact2, ref logStringHandler);
					}
					this.SetBuckledTo(buckle, null);
					TransformComponent xform = base.Transform(buckleId);
					TransformComponent oldBuckledXform = base.Transform(oldBuckledTo.Owner);
					if (xform.ParentUid == oldBuckledXform.Owner && !base.Terminating(xform.ParentUid, null))
					{
						this._containers.AttachParentToContainerOrGrid(xform);
						xform.WorldRotation = oldBuckledXform.WorldRotation;
						if (oldBuckledTo.UnbuckleOffset != Vector2.Zero)
						{
							xform.Coordinates = oldBuckledXform.Coordinates.Offset(oldBuckledTo.UnbuckleOffset);
						}
					}
					AppearanceComponent appearance;
					if (base.TryComp<AppearanceComponent>(buckleId, ref appearance))
					{
						this._appearance.SetData(buckleId, BuckleVisuals.Buckled, false, appearance);
					}
					MobStateComponent mobState;
					if ((base.TryComp<MobStateComponent>(buckleId, ref mobState) && this._mobState.IsIncapacitated(buckleId, mobState)) || base.HasComp<KnockedDownComponent>(buckleId))
					{
						this._standing.Down(buckleId, true, true, null, null, null);
					}
					else
					{
						this._standing.Stand(buckleId, null, null, false);
					}
					if (this._mobState.IsIncapacitated(buckleId, mobState))
					{
						this._standing.Down(buckleId, true, true, null, null, null);
					}
					this._appearance.SetData(oldBuckledTo.Owner, StrapVisuals.State, false, null);
					if (oldBuckledTo.BuckledEntities.Remove(buckleId))
					{
						oldBuckledTo.OccupiedSize -= buckle.Size;
						base.Dirty(oldBuckledTo, null);
					}
					this._audio.PlayPvs(oldBuckledTo.UnbuckleSound, buckleId, null);
					BuckleChangeEvent ev = new BuckleChangeEvent
					{
						Buckling = false,
						Strap = oldBuckledTo.Owner,
						BuckledEntity = buckleId
					};
					base.RaiseLocalEvent<BuckleChangeEvent>(buckleId, ev, false);
					base.RaiseLocalEvent<BuckleChangeEvent>(oldBuckledTo.Owner, ev, false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002533 RID: 9523 RVA: 0x000C2FBC File Offset: 0x000C11BC
		[NullableContext(2)]
		public bool ToggleBuckle(EntityUid buckleId, EntityUid user, EntityUid to, bool force = false, BuckleComponent buckle = null)
		{
			if (!base.Resolve<BuckleComponent>(buckleId, ref buckle, false))
			{
				return false;
			}
			StrapComponent buckledTo = buckle.BuckledTo;
			if (buckledTo != null && buckledTo.Owner == to)
			{
				return this.TryUnbuckle(buckleId, user, force, buckle);
			}
			return this.TryBuckle(buckleId, user, to, buckle);
		}

		// Token: 0x06002534 RID: 9524 RVA: 0x000C300A File Offset: 0x000C120A
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesAfter.Add(typeof(InteractionSystem));
			base.UpdatesAfter.Add(typeof(InputSystem));
			this.InitializeBuckle();
			this.InitializeStrap();
		}

		// Token: 0x06002535 RID: 9525 RVA: 0x000C3048 File Offset: 0x000C1248
		private void InitializeStrap()
		{
			base.SubscribeLocalEvent<StrapComponent, ComponentShutdown>(new ComponentEventHandler<StrapComponent, ComponentShutdown>(this.OnStrapShutdown), null, null);
			base.SubscribeLocalEvent<StrapComponent, ComponentRemove>(delegate(EntityUid _, StrapComponent c, ComponentRemove _)
			{
				this.StrapRemoveAll(c);
			}, null, null);
			base.SubscribeLocalEvent<StrapComponent, ComponentGetState>(new ComponentEventRefHandler<StrapComponent, ComponentGetState>(this.OnStrapGetState), null, null);
			base.SubscribeLocalEvent<StrapComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<StrapComponent, EntInsertedIntoContainerMessage>(this.ContainerModifiedStrap), null, null);
			base.SubscribeLocalEvent<StrapComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<StrapComponent, EntRemovedFromContainerMessage>(this.ContainerModifiedStrap), null, null);
			base.SubscribeLocalEvent<StrapComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<StrapComponent, GetVerbsEvent<InteractionVerb>>(this.AddStrapVerbs), null, null);
			base.SubscribeLocalEvent<StrapComponent, ContainerGettingInsertedAttemptEvent>(new ComponentEventHandler<StrapComponent, ContainerGettingInsertedAttemptEvent>(this.OnStrapInsertAttempt), null, null);
			base.SubscribeLocalEvent<StrapComponent, InteractHandEvent>(new ComponentEventHandler<StrapComponent, InteractHandEvent>(this.OnStrapInteractHand), null, null);
			base.SubscribeLocalEvent<StrapComponent, DestructionEventArgs>(delegate(EntityUid _, StrapComponent c, DestructionEventArgs _)
			{
				this.StrapRemoveAll(c);
			}, null, null);
			base.SubscribeLocalEvent<StrapComponent, BreakageEventArgs>(delegate(EntityUid _, StrapComponent c, BreakageEventArgs _)
			{
				this.StrapRemoveAll(c);
			}, null, null);
			base.SubscribeLocalEvent<StrapComponent, ConstructionBeforeDeleteEvent>(delegate(EntityUid _, StrapComponent c, ConstructionBeforeDeleteEvent _)
			{
				this.StrapRemoveAll(c);
			}, null, null);
			base.SubscribeLocalEvent<StrapComponent, DragDropTargetEvent>(new ComponentEventRefHandler<StrapComponent, DragDropTargetEvent>(this.OnStrapDragDrop), null, null);
		}

		// Token: 0x06002536 RID: 9526 RVA: 0x000C3145 File Offset: 0x000C1345
		private void OnStrapGetState(EntityUid uid, StrapComponent component, ref ComponentGetState args)
		{
			args.State = new StrapComponentState(component.Position, component.BuckleOffset, component.BuckledEntities, component.MaxBuckleDistance);
		}

		// Token: 0x06002537 RID: 9527 RVA: 0x000C316C File Offset: 0x000C136C
		private void ContainerModifiedStrap(EntityUid uid, StrapComponent strap, ContainerModifiedMessage message)
		{
			if (this.GameTiming.ApplyingState)
			{
				return;
			}
			foreach (EntityUid buckledEntity in strap.BuckledEntities)
			{
				BuckleComponent buckled;
				if (base.TryComp<BuckleComponent>(buckledEntity, ref buckled))
				{
					this.ContainerModifiedReAttach(buckledEntity, strap.Owner, buckled, strap);
				}
			}
		}

		// Token: 0x06002538 RID: 9528 RVA: 0x000C31E0 File Offset: 0x000C13E0
		[NullableContext(2)]
		private void ContainerModifiedReAttach(EntityUid buckleId, EntityUid strapId, BuckleComponent buckle = null, StrapComponent strap = null)
		{
			if (!base.Resolve<BuckleComponent>(buckleId, ref buckle, false) || !base.Resolve<StrapComponent>(strapId, ref strap, false))
			{
				return;
			}
			IContainer ownContainer;
			bool contained = this._containers.TryGetContainingContainer(buckleId, ref ownContainer, null, null);
			IContainer strapContainer;
			bool strapContained = this._containers.TryGetContainingContainer(strapId, ref strapContainer, null, null);
			if (contained != strapContained || ownContainer != strapContainer)
			{
				this.TryUnbuckle(buckleId, buckle.Owner, true, buckle);
				return;
			}
			if (!contained)
			{
				base.ReAttach(buckleId, strap, buckle);
			}
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x000C324E File Offset: 0x000C144E
		private void OnStrapShutdown(EntityUid uid, StrapComponent component, ComponentShutdown args)
		{
			if (base.LifeStage(uid, null) > 3)
			{
				return;
			}
			this.StrapRemoveAll(component);
		}

		// Token: 0x0600253A RID: 9530 RVA: 0x000C3263 File Offset: 0x000C1463
		private void OnStrapInsertAttempt(EntityUid uid, StrapComponent component, ContainerGettingInsertedAttemptEvent args)
		{
			if (base.HasComp<SharedStorageComponent>(args.Container.Owner) && component.BuckledEntities.Count != 0)
			{
				args.Cancel();
			}
		}

		// Token: 0x0600253B RID: 9531 RVA: 0x000C328B File Offset: 0x000C148B
		private void OnStrapInteractHand(EntityUid uid, StrapComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.ToggleBuckle(args.User, args.User, uid, false, null);
		}

		// Token: 0x0600253C RID: 9532 RVA: 0x000C32AC File Offset: 0x000C14AC
		private void AddStrapVerbs(EntityUid uid, StrapComponent strap, GetVerbsEvent<InteractionVerb> args)
		{
			BuckleSystem.<>c__DisplayClass36_0 CS$<>8__locals1 = new BuckleSystem.<>c__DisplayClass36_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.args = args;
			if (CS$<>8__locals1.args.Hands == null || !CS$<>8__locals1.args.CanAccess || !CS$<>8__locals1.args.CanInteract || !strap.Enabled)
			{
				return;
			}
			using (HashSet<EntityUid>.Enumerator enumerator = strap.BuckledEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EntityUid entity = enumerator.Current;
					BuckleComponent buckledComp = base.Comp<BuckleComponent>(entity);
					if (this._interactions.InRangeUnobstructed(CS$<>8__locals1.args.User, CS$<>8__locals1.args.Target, buckledComp.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
					{
						InteractionVerb verb = new InteractionVerb
						{
							Act = delegate()
							{
								CS$<>8__locals1.<>4__this.TryUnbuckle(entity, CS$<>8__locals1.args.User, false, buckledComp);
							},
							Category = VerbCategory.Unbuckle
						};
						if (entity == CS$<>8__locals1.args.User)
						{
							verb.Text = Loc.GetString("verb-self-target-pronoun");
						}
						else
						{
							verb.Text = base.Comp<MetaDataComponent>(entity).EntityName;
						}
						CS$<>8__locals1.args.Verbs.Add(verb);
					}
				}
			}
			if (base.TryComp<BuckleComponent>(CS$<>8__locals1.args.User, ref CS$<>8__locals1.buckle) && CS$<>8__locals1.buckle.BuckledTo != strap && CS$<>8__locals1.args.User != strap.Owner && this.StrapHasSpace(uid, CS$<>8__locals1.buckle, strap) && this._interactions.InRangeUnobstructed(CS$<>8__locals1.args.User, CS$<>8__locals1.args.Target, CS$<>8__locals1.buckle.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				InteractionVerb verb2 = new InteractionVerb
				{
					Act = delegate()
					{
						CS$<>8__locals1.<>4__this.TryBuckle(CS$<>8__locals1.args.User, CS$<>8__locals1.args.User, CS$<>8__locals1.args.Target, CS$<>8__locals1.buckle);
					},
					Category = VerbCategory.Buckle,
					Text = Loc.GetString("verb-self-target-pronoun")
				};
				CS$<>8__locals1.args.Verbs.Add(verb2);
			}
			EntityUid? @using = CS$<>8__locals1.args.Using;
			if (@using != null)
			{
				CS$<>8__locals1.@using = @using.GetValueOrDefault();
				if (CS$<>8__locals1.@using.Valid && base.TryComp<BuckleComponent>(CS$<>8__locals1.@using, ref CS$<>8__locals1.usingBuckle) && this.StrapHasSpace(uid, CS$<>8__locals1.usingBuckle, strap) && this._interactions.InRangeUnobstructed(CS$<>8__locals1.@using, CS$<>8__locals1.args.Target, CS$<>8__locals1.usingBuckle.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
				{
					if (!this._interactions.InRangeUnobstructed(CS$<>8__locals1.@using, CS$<>8__locals1.args.Target, CS$<>8__locals1.usingBuckle.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<AddStrapVerbs>g__Ignored|2), false))
					{
						return;
					}
					InteractionVerb verb3 = new InteractionVerb
					{
						Act = delegate()
						{
							CS$<>8__locals1.<>4__this.TryBuckle(CS$<>8__locals1.@using, CS$<>8__locals1.args.User, CS$<>8__locals1.args.Target, CS$<>8__locals1.usingBuckle);
						},
						Category = VerbCategory.Buckle,
						Text = base.Comp<MetaDataComponent>(CS$<>8__locals1.@using).EntityName,
						Priority = (base.HasComp<ActorComponent>(CS$<>8__locals1.@using) ? 1 : -1)
					};
					CS$<>8__locals1.args.Verbs.Add(verb3);
				}
			}
		}

		// Token: 0x0600253D RID: 9533 RVA: 0x000C3640 File Offset: 0x000C1840
		private void StrapRemoveAll(StrapComponent strap)
		{
			foreach (EntityUid entity in strap.BuckledEntities.ToArray<EntityUid>())
			{
				this.TryUnbuckle(entity, entity, true, null);
			}
			strap.BuckledEntities.Clear();
			strap.OccupiedSize = 0;
			base.Dirty(strap, null);
		}

		// Token: 0x0600253E RID: 9534 RVA: 0x000C3694 File Offset: 0x000C1894
		private void OnStrapDragDrop(EntityUid uid, StrapComponent component, ref DragDropTargetEvent args)
		{
			if (!base.StrapCanDragDropOn(uid, args.User, uid, args.Dragged, component, null))
			{
				return;
			}
			args.Handled = this.TryBuckle(args.Dragged, args.User, uid, null);
		}

		// Token: 0x0600253F RID: 9535 RVA: 0x000C36C9 File Offset: 0x000C18C9
		private bool StrapHasSpace(EntityUid strapId, BuckleComponent buckle, [Nullable(2)] StrapComponent strap = null)
		{
			return base.Resolve<StrapComponent>(strapId, ref strap, false) && strap.OccupiedSize + buckle.Size <= strap.Size;
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x000C36F4 File Offset: 0x000C18F4
		private bool StrapTryAdd(EntityUid strapId, BuckleComponent buckle, bool force = false, [Nullable(2)] StrapComponent strap = null)
		{
			if (!base.Resolve<StrapComponent>(strapId, ref strap, false) || !strap.Enabled)
			{
				return false;
			}
			if (!force && !this.StrapHasSpace(strapId, buckle, strap))
			{
				return false;
			}
			if (!strap.BuckledEntities.Add(buckle.Owner))
			{
				return false;
			}
			strap.OccupiedSize += buckle.Size;
			this._appearance.SetData(buckle.Owner, StrapVisuals.RotationAngle, strap.Rotation, null);
			this._appearance.SetData(strap.Owner, StrapVisuals.State, true, null);
			base.Dirty(strap, null);
			return true;
		}

		// Token: 0x06002541 RID: 9537 RVA: 0x000C37A0 File Offset: 0x000C19A0
		[NullableContext(2)]
		public void StrapSetEnabled(EntityUid strapId, bool enabled, StrapComponent strap = null)
		{
			if (!base.Resolve<StrapComponent>(strapId, ref strap, false) || strap.Enabled == enabled)
			{
				return;
			}
			strap.Enabled = enabled;
			if (!enabled)
			{
				this.StrapRemoveAll(strap);
			}
		}

		// Token: 0x040016D3 RID: 5843
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040016D4 RID: 5844
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040016D5 RID: 5845
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x040016D6 RID: 5846
		[Dependency]
		private readonly AlertsSystem _alerts;

		// Token: 0x040016D7 RID: 5847
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x040016D8 RID: 5848
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040016D9 RID: 5849
		[Dependency]
		private readonly ContainerSystem _containers;

		// Token: 0x040016DA RID: 5850
		[Dependency]
		private readonly InteractionSystem _interactions;

		// Token: 0x040016DB RID: 5851
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x040016DC RID: 5852
		[Dependency]
		private readonly PopupSystem _popups;

		// Token: 0x040016DD RID: 5853
		[Dependency]
		private readonly PullingSystem _pulling;

		// Token: 0x040016DE RID: 5854
		[Dependency]
		private readonly StandingStateSystem _standing;
	}
}
