using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.UserInterface;
using Content.Shared.ActionBlocker;
using Content.Shared.Alert;
using Content.Shared.Popups;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Events;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Shuttles.Systems
{
	// Token: 0x020001F8 RID: 504
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShuttleConsoleSystem : SharedShuttleConsoleSystem
	{
		// Token: 0x060009D1 RID: 2513 RVA: 0x00032534 File Offset: 0x00030734
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ShuttleConsoleComponent, ComponentShutdown>(new ComponentEventHandler<ShuttleConsoleComponent, ComponentShutdown>(this.OnConsoleShutdown), null, null);
			base.SubscribeLocalEvent<ShuttleConsoleComponent, PowerChangedEvent>(new ComponentEventRefHandler<ShuttleConsoleComponent, PowerChangedEvent>(this.OnConsolePowerChange), null, null);
			base.SubscribeLocalEvent<ShuttleConsoleComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<ShuttleConsoleComponent, AnchorStateChangedEvent>(this.OnConsoleAnchorChange), null, null);
			base.SubscribeLocalEvent<ShuttleConsoleComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<ShuttleConsoleComponent, ActivatableUIOpenAttemptEvent>(this.OnConsoleUIOpenAttempt), null, null);
			base.SubscribeLocalEvent<ShuttleConsoleComponent, ShuttleConsoleDestinationMessage>(new ComponentEventHandler<ShuttleConsoleComponent, ShuttleConsoleDestinationMessage>(this.OnDestinationMessage), null, null);
			base.SubscribeLocalEvent<ShuttleConsoleComponent, BoundUIClosedEvent>(new ComponentEventHandler<ShuttleConsoleComponent, BoundUIClosedEvent>(this.OnConsoleUIClose), null, null);
			base.SubscribeLocalEvent<DockEvent>(new EntityEventHandler<DockEvent>(this.OnDock), null, null);
			base.SubscribeLocalEvent<UndockEvent>(new EntityEventHandler<UndockEvent>(this.OnUndock), null, null);
			base.SubscribeLocalEvent<PilotComponent, MoveEvent>(new ComponentEventRefHandler<PilotComponent, MoveEvent>(this.HandlePilotMove), null, null);
			base.SubscribeLocalEvent<PilotComponent, ComponentGetState>(new ComponentEventRefHandler<PilotComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x00032610 File Offset: 0x00030810
		private void OnDestinationMessage(EntityUid uid, ShuttleConsoleComponent component, ShuttleConsoleDestinationMessage args)
		{
			FTLDestinationComponent dest;
			if (!base.TryComp<FTLDestinationComponent>(args.Destination, ref dest))
			{
				return;
			}
			if (!dest.Enabled)
			{
				return;
			}
			EntityUid? entity = new EntityUid?(component.Owner);
			ConsoleShuttleEvent getShuttleEv = new ConsoleShuttleEvent
			{
				Console = new EntityUid?(uid)
			};
			base.RaiseLocalEvent<ConsoleShuttleEvent>(entity.Value, ref getShuttleEv, false);
			entity = getShuttleEv.Console;
			if (entity != null)
			{
				EntityWhitelist whitelist = dest.Whitelist;
				if (whitelist == null || whitelist.IsValid(entity.Value, this.EntityManager))
				{
					TransformComponent xform;
					ShuttleComponent shuttle;
					if (!base.TryComp<TransformComponent>(entity, ref xform) || !base.TryComp<ShuttleComponent>(xform.GridUid, ref shuttle))
					{
						return;
					}
					if (base.HasComp<FTLComponent>(xform.GridUid))
					{
						this._popup.PopupCursor(Loc.GetString("shuttle-console-in-ftl"), args.Session, PopupType.Small);
						return;
					}
					string reason;
					if (!this._shuttle.CanFTL(new EntityUid?(shuttle.Owner), out reason, null))
					{
						this._popup.PopupCursor(reason, args.Session, PopupType.Small);
						return;
					}
					this._shuttle.FTLTravel(shuttle, args.Destination, 5.5f, this._shuttle.TransitTime, false);
					return;
				}
			}
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x0003273D File Offset: 0x0003093D
		private void OnDock(DockEvent ev)
		{
			this.RefreshShuttleConsoles();
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x00032745 File Offset: 0x00030945
		private void OnUndock(UndockEvent ev)
		{
			this.RefreshShuttleConsoles();
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x0003274D File Offset: 0x0003094D
		public void RefreshShuttleConsoles(EntityUid uid)
		{
			this.RefreshShuttleConsoles();
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x00032758 File Offset: 0x00030958
		public void RefreshShuttleConsoles()
		{
			List<DockingInterfaceState> docks = this.GetAllDocks();
			foreach (ShuttleConsoleComponent comp in base.EntityQuery<ShuttleConsoleComponent>(true))
			{
				this.UpdateState(comp, docks);
			}
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x000327B0 File Offset: 0x000309B0
		private void OnConsoleUIClose(EntityUid uid, ShuttleConsoleComponent component, BoundUIClosedEvent args)
		{
			if ((ShuttleConsoleUiKey)args.UiKey == ShuttleConsoleUiKey.Key)
			{
				EntityUid? attachedEntity = args.Session.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid user = attachedEntity.GetValueOrDefault();
					foreach (AutoDockComponent autoDockComponent in base.EntityQuery<AutoDockComponent>(true))
					{
						autoDockComponent.Requesters.Remove(user);
					}
					this.RemovePilot(user);
					return;
				}
			}
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x00032838 File Offset: 0x00030A38
		private void OnConsoleUIOpenAttempt(EntityUid uid, ShuttleConsoleComponent component, ActivatableUIOpenAttemptEvent args)
		{
			if (!this.TryPilot(args.User, uid))
			{
				args.Cancel();
			}
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0003284F File Offset: 0x00030A4F
		private void OnConsoleAnchorChange(EntityUid uid, ShuttleConsoleComponent component, ref AnchorStateChangedEvent args)
		{
			this.UpdateState(component, null);
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x00032859 File Offset: 0x00030A59
		private void OnConsolePowerChange(EntityUid uid, ShuttleConsoleComponent component, ref PowerChangedEvent args)
		{
			this.UpdateState(component, null);
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x00032864 File Offset: 0x00030A64
		private bool TryPilot(EntityUid user, EntityUid uid)
		{
			ShuttleConsoleComponent component;
			if (!this._tags.HasTag(user, "CanPilot") || !base.TryComp<ShuttleConsoleComponent>(uid, ref component) || !this.IsPowered(uid, this.EntityManager, null) || !base.Transform(uid).Anchored || !this._blocker.CanInteract(user, new EntityUid?(uid)))
			{
				return false;
			}
			PilotComponent pilotComponent = this.EntityManager.EnsureComponent<PilotComponent>(user);
			SharedShuttleConsoleComponent console = pilotComponent.Console;
			if (console != null)
			{
				this.RemovePilot(pilotComponent);
				if (console == component)
				{
					return false;
				}
			}
			this.AddPilot(user, component);
			return true;
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x000328F0 File Offset: 0x00030AF0
		private void OnGetState(EntityUid uid, PilotComponent component, ref ComponentGetState args)
		{
			SharedShuttleConsoleComponent console = component.Console;
			args.State = new SharedShuttleConsoleSystem.PilotComponentState((console != null) ? new EntityUid?(console.Owner) : null);
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x00032928 File Offset: 0x00030B28
		private List<DockingInterfaceState> GetAllDocks()
		{
			List<DockingInterfaceState> result = new List<DockingInterfaceState>();
			foreach (ValueTuple<DockingComponent, TransformComponent> valueTuple in base.EntityQuery<DockingComponent, TransformComponent>(true))
			{
				DockingComponent comp = valueTuple.Item1;
				TransformComponent xform = valueTuple.Item2;
				if (!(xform.ParentUid != xform.GridUid))
				{
					DockingInterfaceState state = new DockingInterfaceState
					{
						Coordinates = xform.Coordinates,
						Angle = xform.LocalRotation,
						Entity = comp.Owner,
						Connected = comp.Docked,
						Color = comp.RadarColor,
						HighlightedColor = comp.HighlightedRadarColor
					};
					result.Add(state);
				}
			}
			return result;
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00032A0C File Offset: 0x00030C0C
		private void UpdateState(ShuttleConsoleComponent component, [Nullable(new byte[]
		{
			2,
			1
		})] List<DockingInterfaceState> docks = null)
		{
			EntityUid? entity = new EntityUid?(component.Owner);
			ConsoleShuttleEvent getShuttleEv = new ConsoleShuttleEvent
			{
				Console = entity
			};
			base.RaiseLocalEvent<ConsoleShuttleEvent>(entity.Value, ref getShuttleEv, false);
			entity = getShuttleEv.Console;
			TransformComponent consoleXform;
			base.TryComp<TransformComponent>(entity, ref consoleXform);
			RadarConsoleComponent radar;
			base.TryComp<RadarConsoleComponent>(entity, ref radar);
			float range = (radar != null) ? radar.MaxRange : 0f;
			ShuttleComponent shuttle;
			base.TryComp<ShuttleComponent>((consoleXform != null) ? consoleXform.GridUid : null, ref shuttle);
			List<ValueTuple<EntityUid, string, bool>> destinations = new List<ValueTuple<EntityUid, string, bool>>();
			FTLState ftlState = FTLState.Available;
			TimeSpan ftlTime = TimeSpan.Zero;
			FTLComponent shuttleFtl;
			if (base.TryComp<FTLComponent>((shuttle != null) ? new EntityUid?(shuttle.Owner) : null, ref shuttleFtl))
			{
				ftlState = shuttleFtl.State;
				ftlTime = this._timing.CurTime + TimeSpan.FromSeconds((double)shuttleFtl.Accumulator);
			}
			if (entity != null && shuttle != null)
			{
				EntityUid owner = shuttle.Owner;
				PhysicsComponent shuttleBody;
				if (!base.TryComp<PhysicsComponent>((shuttle != null) ? new EntityUid?(shuttle.Owner) : null, ref shuttleBody) || shuttleBody.Mass < 1000f)
				{
					EntityQuery<MetaDataComponent> metaQuery = base.GetEntityQuery<MetaDataComponent>();
					bool locked = shuttleFtl != null || base.Paused(shuttle.Owner, null);
					foreach (FTLDestinationComponent comp in base.EntityQuery<FTLDestinationComponent>(true))
					{
						EntityUid owner2 = comp.Owner;
						if (!(owner2 == ((shuttle != null) ? new EntityUid?(shuttle.Owner) : null)))
						{
							EntityWhitelist whitelist = comp.Whitelist;
							if (whitelist == null || whitelist.IsValid(entity.Value, null))
							{
								MetaDataComponent meta = metaQuery.GetComponent(comp.Owner);
								string name = meta.EntityName;
								if (string.IsNullOrEmpty(name))
								{
									name = Loc.GetString("shuttle-console-unknown");
								}
								FTLComponent ftl;
								bool canTravel = !locked && comp.Enabled && !base.Paused(comp.Owner, meta) && (!base.TryComp<FTLComponent>(comp.Owner, ref ftl) || ftl.State == FTLState.Cooldown);
								if (canTravel && ((consoleXform != null) ? consoleXform.MapUid : null) == base.Transform(comp.Owner).MapUid)
								{
									canTravel = false;
								}
								destinations.Add(new ValueTuple<EntityUid, string, bool>(comp.Owner, name, canTravel));
							}
						}
					}
				}
			}
			if (docks == null)
			{
				docks = this.GetAllDocks();
			}
			BoundUserInterface uiOrNull = this._ui.GetUiOrNull(component.Owner, ShuttleConsoleUiKey.Key, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.SetState(new ShuttleConsoleBoundInterfaceState(ftlState, ftlTime, destinations, range, (consoleXform != null) ? new EntityCoordinates?(consoleXform.Coordinates) : null, (consoleXform != null) ? new Angle?(consoleXform.LocalRotation) : null, docks), null, true);
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x00032D80 File Offset: 0x00030F80
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			RemQueue<PilotComponent> toRemove = default(RemQueue<PilotComponent>);
			foreach (PilotComponent comp in this.EntityManager.EntityQuery<PilotComponent>(false))
			{
				if (comp.Console != null && !this._blocker.CanInteract(comp.Owner, new EntityUid?(comp.Console.Owner)))
				{
					toRemove.Add(comp);
				}
			}
			foreach (PilotComponent comp2 in toRemove)
			{
				this.RemovePilot(comp2);
			}
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x00032E50 File Offset: 0x00031050
		private void HandlePilotMove(EntityUid uid, PilotComponent component, ref MoveEvent args)
		{
			if (component.Console == null || component.Position == null)
			{
				this.EntityManager.RemoveComponent<PilotComponent>(uid);
				return;
			}
			float distance;
			if (args.NewPosition.TryDistance(this.EntityManager, component.Position.Value, ref distance) && distance < 0.25f)
			{
				return;
			}
			this.RemovePilot(component);
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00032EB6 File Offset: 0x000310B6
		protected override void HandlePilotShutdown(EntityUid uid, PilotComponent component, ComponentShutdown args)
		{
			base.HandlePilotShutdown(uid, component, args);
			this.RemovePilot(component);
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x00032EC8 File Offset: 0x000310C8
		private void OnConsoleShutdown(EntityUid uid, ShuttleConsoleComponent component, ComponentShutdown args)
		{
			this.ClearPilots(component);
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00032ED4 File Offset: 0x000310D4
		public void AddPilot(EntityUid entity, ShuttleConsoleComponent component)
		{
			PilotComponent pilotComponent;
			if (!this.EntityManager.TryGetComponent<PilotComponent>(entity, ref pilotComponent) || component.SubscribedPilots.Contains(pilotComponent))
			{
				return;
			}
			SharedEyeComponent eye;
			if (base.TryComp<SharedEyeComponent>(entity, ref eye))
			{
				eye.Zoom = component.Zoom;
			}
			component.SubscribedPilots.Add(pilotComponent);
			this._alertsSystem.ShowAlert(entity, AlertType.PilotingShuttle, null, null);
			pilotComponent.Console = component;
			this.ActionBlockerSystem.UpdateCanMove(entity, null);
			pilotComponent.Position = new EntityCoordinates?(this.EntityManager.GetComponent<TransformComponent>(entity).Coordinates);
			base.Dirty(pilotComponent, null);
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00032F7C File Offset: 0x0003117C
		public void RemovePilot(PilotComponent pilotComponent)
		{
			ShuttleConsoleComponent helmsman = pilotComponent.Console as ShuttleConsoleComponent;
			if (helmsman == null)
			{
				return;
			}
			pilotComponent.Console = null;
			pilotComponent.Position = null;
			SharedEyeComponent eye;
			if (base.TryComp<SharedEyeComponent>(pilotComponent.Owner, ref eye))
			{
				eye.Zoom = new Vector2(1f, 1f);
			}
			if (!helmsman.SubscribedPilots.Remove(pilotComponent))
			{
				return;
			}
			this._alertsSystem.ClearAlert(pilotComponent.Owner, AlertType.PilotingShuttle);
			pilotComponent.Owner.PopupMessage(Loc.GetString("shuttle-pilot-end"));
			if (pilotComponent.LifeStage < 7)
			{
				this.EntityManager.RemoveComponent<PilotComponent>(pilotComponent.Owner);
			}
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x00033028 File Offset: 0x00031228
		public void RemovePilot(EntityUid entity)
		{
			PilotComponent pilotComponent;
			if (!this.EntityManager.TryGetComponent<PilotComponent>(entity, ref pilotComponent))
			{
				return;
			}
			this.RemovePilot(pilotComponent);
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00033050 File Offset: 0x00031250
		public void ClearPilots(ShuttleConsoleComponent component)
		{
			PilotComponent pilot;
			while (Extensions.TryGetValue<PilotComponent>(component.SubscribedPilots, 0, ref pilot))
			{
				this.RemovePilot(pilot);
			}
		}

		// Token: 0x040005E5 RID: 1509
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040005E6 RID: 1510
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x040005E7 RID: 1511
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x040005E8 RID: 1512
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x040005E9 RID: 1513
		[Dependency]
		private readonly ShuttleSystem _shuttle;

		// Token: 0x040005EA RID: 1514
		[Dependency]
		private readonly TagSystem _tags;

		// Token: 0x040005EB RID: 1515
		[Dependency]
		private readonly UserInterfaceSystem _ui;
	}
}
