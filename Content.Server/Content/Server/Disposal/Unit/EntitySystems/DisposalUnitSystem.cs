using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Disposal.Tube.Components;
using Content.Server.Disposal.Unit.Components;
using Content.Server.DoAfter;
using Content.Server.Hands.Components;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Construction.Components;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.Disposal;
using Content.Shared.Disposal.Components;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Disposal.Unit.EntitySystems
{
	// Token: 0x0200054E RID: 1358
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DisposalUnitSystem : SharedDisposalUnitSystem
	{
		// Token: 0x06001C7C RID: 7292 RVA: 0x00097F18 File Offset: 0x00096118
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DisposalUnitComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<DisposalUnitComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, ContainerRelayMovementEntityEvent>(new ComponentEventRefHandler<DisposalUnitComponent, ContainerRelayMovementEntityEvent>(this.HandleMovement), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, PowerChangedEvent>(new ComponentEventRefHandler<DisposalUnitComponent, PowerChangedEvent>(this.HandlePowerChange), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, ComponentInit>(new ComponentEventHandler<DisposalUnitComponent, ComponentInit>(this.HandleDisposalInit), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, ComponentRemove>(new ComponentEventHandler<DisposalUnitComponent, ComponentRemove>(this.HandleDisposalRemove), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, ThrowHitByEvent>(new ComponentEventHandler<DisposalUnitComponent, ThrowHitByEvent>(this.HandleThrowCollide), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, ActivateInWorldEvent>(new ComponentEventHandler<DisposalUnitComponent, ActivateInWorldEvent>(this.HandleActivate), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, AfterInteractUsingEvent>(new ComponentEventHandler<DisposalUnitComponent, AfterInteractUsingEvent>(this.HandleAfterInteractUsing), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, DragDropTargetEvent>(new ComponentEventRefHandler<DisposalUnitComponent, DragDropTargetEvent>(this.HandleDragDropOn), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, DestructionEventArgs>(new ComponentEventHandler<DisposalUnitComponent, DestructionEventArgs>(this.HandleDestruction), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<DisposalUnitComponent, GetVerbsEvent<InteractionVerb>>(this.AddInsertVerb), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<DisposalUnitComponent, GetVerbsEvent<AlternativeVerb>>(this.AddDisposalAltVerbs), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<DisposalUnitComponent, GetVerbsEvent<Verb>>(this.AddClimbInsideVerb), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, DoAfterEvent>(new ComponentEventHandler<DisposalUnitComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, SharedDisposalUnitComponent.UiButtonPressedMessage>(new ComponentEventHandler<DisposalUnitComponent, SharedDisposalUnitComponent.UiButtonPressedMessage>(this.OnUiButtonPressed), null, null);
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x00098058 File Offset: 0x00096258
		private void AddDisposalAltVerbs(EntityUid uid, DisposalUnitComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			if (component.Container.ContainedEntities.Count > 0)
			{
				AlternativeVerb flushVerb = new AlternativeVerb();
				flushVerb.Act = delegate()
				{
					this.Engage(uid, component);
				};
				flushVerb.Text = Loc.GetString("disposal-flush-verb-get-data-text");
				flushVerb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/delete_transparent.svg.192dpi.png", "/"));
				flushVerb.Priority = 1;
				args.Verbs.Add(flushVerb);
				AlternativeVerb ejectVerb = new AlternativeVerb
				{
					Act = delegate()
					{
						this.TryEjectContents(uid, component);
					},
					Category = VerbCategory.Eject,
					Text = Loc.GetString("disposal-eject-verb-get-data-text")
				};
				args.Verbs.Add(ejectVerb);
			}
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x00098144 File Offset: 0x00096344
		private void AddClimbInsideVerb(EntityUid uid, DisposalUnitComponent component, GetVerbsEvent<Verb> args)
		{
			if (!component.MobsCanEnter || !args.CanAccess || !args.CanInteract || component.Container.ContainedEntities.Contains(args.User) || !this._actionBlockerSystem.CanMove(args.User, null))
			{
				return;
			}
			Verb verb = new Verb
			{
				Act = delegate()
				{
					this.TryInsert(uid, args.User, new EntityUid?(args.User), null);
				},
				DoContactInteraction = new bool?(true),
				Text = Loc.GetString("disposal-self-insert-verb-get-data-text")
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x0009820C File Offset: 0x0009640C
		private void AddInsertVerb(EntityUid uid, DisposalUnitComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null || args.Using == null)
			{
				return;
			}
			if (!this._actionBlockerSystem.CanDrop(args.User))
			{
				return;
			}
			if (!this.CanInsert(component, args.Using.Value))
			{
				return;
			}
			InteractionVerb insertVerb = new InteractionVerb
			{
				Text = base.Name(args.Using.Value, null),
				Category = VerbCategory.Insert,
				Act = delegate()
				{
					this._handsSystem.TryDropIntoContainer(args.User, args.Using.Value, component.Container, false, args.Hands);
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Medium;
					LogStringHandler logStringHandler = new LogStringHandler(16, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
					logStringHandler.AppendLiteral(" inserted ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(args.Using.Value), "ToPrettyString(args.Using.Value)");
					logStringHandler.AppendLiteral(" into ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(uid), "ToPrettyString(uid)");
					adminLogger.Add(type, impact, ref logStringHandler);
					this.AfterInsert(uid, component, args.Using.Value, new EntityUid?(args.User));
				}
			};
			args.Verbs.Add(insertVerb);
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x00098300 File Offset: 0x00096500
		private void OnDoAfter(EntityUid uid, DisposalUnitComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null || args.Args.Used == null)
			{
				return;
			}
			this.AfterInsert(uid, component, args.Args.Target.Value, new EntityUid?(args.Args.User));
			args.Handled = true;
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x00098374 File Offset: 0x00096574
		[NullableContext(2)]
		public void DoInsertDisposalUnit(EntityUid uid, EntityUid toInsert, EntityUid user, DisposalUnitComponent disposal = null)
		{
			if (!base.Resolve<DisposalUnitComponent>(uid, ref disposal, true))
			{
				return;
			}
			if (!disposal.Container.Insert(toInsert, null, null, null, null, null))
			{
				return;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(16, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
			logStringHandler.AppendLiteral(" inserted ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(toInsert), "ToPrettyString(toInsert)");
			logStringHandler.AppendLiteral(" into ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.AfterInsert(uid, disposal, toInsert, new EntityUid?(user));
		}

		// Token: 0x06001C82 RID: 7298 RVA: 0x00098424 File Offset: 0x00096624
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveDisposalUnitComponent, DisposalUnitComponent> valueTuple in base.EntityQuery<ActiveDisposalUnitComponent, DisposalUnitComponent>(false))
			{
				DisposalUnitComponent comp = valueTuple.Item2;
				EntityUid uid = comp.Owner;
				if (this.Update(uid, comp, frameTime))
				{
					base.RemComp<ActiveDisposalUnitComponent>(uid);
				}
			}
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x00098494 File Offset: 0x00096694
		private void OnUiButtonPressed(EntityUid uid, DisposalUnitComponent component, SharedDisposalUnitComponent.UiButtonPressedMessage args)
		{
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid player = attachedEntity.GetValueOrDefault();
				if (player.Valid)
				{
					switch (args.Button)
					{
					case SharedDisposalUnitComponent.UiButton.Eject:
					{
						this.TryEjectContents(uid, component);
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.Action;
						LogImpact impact = LogImpact.Low;
						LogStringHandler logStringHandler = new LogStringHandler(21, 2);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "player", "ToPrettyString(player)");
						logStringHandler.AppendLiteral(" hit eject button on ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
						adminLogger.Add(type, impact, ref logStringHandler);
						return;
					}
					case SharedDisposalUnitComponent.UiButton.Engage:
					{
						this.ToggleEngage(uid, component);
						ISharedAdminLogManager adminLogger2 = this._adminLogger;
						LogType type2 = LogType.Action;
						LogImpact impact2 = LogImpact.Low;
						LogStringHandler logStringHandler = new LogStringHandler(32, 3);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "player", "ToPrettyString(player)");
						logStringHandler.AppendLiteral(" hit flush button on ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
						logStringHandler.AppendLiteral(", it's now ");
						logStringHandler.AppendFormatted(component.Engaged ? "on" : "off");
						adminLogger2.Add(type2, impact2, ref logStringHandler);
						return;
					}
					case SharedDisposalUnitComponent.UiButton.Power:
						this._power.TogglePower(uid, true, null, args.Session.AttachedEntity);
						return;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		// Token: 0x06001C84 RID: 7300 RVA: 0x000985E1 File Offset: 0x000967E1
		public void ToggleEngage(EntityUid uid, DisposalUnitComponent component)
		{
			component.Engaged = !component.Engaged;
			if (component.Engaged)
			{
				this.Engage(uid, component);
				return;
			}
			this.Disengage(uid, component);
		}

		// Token: 0x06001C85 RID: 7301 RVA: 0x0009860C File Offset: 0x0009680C
		private void HandleActivate(EntityUid uid, DisposalUnitComponent component, ActivateInWorldEvent args)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			args.Handled = true;
			this._ui.TryOpen(uid, SharedDisposalUnitComponent.DisposalUnitUiKey.Key, actor.PlayerSession, null);
		}

		// Token: 0x06001C86 RID: 7302 RVA: 0x0009864C File Offset: 0x0009684C
		private void HandleAfterInteractUsing(EntityUid uid, DisposalUnitComponent component, AfterInteractUsingEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			if (!base.HasComp<HandsComponent>(args.User))
			{
				return;
			}
			if (!this.CanInsert(component, args.Used) || !this._handsSystem.TryDropIntoContainer(args.User, args.Used, component.Container, true, null))
			{
				return;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(16, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" inserted ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Used), "ToPrettyString(args.Used)");
			logStringHandler.AppendLiteral(" into ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.AfterInsert(uid, component, args.Used, new EntityUid?(args.User));
			args.Handled = true;
		}

		// Token: 0x06001C87 RID: 7303 RVA: 0x00098744 File Offset: 0x00096944
		private void HandleThrowCollide(EntityUid uid, DisposalUnitComponent component, ThrowHitByEvent args)
		{
			if (!this.CanInsert(component, args.Thrown) || this._robustRandom.NextDouble() > 0.75 || !component.Container.Insert(args.Thrown, null, null, null, null, null))
			{
				this._popupSystem.PopupEntity(Loc.GetString("disposal-unit-thrown-missed"), uid, PopupType.Small);
				return;
			}
			if (args.User != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Landed;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(22, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Thrown), "ToPrettyString(args.Thrown)");
				logStringHandler.AppendLiteral(" thrown by ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User.Value), "player", "ToPrettyString(args.User.Value)");
				logStringHandler.AppendLiteral(" landed in ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this.AfterInsert(uid, component, args.Thrown, null);
		}

		// Token: 0x06001C88 RID: 7304 RVA: 0x00098854 File Offset: 0x00096A54
		private void HandleDisposalInit(EntityUid uid, DisposalUnitComponent component, ComponentInit args)
		{
			component.Container = this._containerSystem.EnsureContainer<Container>(uid, "DisposalUnit", null);
			this.UpdateInterface(uid, component, component.Powered);
			if (!base.HasComp<AnchorableComponent>(uid))
			{
				string text = "VitalComponentMissing";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Disposal unit ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" is missing an ");
				defaultInterpolatedStringHandler.AppendFormatted("AnchorableComponent");
				Logger.WarningS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x000988D8 File Offset: 0x00096AD8
		private void HandleDisposalRemove(EntityUid uid, DisposalUnitComponent component, ComponentRemove args)
		{
			foreach (EntityUid entity in component.Container.ContainedEntities.ToArray<EntityUid>())
			{
				component.Container.Remove(entity, null, null, null, true, true, null, null);
			}
			this._ui.TryCloseAll(uid, SharedDisposalUnitComponent.DisposalUnitUiKey.Key, null);
			CancellationTokenSource automaticEngageToken = component.AutomaticEngageToken;
			if (automaticEngageToken != null)
			{
				automaticEngageToken.Cancel();
			}
			component.AutomaticEngageToken = null;
			component.Container = null;
			base.RemComp<ActiveDisposalUnitComponent>(uid);
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x0009896C File Offset: 0x00096B6C
		private void HandlePowerChange(EntityUid uid, DisposalUnitComponent component, ref PowerChangedEvent args)
		{
			if (!component.Running)
			{
				return;
			}
			component.Powered = args.Powered;
			if (!args.Powered)
			{
				CancellationTokenSource automaticEngageToken = component.AutomaticEngageToken;
				if (automaticEngageToken != null)
				{
					automaticEngageToken.Cancel();
				}
				component.AutomaticEngageToken = null;
			}
			this.HandleStateChange(uid, component, args.Powered && component.State == SharedDisposalUnitComponent.PressureState.Pressurizing);
			this.UpdateVisualState(uid, component);
			this.UpdateInterface(uid, component, args.Powered);
			if (component.Engaged && !this.TryFlush(uid, component))
			{
				this.TryQueueEngage(uid, component);
			}
		}

		// Token: 0x06001C8B RID: 7307 RVA: 0x000989FA File Offset: 0x00096BFA
		public void HandleStateChange(EntityUid uid, DisposalUnitComponent component, bool active)
		{
			if (active)
			{
				base.EnsureComp<ActiveDisposalUnitComponent>(uid);
				return;
			}
			base.RemComp<ActiveDisposalUnitComponent>(uid);
		}

		// Token: 0x06001C8C RID: 7308 RVA: 0x00098A10 File Offset: 0x00096C10
		private void HandleMovement(EntityUid uid, DisposalUnitComponent component, ref ContainerRelayMovementEntityEvent args)
		{
			TimeSpan currentTime = this.GameTiming.CurTime;
			HandsComponent hands;
			if (!base.TryComp<HandsComponent>(args.Entity, ref hands) || hands.Count == 0 || currentTime < component.LastExitAttempt + SharedDisposalUnitSystem.ExitAttemptDelay)
			{
				return;
			}
			component.LastExitAttempt = currentTime;
			this.Remove(uid, component, args.Entity);
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x00098A6F File Offset: 0x00096C6F
		private void OnAnchorChanged(EntityUid uid, DisposalUnitComponent component, ref AnchorStateChangedEvent args)
		{
			if (base.Terminating(uid, null))
			{
				return;
			}
			this.UpdateVisualState(uid, component);
			if (!args.Anchored)
			{
				this.TryEjectContents(uid, component);
			}
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x00098A94 File Offset: 0x00096C94
		private void HandleDestruction(EntityUid uid, DisposalUnitComponent component, DestructionEventArgs args)
		{
			this.TryEjectContents(uid, component);
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x00098A9E File Offset: 0x00096C9E
		private void HandleDragDropOn(EntityUid uid, DisposalUnitComponent component, ref DragDropTargetEvent args)
		{
			args.Handled = this.TryInsert(uid, args.Dragged, new EntityUid?(args.User), null);
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x00098AC0 File Offset: 0x00096CC0
		private bool Update(EntityUid uid, DisposalUnitComponent component, float frameTime)
		{
			float oldPressure = component.Pressure;
			component.Pressure = MathF.Min(1f, component.Pressure + 0.05f * frameTime);
			component.State = ((component.Pressure >= 1f) ? SharedDisposalUnitComponent.PressureState.Ready : SharedDisposalUnitComponent.PressureState.Pressurizing);
			SharedDisposalUnitComponent.PressureState state = component.State;
			if (oldPressure < 1f && state == SharedDisposalUnitComponent.PressureState.Ready)
			{
				this.UpdateVisualState(uid, component);
				this.UpdateInterface(uid, component, component.Powered);
				if (component.Engaged)
				{
					this.TryFlush(uid, component);
					state = component.State;
				}
			}
			if (component.State == SharedDisposalUnitComponent.PressureState.Pressurizing)
			{
				float oldTimeElapsed = oldPressure / 0.05f;
				if (oldTimeElapsed < component.FlushTime && oldTimeElapsed + frameTime >= component.FlushTime)
				{
					this.UpdateVisualState(uid, component);
				}
			}
			Box2? disposalsBounds = null;
			int count = component.RecentlyEjected.Count;
			if (count > 0)
			{
				PhysicsComponent disposalsBody;
				if (!base.TryComp<PhysicsComponent>(uid, ref disposalsBody))
				{
					component.RecentlyEjected.Clear();
				}
				else
				{
					disposalsBounds = new Box2?(this._lookup.GetWorldAABB(uid, null));
				}
			}
			for (int i = component.RecentlyEjected.Count - 1; i >= 0; i--)
			{
				EntityUid ejectedId = component.RecentlyEjected[i];
				PhysicsComponent body;
				if (base.Exists(ejectedId) && base.TryComp<PhysicsComponent>(ejectedId, ref body))
				{
					if (!base.HasComp<ItemComponent>(ejectedId))
					{
						Box2 worldAABB = this._lookup.GetWorldAABB(ejectedId, null);
						Box2 value = disposalsBounds.Value;
						if (worldAABB.Intersects(ref value))
						{
							goto IL_161;
						}
					}
					component.RecentlyEjected.RemoveAt(i);
				}
				IL_161:;
			}
			if (count != component.RecentlyEjected.Count)
			{
				base.Dirty(component, null);
			}
			return state == SharedDisposalUnitComponent.PressureState.Ready && component.RecentlyEjected.Count == 0;
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x00098C64 File Offset: 0x00096E64
		[NullableContext(2)]
		public bool TryInsert(EntityUid unitId, EntityUid toInsertId, EntityUid? userId, DisposalUnitComponent unit = null)
		{
			if (!base.Resolve<DisposalUnitComponent>(unitId, ref unit, true))
			{
				return false;
			}
			if (userId != null && !base.HasComp<SharedHandsComponent>(userId) && toInsertId != userId)
			{
				this._popupSystem.PopupEntity(Loc.GetString("disposal-unit-no-hands"), userId.Value, userId.Value, PopupType.SmallCaution);
				return false;
			}
			if (!this.CanInsert(unit, toInsertId))
			{
				return false;
			}
			EntityUid? target = userId;
			float delay = (target != null && (target == null || target.GetValueOrDefault() == toInsertId)) ? unit.EntryDelay : unit.DraggedEntryDelay;
			if (delay <= 0f || userId == null)
			{
				this.AfterInsert(unitId, unit, toInsertId, userId);
				return true;
			}
			EntityUid value = userId.Value;
			float delay2 = delay;
			target = new EntityUid?(toInsertId);
			EntityUid? used = new EntityUid?(unitId);
			DoAfterEventArgs doAfterArgs = new DoAfterEventArgs(value, delay2, default(CancellationToken), target, used)
			{
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				NeedHand = false
			};
			this._doAfterSystem.DoAfter(doAfterArgs);
			return true;
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x00098D9C File Offset: 0x00096F9C
		public bool TryFlush(EntityUid uid, DisposalUnitComponent component)
		{
			if (component.Deleted || !this.CanFlush(uid, component))
			{
				return false;
			}
			BeforeDisposalFlushEvent beforeFlushArgs = new BeforeDisposalFlushEvent();
			base.RaiseLocalEvent<BeforeDisposalFlushEvent>(uid, beforeFlushArgs, false);
			if (beforeFlushArgs.Cancelled)
			{
				this.Disengage(uid, component);
				return false;
			}
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (!base.TryComp<MapGridComponent>(xform.GridUid, ref grid))
			{
				return false;
			}
			EntityCoordinates coords = xform.Coordinates;
			EntityUid entry = grid.GetLocal(coords).FirstOrDefault((EntityUid entity) => base.HasComp<DisposalEntryComponent>(entity));
			if (entry == default(EntityUid))
			{
				return false;
			}
			GasMixture air = component.Air;
			DisposalEntryComponent disposalEntryComponent = base.Comp<DisposalEntryComponent>(entry);
			Vector2i indices = this._transformSystem.GetGridOrMapTilePosition(uid, xform);
			GasMixture environment = this._atmosSystem.GetTileMixture(xform.GridUid, xform.MapUid, indices, true);
			if (environment != null && environment.Temperature > 0f)
			{
				float transferMoles = 0.1f * (25.584562f - air.Pressure) * air.Volume / (environment.Temperature * 8.314463f);
				component.Air = environment.Remove(transferMoles);
			}
			disposalEntryComponent.TryInsert(component, beforeFlushArgs.Tags);
			CancellationTokenSource automaticEngageToken = component.AutomaticEngageToken;
			if (automaticEngageToken != null)
			{
				automaticEngageToken.Cancel();
			}
			component.AutomaticEngageToken = null;
			component.Pressure = 0f;
			component.State = SharedDisposalUnitComponent.PressureState.Pressurizing;
			component.Engaged = false;
			this.HandleStateChange(uid, component, true);
			this.UpdateVisualState(uid, component, true);
			this.UpdateInterface(uid, component, component.Powered);
			return true;
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x00098F14 File Offset: 0x00097114
		public void UpdateInterface(EntityUid uid, DisposalUnitComponent component, bool powered)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
			defaultInterpolatedStringHandler.AppendLiteral("disposal-unit-state-");
			defaultInterpolatedStringHandler.AppendFormatted<SharedDisposalUnitComponent.PressureState>(component.State);
			string stateString = Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear());
			SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState state = new SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState(base.Name(uid, null), stateString, this.EstimatedFullPressure(component), powered, component.Engaged);
			this._ui.TrySetUiState(uid, SharedDisposalUnitComponent.DisposalUnitUiKey.Key, state, null, null, true);
			DisposalUnitUIStateUpdatedEvent stateUpdatedEvent = new DisposalUnitUIStateUpdatedEvent(state);
			base.RaiseLocalEvent<DisposalUnitUIStateUpdatedEvent>(uid, stateUpdatedEvent, false);
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x00098F98 File Offset: 0x00097198
		private TimeSpan EstimatedFullPressure(DisposalUnitComponent component)
		{
			if (component.State == SharedDisposalUnitComponent.PressureState.Ready)
			{
				return TimeSpan.Zero;
			}
			TimeSpan currentTime = this.GameTiming.CurTime;
			float pressure = component.Pressure;
			return TimeSpan.FromSeconds(currentTime.TotalSeconds + (double)((1f - pressure) / 0.05f));
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x00098FE1 File Offset: 0x000971E1
		public void UpdateVisualState(EntityUid uid, DisposalUnitComponent component)
		{
			this.UpdateVisualState(uid, component, false);
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x00098FEC File Offset: 0x000971EC
		public void UpdateVisualState(EntityUid uid, DisposalUnitComponent component, bool flush)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			if (!base.Comp<TransformComponent>(uid).Anchored)
			{
				this._appearance.SetData(uid, SharedDisposalUnitComponent.Visuals.VisualState, SharedDisposalUnitComponent.VisualState.UnAnchored, appearance);
				this._appearance.SetData(uid, SharedDisposalUnitComponent.Visuals.Handle, SharedDisposalUnitComponent.HandleState.Normal, appearance);
				this._appearance.SetData(uid, SharedDisposalUnitComponent.Visuals.Light, SharedDisposalUnitComponent.LightStates.Off, appearance);
				return;
			}
			this._appearance.SetData(uid, SharedDisposalUnitComponent.Visuals.VisualState, (component.Pressure < 1f) ? SharedDisposalUnitComponent.VisualState.Charging : SharedDisposalUnitComponent.VisualState.Anchored, appearance);
			this._appearance.SetData(uid, SharedDisposalUnitComponent.Visuals.Handle, component.Engaged ? SharedDisposalUnitComponent.HandleState.Engaged : SharedDisposalUnitComponent.HandleState.Normal, appearance);
			if (!component.Powered)
			{
				this._appearance.SetData(uid, SharedDisposalUnitComponent.Visuals.Light, SharedDisposalUnitComponent.LightStates.Off, appearance);
				return;
			}
			SharedDisposalUnitComponent.LightStates lightState = SharedDisposalUnitComponent.LightStates.Off;
			if (flush)
			{
				this._appearance.SetData(uid, SharedDisposalUnitComponent.Visuals.VisualState, SharedDisposalUnitComponent.VisualState.Flushing, appearance);
			}
			if (component.Container.ContainedEntities.Count > 0)
			{
				lightState |= SharedDisposalUnitComponent.LightStates.Full;
			}
			if (component.Pressure < 1f)
			{
				lightState |= SharedDisposalUnitComponent.LightStates.Charging;
			}
			else if (!component.Engaged)
			{
				lightState |= SharedDisposalUnitComponent.LightStates.Ready;
			}
			this._appearance.SetData(uid, SharedDisposalUnitComponent.Visuals.Light, lightState, appearance);
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x0009913C File Offset: 0x0009733C
		public void Remove(EntityUid uid, DisposalUnitComponent component, EntityUid toRemove)
		{
			component.Container.Remove(toRemove, null, null, null, true, false, null, null);
			if (component.Container.ContainedEntities.Count == 0)
			{
				CancellationTokenSource automaticEngageToken = component.AutomaticEngageToken;
				if (automaticEngageToken != null)
				{
					automaticEngageToken.Cancel();
				}
				component.AutomaticEngageToken = null;
			}
			if (!component.RecentlyEjected.Contains(toRemove))
			{
				component.RecentlyEjected.Add(toRemove);
			}
			base.Dirty(component, null);
			this.HandleStateChange(uid, component, true);
			this.UpdateVisualState(uid, component);
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x000991CA File Offset: 0x000973CA
		public bool CanFlush(EntityUid unit, DisposalUnitComponent component)
		{
			return component.State == SharedDisposalUnitComponent.PressureState.Ready && component.Powered && base.Comp<TransformComponent>(unit).Anchored;
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x000991EC File Offset: 0x000973EC
		public void Engage(EntityUid uid, DisposalUnitComponent component)
		{
			component.Engaged = true;
			this.UpdateVisualState(uid, component);
			this.UpdateInterface(uid, component, component.Powered);
			if (this.CanFlush(uid, component))
			{
				TimerExtensions.SpawnTimer(uid, component.FlushDelay, delegate()
				{
					this.TryFlush(uid, component);
				}, default(CancellationToken));
			}
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x0009928E File Offset: 0x0009748E
		public void Disengage(EntityUid uid, DisposalUnitComponent component)
		{
			component.Engaged = false;
			this.UpdateVisualState(uid, component);
			this.UpdateInterface(uid, component, component.Powered);
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x000992B0 File Offset: 0x000974B0
		public void TryEjectContents(EntityUid uid, DisposalUnitComponent component)
		{
			foreach (EntityUid entity in component.Container.ContainedEntities.ToArray<EntityUid>())
			{
				this.Remove(uid, component, entity);
			}
		}

		// Token: 0x06001C9C RID: 7324 RVA: 0x000992F0 File Offset: 0x000974F0
		public override bool CanInsert(SharedDisposalUnitComponent component, EntityUid entity)
		{
			if (base.CanInsert(component, entity))
			{
				DisposalUnitComponent serverComp = component as DisposalUnitComponent;
				if (serverComp != null)
				{
					return serverComp.Container.CanInsert(entity, null);
				}
			}
			return false;
		}

		// Token: 0x06001C9D RID: 7325 RVA: 0x00099320 File Offset: 0x00097520
		public void TryQueueEngage(EntityUid uid, DisposalUnitComponent component)
		{
			if (component.Deleted || !component.AutomaticEngage || (!component.Powered && component.Container.ContainedEntities.Count == 0))
			{
				return;
			}
			component.AutomaticEngageToken = new CancellationTokenSource();
			TimerExtensions.SpawnTimer(uid, component.AutomaticEngageTime, delegate()
			{
				if (!this.TryFlush(uid, component))
				{
					this.TryQueueEngage(uid, component);
				}
			}, component.AutomaticEngageToken.Token);
		}

		// Token: 0x06001C9E RID: 7326 RVA: 0x000993CC File Offset: 0x000975CC
		public void AfterInsert(EntityUid uid, DisposalUnitComponent component, EntityUid inserted, EntityUid? user = null)
		{
			if (!component.Container.Insert(inserted, null, null, null, null, null))
			{
				return;
			}
			EntityUid? entityUid = user;
			if ((entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != inserted)) && user != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(16, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "player", "ToPrettyString(user.Value)");
				logStringHandler.AppendLiteral(" inserted ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(inserted), "ToPrettyString(inserted)");
				logStringHandler.AppendLiteral(" into ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this.TryQueueEngage(uid, component);
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(inserted, ref actor))
			{
				this._ui.TryClose(uid, SharedDisposalUnitComponent.DisposalUnitUiKey.Key, actor.PlayerSession, null);
			}
			this.UpdateVisualState(uid, component);
		}

		// Token: 0x0400124A RID: 4682
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x0400124B RID: 4683
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400124C RID: 4684
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x0400124D RID: 4685
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x0400124E RID: 4686
		[Dependency]
		private readonly AtmosphereSystem _atmosSystem;

		// Token: 0x0400124F RID: 4687
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04001250 RID: 4688
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04001251 RID: 4689
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001252 RID: 4690
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04001253 RID: 4691
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04001254 RID: 4692
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x04001255 RID: 4693
		[Dependency]
		private readonly UserInterfaceSystem _ui;

		// Token: 0x04001256 RID: 4694
		[Dependency]
		private readonly PowerReceiverSystem _power;
	}
}
