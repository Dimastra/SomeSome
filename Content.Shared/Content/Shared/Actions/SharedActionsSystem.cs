using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Actions
{
	// Token: 0x02000763 RID: 1891
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedActionsSystem : EntitySystem
	{
		// Token: 0x06001730 RID: 5936 RVA: 0x0004AEF4 File Offset: 0x000490F4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ActionsComponent, DidEquipEvent>(new ComponentEventHandler<ActionsComponent, DidEquipEvent>(this.OnDidEquip), null, null);
			base.SubscribeLocalEvent<ActionsComponent, DidEquipHandEvent>(new ComponentEventHandler<ActionsComponent, DidEquipHandEvent>(this.OnHandEquipped), null, null);
			base.SubscribeLocalEvent<ActionsComponent, DidUnequipEvent>(new ComponentEventHandler<ActionsComponent, DidUnequipEvent>(this.OnDidUnequip), null, null);
			base.SubscribeLocalEvent<ActionsComponent, DidUnequipHandEvent>(new ComponentEventHandler<ActionsComponent, DidUnequipHandEvent>(this.OnHandUnequipped), null, null);
			base.SubscribeLocalEvent<ActionsComponent, ComponentGetState>(new ComponentEventRefHandler<ActionsComponent, ComponentGetState>(this.GetState), null, null);
			base.SubscribeAllEvent<RequestPerformActionEvent>(new EntitySessionEventHandler<RequestPerformActionEvent>(this.OnActionRequest), null, null);
		}

		// Token: 0x06001731 RID: 5937 RVA: 0x0004AF80 File Offset: 0x00049180
		public virtual void Dirty(ActionType action)
		{
			if (action.AttachedEntity == null)
			{
				return;
			}
			ActionsComponent comp;
			if (!base.TryComp<ActionsComponent>(action.AttachedEntity, ref comp))
			{
				action.AttachedEntity = null;
				return;
			}
			base.Dirty(comp, null);
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x0004AFC0 File Offset: 0x000491C0
		public void SetToggled(ActionType action, bool toggled)
		{
			if (action.Toggled == toggled)
			{
				return;
			}
			action.Toggled = toggled;
			this.Dirty(action);
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x0004AFDA File Offset: 0x000491DA
		public void SetEnabled(ActionType action, bool enabled)
		{
			if (action.Enabled == enabled)
			{
				return;
			}
			action.Enabled = enabled;
			this.Dirty(action);
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x0004AFF4 File Offset: 0x000491F4
		public void SetCharges(ActionType action, int? charges)
		{
			int? charges2 = action.Charges;
			int? num = charges;
			if (charges2.GetValueOrDefault() == num.GetValueOrDefault() & charges2 != null == (num != null))
			{
				return;
			}
			action.Charges = charges;
			this.Dirty(action);
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x0004B03C File Offset: 0x0004923C
		private void GetState(EntityUid uid, ActionsComponent component, ref ComponentGetState args)
		{
			args.State = new ActionsComponentState(component.Actions.ToList<ActionType>());
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x0004B054 File Offset: 0x00049254
		private void OnActionRequest(RequestPerformActionEvent ev, EntitySessionEventArgs args)
		{
			EntityUid? entityUid = args.SenderSession.AttachedEntity;
			if (entityUid == null)
			{
				return;
			}
			EntityUid user = entityUid.GetValueOrDefault();
			ActionsComponent component;
			if (!base.TryComp<ActionsComponent>(user, ref component))
			{
				return;
			}
			ActionType act;
			if (!component.Actions.TryGetValue(ev.Action, out act))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogStringHandler logStringHandler = new LogStringHandler(56, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" attempted to perform an action that they do not have: ");
				logStringHandler.AppendFormatted(ev.Action.DisplayName);
				logStringHandler.AppendLiteral(".");
				adminLogger.Add(type, ref logStringHandler);
				return;
			}
			if (!act.Enabled)
			{
				return;
			}
			TimeSpan curTime = this.GameTiming.CurTime;
			if (act.Cooldown != null && act.Cooldown.Value.Item2 > curTime)
			{
				return;
			}
			BaseActionEvent performEvent = null;
			string name = Loc.GetString(act.DisplayName);
			EntityTargetAction entityAction = act as EntityTargetAction;
			if (entityAction != null)
			{
				entityUid = ev.EntityTarget;
				if (entityUid != null)
				{
					EntityUid entityTarget = entityUid.GetValueOrDefault();
					if (entityTarget.Valid)
					{
						this._rotateToFaceSystem.TryFaceCoordinates(user, base.Transform(entityTarget).WorldPosition, null);
						if (!this.ValidateEntityTarget(user, entityTarget, entityAction))
						{
							return;
						}
						if (act.Provider == null)
						{
							ISharedAdminLogManager adminLogger2 = this._adminLogger;
							LogType type2 = LogType.Action;
							LogStringHandler logStringHandler = new LogStringHandler(40, 3);
							logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
							logStringHandler.AppendLiteral(" is performing the ");
							logStringHandler.AppendFormatted(name, 0, "action");
							logStringHandler.AppendLiteral(" action targeted at ");
							logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entityTarget), "target", "ToPrettyString(entityTarget)");
							logStringHandler.AppendLiteral(".");
							adminLogger2.Add(type2, ref logStringHandler);
						}
						else
						{
							ISharedAdminLogManager adminLogger3 = this._adminLogger;
							LogType type3 = LogType.Action;
							LogStringHandler logStringHandler = new LogStringHandler(55, 4);
							logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
							logStringHandler.AppendLiteral(" is performing the ");
							logStringHandler.AppendFormatted(name, 0, "action");
							logStringHandler.AppendLiteral(" action (provided by ");
							logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(act.Provider.Value), "provider", "ToPrettyString(act.Provider.Value)");
							logStringHandler.AppendLiteral(") targeted at ");
							logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entityTarget), "target", "ToPrettyString(entityTarget)");
							logStringHandler.AppendLiteral(".");
							adminLogger3.Add(type3, ref logStringHandler);
						}
						if (entityAction.Event != null)
						{
							entityAction.Event.Target = entityTarget;
							performEvent = entityAction.Event;
							goto IL_5A7;
						}
						goto IL_5A7;
					}
				}
				Logger.Error("Attempted to perform an entity-targeted action without a target! Action: " + entityAction.DisplayName);
				return;
			}
			WorldTargetAction worldAction = act as WorldTargetAction;
			if (worldAction == null)
			{
				InstantAction instantAction = act as InstantAction;
				if (instantAction != null)
				{
					if (act.CheckCanInteract && !this._actionBlockerSystem.CanInteract(user, null))
					{
						return;
					}
					if (act.Provider == null)
					{
						ISharedAdminLogManager adminLogger4 = this._adminLogger;
						LogType type4 = LogType.Action;
						LogStringHandler logStringHandler = new LogStringHandler(27, 2);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
						logStringHandler.AppendLiteral(" is performing the ");
						logStringHandler.AppendFormatted(name, 0, "action");
						logStringHandler.AppendLiteral(" action.");
						adminLogger4.Add(type4, ref logStringHandler);
					}
					else
					{
						ISharedAdminLogManager adminLogger5 = this._adminLogger;
						LogType type5 = LogType.Action;
						LogStringHandler logStringHandler = new LogStringHandler(40, 3);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
						logStringHandler.AppendLiteral(" is performing the ");
						logStringHandler.AppendFormatted(name, 0, "action");
						logStringHandler.AppendLiteral(" action provided by ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(act.Provider.Value), "provider", "ToPrettyString(act.Provider.Value)");
						logStringHandler.AppendLiteral(".");
						adminLogger5.Add(type5, ref logStringHandler);
					}
					performEvent = instantAction.Event;
				}
			}
			else
			{
				EntityCoordinates? entityCoordinatesTarget2 = ev.EntityCoordinatesTarget;
				if (entityCoordinatesTarget2 == null)
				{
					Logger.Error("Attempted to perform a world-targeted action without a target! Action: " + worldAction.DisplayName);
					return;
				}
				EntityCoordinates entityCoordinatesTarget = entityCoordinatesTarget2.GetValueOrDefault();
				this._rotateToFaceSystem.TryFaceCoordinates(user, entityCoordinatesTarget.Position, null);
				if (!this.ValidateWorldTarget(user, entityCoordinatesTarget, worldAction))
				{
					return;
				}
				if (act.Provider == null)
				{
					ISharedAdminLogManager adminLogger6 = this._adminLogger;
					LogType type6 = LogType.Action;
					LogStringHandler logStringHandler = new LogStringHandler(40, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
					logStringHandler.AppendLiteral(" is performing the ");
					logStringHandler.AppendFormatted(name, 0, "action");
					logStringHandler.AppendLiteral(" action targeted at ");
					logStringHandler.AppendFormatted<EntityCoordinates>(entityCoordinatesTarget, "target", "entityCoordinatesTarget");
					logStringHandler.AppendLiteral(".");
					adminLogger6.Add(type6, ref logStringHandler);
				}
				else
				{
					ISharedAdminLogManager adminLogger7 = this._adminLogger;
					LogType type7 = LogType.Action;
					LogStringHandler logStringHandler = new LogStringHandler(55, 4);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
					logStringHandler.AppendLiteral(" is performing the ");
					logStringHandler.AppendFormatted(name, 0, "action");
					logStringHandler.AppendLiteral(" action (provided by ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(act.Provider.Value), "provider", "ToPrettyString(act.Provider.Value)");
					logStringHandler.AppendLiteral(") targeted at ");
					logStringHandler.AppendFormatted<EntityCoordinates>(entityCoordinatesTarget, "target", "entityCoordinatesTarget");
					logStringHandler.AppendLiteral(".");
					adminLogger7.Add(type7, ref logStringHandler);
				}
				if (worldAction.Event != null)
				{
					worldAction.Event.Target = entityCoordinatesTarget;
					performEvent = worldAction.Event;
				}
			}
			IL_5A7:
			if (performEvent != null)
			{
				performEvent.Performer = user;
			}
			this.PerformAction(user, component, act, performEvent, curTime, true);
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x0004B624 File Offset: 0x00049824
		public bool ValidateEntityTarget(EntityUid user, EntityUid target, EntityTargetAction action)
		{
			if (!target.IsValid() || base.Deleted(target, null))
			{
				return false;
			}
			if (action.Whitelist != null && !action.Whitelist.IsValid(target, this.EntityManager))
			{
				return false;
			}
			if (action.CheckCanInteract && !this._actionBlockerSystem.CanInteract(user, new EntityUid?(target)))
			{
				return false;
			}
			if (user == target)
			{
				return action.CanTargetSelf;
			}
			if (!action.CheckCanAccess)
			{
				TransformComponent xform = base.Transform(user);
				TransformComponent targetXform = base.Transform(target);
				return !(xform.MapID != targetXform.MapID) && (action.Range <= 0f || (xform.WorldPosition - targetXform.WorldPosition).Length <= action.Range);
			}
			return (this._interactionSystem.InRangeUnobstructed(user, target, action.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && this._containerSystem.IsInSameOrParentContainer(user, target)) || this._interactionSystem.CanAccessViaStorage(user, target);
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x0004B730 File Offset: 0x00049930
		public bool ValidateWorldTarget(EntityUid user, EntityCoordinates coords, WorldTargetAction action)
		{
			if (action.CheckCanInteract && !this._actionBlockerSystem.CanInteract(user, null))
			{
				return false;
			}
			if (!action.CheckCanAccess)
			{
				return !(base.Transform(user).MapID != coords.GetMapId(this.EntityManager)) && (action.Range <= 0f || coords.InRange(this.EntityManager, base.Transform(user).Coordinates, action.Range));
			}
			return this._interactionSystem.InRangeUnobstructed(user, coords, action.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x0004B7D4 File Offset: 0x000499D4
		[NullableContext(2)]
		public void PerformAction(EntityUid performer, ActionsComponent component, [Nullable(1)] ActionType action, BaseActionEvent actionEvent, TimeSpan curTime, bool predicted = true)
		{
			bool handled = false;
			bool toggledBefore = action.Toggled;
			if (actionEvent != null)
			{
				actionEvent.Handled = false;
				if (action.Provider == null)
				{
					base.RaiseLocalEvent(performer, actionEvent, true);
				}
				else
				{
					base.RaiseLocalEvent(action.Provider.Value, actionEvent, true);
				}
				handled = actionEvent.Handled;
			}
			if (!(handled | this.PerformBasicActions(performer, action, predicted)))
			{
				return;
			}
			bool dirty = toggledBefore == action.Toggled;
			if (action.Charges != null)
			{
				dirty = true;
				action.Charges--;
				int? charges = action.Charges;
				int num = 0;
				if (charges.GetValueOrDefault() == num & charges != null)
				{
					action.Enabled = false;
				}
			}
			action.Cooldown = null;
			if (action.UseDelay != null)
			{
				dirty = true;
				action.Cooldown = new ValueTuple<TimeSpan, TimeSpan>?(new ValueTuple<TimeSpan, TimeSpan>(curTime, curTime + action.UseDelay.Value));
			}
			if (dirty && component != null)
			{
				base.Dirty(component, null);
			}
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x0004B8F8 File Offset: 0x00049AF8
		protected virtual bool PerformBasicActions(EntityUid performer, ActionType action, bool predicted)
		{
			if (action.Sound == null && string.IsNullOrWhiteSpace(action.Popup))
			{
				return false;
			}
			Filter filter = predicted ? Filter.PvsExcept(performer, 2f, null) : Filter.Pvs(performer, 2f, null, null, null);
			this._audio.Play(action.Sound, filter, performer, true, action.AudioParams);
			if (string.IsNullOrWhiteSpace(action.Popup))
			{
				return true;
			}
			string msg = (!action.Toggled || string.IsNullOrWhiteSpace(action.PopupToggleSuffix)) ? Loc.GetString(action.Popup) : Loc.GetString(action.Popup + action.PopupToggleSuffix);
			this._popupSystem.PopupEntity(msg, performer, filter, true, PopupType.Small);
			return true;
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x0004B9B0 File Offset: 0x00049BB0
		public virtual void AddAction(EntityUid uid, ActionType action, EntityUid? provider, [Nullable(2)] ActionsComponent comp = null, bool dirty = true)
		{
			if (action is IPrototype)
			{
				Logger.Error("Attempted to directly add a prototype action. You need to clone a prototype in order to use it.");
				return;
			}
			if (comp == null)
			{
				comp = base.EnsureComp<ActionsComponent>(uid);
			}
			action.Provider = provider;
			action.AttachedEntity = new EntityUid?(comp.Owner);
			this.AddActionInternal(comp, action);
			if (dirty)
			{
				base.Dirty(comp, null);
			}
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x0004BA0C File Offset: 0x00049C0C
		protected virtual void AddActionInternal(ActionsComponent comp, ActionType action)
		{
			comp.Actions.Add(action);
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x0004BA1C File Offset: 0x00049C1C
		public void AddActions(EntityUid uid, IEnumerable<ActionType> actions, EntityUid? provider, [Nullable(2)] ActionsComponent comp = null, bool dirty = true)
		{
			if (comp == null)
			{
				comp = base.EnsureComp<ActionsComponent>(uid);
			}
			foreach (ActionType action in actions)
			{
				this.AddAction(uid, action, provider, comp, false);
			}
			if (dirty)
			{
				base.Dirty(comp, null);
			}
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x0004BA84 File Offset: 0x00049C84
		[NullableContext(2)]
		public void RemoveProvidedActions(EntityUid uid, EntityUid provider, ActionsComponent comp = null)
		{
			if (!base.Resolve<ActionsComponent>(uid, ref comp, false))
			{
				return;
			}
			List<ActionType> provided = comp.Actions.Where(delegate(ActionType act)
			{
				EntityUid? provider2 = act.Provider;
				EntityUid provider3 = provider;
				return provider2 != null && (provider2 == null || provider2.GetValueOrDefault() == provider3);
			}).ToList<ActionType>();
			if (provided.Count > 0)
			{
				this.RemoveActions(uid, provided, comp, true);
			}
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x0004BADC File Offset: 0x00049CDC
		public virtual void RemoveActions(EntityUid uid, IEnumerable<ActionType> actions, [Nullable(2)] ActionsComponent comp = null, bool dirty = true)
		{
			if (!base.Resolve<ActionsComponent>(uid, ref comp, false))
			{
				return;
			}
			foreach (ActionType action in actions)
			{
				comp.Actions.Remove(action);
				action.AttachedEntity = null;
			}
			if (dirty)
			{
				base.Dirty(comp, null);
			}
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x0004BB50 File Offset: 0x00049D50
		public void RemoveAction(EntityUid uid, ActionType action, [Nullable(2)] ActionsComponent comp = null)
		{
			this.RemoveActions(uid, new ActionType[]
			{
				action
			}, comp, true);
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x0004BB68 File Offset: 0x00049D68
		private void OnDidEquip(EntityUid uid, ActionsComponent component, DidEquipEvent args)
		{
			GetItemActionsEvent ev = new GetItemActionsEvent(new SlotFlags?(args.SlotFlags));
			base.RaiseLocalEvent<GetItemActionsEvent>(args.Equipment, ev, false);
			if (ev.Actions.Count == 0)
			{
				return;
			}
			this.AddActions(args.Equipee, ev.Actions, new EntityUid?(args.Equipment), component, true);
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x0004BBC4 File Offset: 0x00049DC4
		private void OnHandEquipped(EntityUid uid, ActionsComponent component, DidEquipHandEvent args)
		{
			GetItemActionsEvent ev = new GetItemActionsEvent(null);
			base.RaiseLocalEvent<GetItemActionsEvent>(args.Equipped, ev, false);
			if (ev.Actions.Count == 0)
			{
				return;
			}
			this.AddActions(args.User, ev.Actions, new EntityUid?(args.Equipped), component, true);
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x0004BC1B File Offset: 0x00049E1B
		private void OnDidUnequip(EntityUid uid, ActionsComponent component, DidUnequipEvent args)
		{
			this.RemoveProvidedActions(uid, args.Equipment, component);
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0004BC2B File Offset: 0x00049E2B
		private void OnHandUnequipped(EntityUid uid, ActionsComponent component, DidUnequipHandEvent args)
		{
			this.RemoveProvidedActions(uid, args.Unequipped, component);
		}

		// Token: 0x0400170F RID: 5903
		[Dependency]
		protected readonly IGameTiming GameTiming;

		// Token: 0x04001710 RID: 5904
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04001711 RID: 5905
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04001712 RID: 5906
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04001713 RID: 5907
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04001714 RID: 5908
		[Dependency]
		private readonly RotateToFaceSystem _rotateToFaceSystem;

		// Token: 0x04001715 RID: 5909
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04001716 RID: 5910
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
