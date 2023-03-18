using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.Components;
using Content.Server.Maps;
using Content.Server.Popups;
using Content.Server.Tools.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Temperature;
using Content.Shared.Toggleable;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Tools
{
	// Token: 0x02000111 RID: 273
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ToolSystem : SharedToolSystem
	{
		// Token: 0x060004E2 RID: 1250 RVA: 0x00017553 File Offset: 0x00015753
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeTilePrying();
			this.InitializeLatticeCutting();
			this.InitializeWelders();
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x0001756D File Offset: 0x0001576D
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateWelders(frameTime);
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0001757D File Offset: 0x0001577D
		private void InitializeLatticeCutting()
		{
			base.SubscribeLocalEvent<LatticeCuttingComponent, AfterInteractEvent>(new ComponentEventHandler<LatticeCuttingComponent, AfterInteractEvent>(this.OnLatticeCuttingAfterInteract), null, null);
			base.SubscribeLocalEvent<LatticeCuttingComponent, ToolSystem.LatticeCuttingCompleteEvent>(new ComponentEventHandler<LatticeCuttingComponent, ToolSystem.LatticeCuttingCompleteEvent>(this.OnLatticeCutComplete), null, null);
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x000175A8 File Offset: 0x000157A8
		private void OnLatticeCutComplete(EntityUid uid, LatticeCuttingComponent component, ToolSystem.LatticeCuttingCompleteEvent args)
		{
			EntityUid? gridUid = args.Coordinates.GetGridUid(this.EntityManager);
			if (gridUid == null)
			{
				return;
			}
			TileRef tile = this._mapManager.GetGrid(gridUid.Value).GetTileRef(args.Coordinates);
			ContentTileDefinition tileDef = this._tileDefinitionManager[(int)tile.Tile.TypeId] as ContentTileDefinition;
			if (tileDef == null || !tileDef.CanWirecutter || tileDef.BaseTurfs.Count == 0 || tile.IsBlockedTurf(true, null, null))
			{
				return;
			}
			this._tile.CutTile(tile);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.LatticeCut;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(20, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" cut the lattice at ");
			logStringHandler.AppendFormatted<EntityCoordinates>(args.Coordinates, "target", "args.Coordinates");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00017698 File Offset: 0x00015898
		private void OnLatticeCuttingAfterInteract(EntityUid uid, LatticeCuttingComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach || args.Target != null)
			{
				return;
			}
			if (this.TryCut(uid, args.User, component, args.ClickLocation))
			{
				args.Handled = true;
			}
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x000176E4 File Offset: 0x000158E4
		private bool TryCut(EntityUid toolEntity, EntityUid user, LatticeCuttingComponent component, EntityCoordinates clickLocation)
		{
			ToolComponent tool = null;
			if (component.ToolComponentNeeded && !base.TryComp<ToolComponent>(toolEntity, ref tool))
			{
				return false;
			}
			MapGridComponent mapGrid;
			if (!this._mapManager.TryGetGrid(clickLocation.GetGridUid(this.EntityManager), ref mapGrid))
			{
				return false;
			}
			TileRef tile = mapGrid.GetTileRef(clickLocation);
			EntityCoordinates coordinates = mapGrid.GridTileToLocal(tile.GridIndices);
			if (!this._interactionSystem.InRangeUnobstructed(user, coordinates, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				return false;
			}
			ContentTileDefinition tileDef = this._tileDefinitionManager[(int)tile.Tile.TypeId] as ContentTileDefinition;
			if (tileDef != null && tileDef.CanWirecutter && tileDef.BaseTurfs.Count != 0)
			{
				ITileDefinitionManager tileDefinitionManager = this._tileDefinitionManager;
				List<string> baseTurfs = tileDef.BaseTurfs;
				if (tileDefinitionManager[baseTurfs[baseTurfs.Count - 1]] is ContentTileDefinition && !tile.IsBlockedTurf(true, null, null))
				{
					ToolEventData toolEvData = new ToolEventData(new ToolSystem.LatticeCuttingCompleteEvent(clickLocation, user), 0f, null, new EntityUid?(toolEntity));
					return base.UseTool(toolEntity, user, null, component.Delay, new string[]
					{
						component.QualityNeeded
					}, toolEvData, 0f, tool, null, null);
				}
			}
			return false;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x00017815 File Offset: 0x00015A15
		private void InitializeTilePrying()
		{
			base.SubscribeLocalEvent<TilePryingComponent, AfterInteractEvent>(new ComponentEventHandler<TilePryingComponent, AfterInteractEvent>(this.OnTilePryingAfterInteract), null, null);
			base.SubscribeLocalEvent<TilePryingComponent, ToolSystem.TilePryingCompleteEvent>(new ComponentEventHandler<TilePryingComponent, ToolSystem.TilePryingCompleteEvent>(this.OnTilePryComplete), null, null);
			base.SubscribeLocalEvent<TilePryingComponent, ToolSystem.TilePryingCancelledEvent>(new ComponentEventHandler<TilePryingComponent, ToolSystem.TilePryingCancelledEvent>(this.OnTilePryCancelled), null, null);
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00017854 File Offset: 0x00015A54
		private void OnTilePryingAfterInteract(EntityUid uid, TilePryingComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach || (args.Target != null && !base.HasComp<PuddleComponent>(args.Target)))
			{
				return;
			}
			if (this.TryPryTile(uid, args.User, component, args.ClickLocation))
			{
				args.Handled = true;
			}
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x000178B0 File Offset: 0x00015AB0
		private void OnTilePryComplete(EntityUid uid, TilePryingComponent component, ToolSystem.TilePryingCompleteEvent args)
		{
			component.CancelToken = null;
			EntityUid? gridUid = args.Coordinates.GetGridUid(this.EntityManager);
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(gridUid, ref grid))
			{
				Logger.Error("Attempted to pry from a non-existent grid?");
				return;
			}
			TileRef tile = grid.GetTileRef(args.Coordinates);
			this._tile.PryTile(tile);
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0001790B File Offset: 0x00015B0B
		private void OnTilePryCancelled(EntityUid uid, TilePryingComponent component, ToolSystem.TilePryingCancelledEvent args)
		{
			component.CancelToken = null;
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00017914 File Offset: 0x00015B14
		private bool TryPryTile(EntityUid toolEntity, EntityUid user, TilePryingComponent component, EntityCoordinates clickLocation)
		{
			ToolComponent tool;
			if ((!base.TryComp<ToolComponent>(toolEntity, ref tool) && component.ToolComponentNeeded) || component.CancelToken != null)
			{
				return false;
			}
			MapGridComponent mapGrid;
			if (!this._mapManager.TryGetGrid(clickLocation.GetGridUid(this.EntityManager), ref mapGrid))
			{
				return false;
			}
			TileRef tile = mapGrid.GetTileRef(clickLocation);
			EntityCoordinates coordinates = mapGrid.GridTileToLocal(tile.GridIndices);
			if (!this._interactionSystem.InRangeUnobstructed(user, coordinates, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				return false;
			}
			if (!((ContentTileDefinition)this._tileDefinitionManager[(int)tile.Tile.TypeId]).CanCrowbar)
			{
				return false;
			}
			component.CancelToken = new CancellationTokenSource();
			ToolEventData toolEvData = new ToolEventData(new ToolSystem.TilePryingCompleteEvent(clickLocation), 0f, new ToolSystem.TilePryingCancelledEvent(), new EntityUid?(toolEntity));
			return base.UseTool(toolEntity, user, null, component.Delay, new string[]
			{
				component.QualityNeeded
			}, toolEvData, 0f, tool, null, component.CancelToken);
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x00017A18 File Offset: 0x00015C18
		public void InitializeWelders()
		{
			base.SubscribeLocalEvent<WelderComponent, ComponentStartup>(new ComponentEventHandler<WelderComponent, ComponentStartup>(this.OnWelderStartup), null, null);
			base.SubscribeLocalEvent<WelderComponent, IsHotEvent>(new ComponentEventHandler<WelderComponent, IsHotEvent>(this.OnWelderIsHotEvent), null, null);
			base.SubscribeLocalEvent<WelderComponent, ExaminedEvent>(new ComponentEventHandler<WelderComponent, ExaminedEvent>(this.OnWelderExamine), null, null);
			base.SubscribeLocalEvent<WelderComponent, SolutionChangedEvent>(new ComponentEventHandler<WelderComponent, SolutionChangedEvent>(this.OnWelderSolutionChange), null, null);
			base.SubscribeLocalEvent<WelderComponent, ActivateInWorldEvent>(new ComponentEventHandler<WelderComponent, ActivateInWorldEvent>(this.OnWelderActivate), null, null);
			base.SubscribeLocalEvent<WelderComponent, AfterInteractEvent>(new ComponentEventHandler<WelderComponent, AfterInteractEvent>(this.OnWelderAfterInteract), null, null);
			base.SubscribeLocalEvent<WelderComponent, ToolUseAttemptEvent>(new ComponentEventHandler<WelderComponent, ToolUseAttemptEvent>(this.OnWelderToolUseAttempt), null, null);
			base.SubscribeLocalEvent<WelderComponent, ToolUseFinishAttemptEvent>(new ComponentEventHandler<WelderComponent, ToolUseFinishAttemptEvent>(this.OnWelderToolUseFinishAttempt), null, null);
			base.SubscribeLocalEvent<WelderComponent, ComponentShutdown>(new ComponentEventHandler<WelderComponent, ComponentShutdown>(this.OnWelderShutdown), null, null);
			base.SubscribeLocalEvent<WelderComponent, ComponentGetState>(new ComponentEventRefHandler<WelderComponent, ComponentGetState>(this.OnWelderGetState), null, null);
			base.SubscribeLocalEvent<WelderComponent, MeleeHitEvent>(new ComponentEventHandler<WelderComponent, MeleeHitEvent>(this.OnMeleeHit), null, null);
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x00017B01 File Offset: 0x00015D01
		private void OnMeleeHit(EntityUid uid, WelderComponent component, MeleeHitEvent args)
		{
			if (!args.Handled && component.Lit)
			{
				args.BonusDamage += component.LitMeleeDamageBonus;
			}
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x00017B2C File Offset: 0x00015D2C
		[NullableContext(2)]
		[return: TupleElementNames(new string[]
		{
			"fuel",
			"capacity"
		})]
		[return: Nullable(0)]
		public ValueTuple<FixedPoint2, FixedPoint2> GetWelderFuelAndCapacity(EntityUid uid, WelderComponent welder = null, SolutionContainerManagerComponent solutionContainer = null)
		{
			Solution fuelSolution;
			if (!base.Resolve<WelderComponent, SolutionContainerManagerComponent>(uid, ref welder, ref solutionContainer, true) || !this._solutionContainerSystem.TryGetSolution(uid, welder.FuelSolution, out fuelSolution, solutionContainer))
			{
				return new ValueTuple<FixedPoint2, FixedPoint2>(FixedPoint2.Zero, FixedPoint2.Zero);
			}
			return new ValueTuple<FixedPoint2, FixedPoint2>(this._solutionContainerSystem.GetReagentQuantity(uid, welder.FuelReagent), fuelSolution.MaxVolume);
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x00017B8C File Offset: 0x00015D8C
		[NullableContext(2)]
		public bool TryToggleWelder(EntityUid uid, EntityUid? user, WelderComponent welder = null, SolutionContainerManagerComponent solutionContainer = null, ItemComponent item = null, PointLightComponent light = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<WelderComponent>(uid, ref welder, true))
			{
				return false;
			}
			if (welder.Lit)
			{
				return this.TryTurnWelderOff(uid, user, welder, item, light, appearance);
			}
			return this.TryTurnWelderOn(uid, user, welder, solutionContainer, item, light, appearance, null);
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00017BD4 File Offset: 0x00015DD4
		[NullableContext(2)]
		public bool TryTurnWelderOn(EntityUid uid, EntityUid? user, WelderComponent welder = null, SolutionContainerManagerComponent solutionContainer = null, ItemComponent item = null, PointLightComponent light = null, AppearanceComponent appearance = null, TransformComponent transform = null)
		{
			if (!base.Resolve<WelderComponent, SolutionContainerManagerComponent, TransformComponent>(uid, ref welder, ref solutionContainer, ref transform, true))
			{
				return false;
			}
			base.Resolve<ItemComponent, PointLightComponent, AppearanceComponent>(uid, ref item, ref light, ref appearance, false);
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, welder.FuelSolution, out solution, solutionContainer))
			{
				return false;
			}
			FixedPoint2 fuel = solution.GetReagentQuantity(welder.FuelReagent);
			if (fuel == FixedPoint2.Zero || fuel < welder.FuelLitCost)
			{
				if (user != null)
				{
					this._popupSystem.PopupEntity(Loc.GetString("welder-component-no-fuel-message"), uid, user.Value, PopupType.Small);
				}
				return false;
			}
			solution.RemoveReagent(welder.FuelReagent, welder.FuelLitCost);
			welder.Lit = true;
			if (user != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.InteractActivate;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(12, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
				logStringHandler.AppendLiteral(" toggled ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "welder", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" on");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Action;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(11, 1);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "welder", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" toggled on");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			WelderToggledEvent ev = new WelderToggledEvent(true);
			base.RaiseLocalEvent<WelderToggledEvent>(welder.Owner, ev, false);
			IsHotEvent hotEvent = new IsHotEvent
			{
				IsHot = true
			};
			base.RaiseLocalEvent<IsHotEvent>(uid, hotEvent, false);
			this._appearanceSystem.SetData(uid, WelderVisuals.Lit, true, null);
			this._appearanceSystem.SetData(uid, ToggleableLightVisuals.Enabled, true, null);
			if (light != null)
			{
				light.Enabled = true;
			}
			this._audioSystem.PlayPvs(welder.WelderOnSounds, uid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.125f)).WithVolume(-5f)));
			EntityUid? gridUid2 = transform.GridUid;
			if (gridUid2 != null)
			{
				EntityUid gridUid = gridUid2.GetValueOrDefault();
				Vector2i position = this._transformSystem.GetGridOrMapTilePosition(uid, transform);
				this._atmosphereSystem.HotspotExpose(gridUid, position, 700f, 50f, true);
			}
			this._entityManager.Dirty(welder, null);
			this._activeWelders.Add(uid);
			return true;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00017E3C File Offset: 0x0001603C
		[NullableContext(2)]
		public bool TryTurnWelderOff(EntityUid uid, EntityUid? user, WelderComponent welder = null, ItemComponent item = null, PointLightComponent light = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<WelderComponent>(uid, ref welder, true))
			{
				return false;
			}
			base.Resolve<ItemComponent, PointLightComponent, AppearanceComponent>(uid, ref item, ref light, ref appearance, false);
			welder.Lit = false;
			if (user != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.InteractActivate;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(13, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
				logStringHandler.AppendLiteral(" toggled ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "welder", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" off");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Action;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(12, 1);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "welder", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" toggled off");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			WelderToggledEvent ev = new WelderToggledEvent(false);
			base.RaiseLocalEvent<WelderToggledEvent>(welder.Owner, ev, false);
			IsHotEvent hotEvent = new IsHotEvent
			{
				IsHot = false
			};
			base.RaiseLocalEvent<IsHotEvent>(uid, hotEvent, false);
			this._appearanceSystem.SetData(uid, WelderVisuals.Lit, false, null);
			this._appearanceSystem.SetData(uid, ToggleableLightVisuals.Enabled, false, null);
			if (light != null)
			{
				light.Enabled = false;
			}
			this._audioSystem.PlayPvs(welder.WelderOffSounds, uid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.125f)).WithVolume(-5f)));
			this._entityManager.Dirty(welder, null);
			this._activeWelders.Remove(uid);
			return true;
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00017FDB File Offset: 0x000161DB
		private void OnWelderStartup(EntityUid uid, WelderComponent welder, ComponentStartup args)
		{
			this._entityManager.Dirty(welder, null);
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00017FEA File Offset: 0x000161EA
		private void OnWelderIsHotEvent(EntityUid uid, WelderComponent welder, IsHotEvent args)
		{
			args.IsHot = welder.Lit;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00017FF8 File Offset: 0x000161F8
		private void OnWelderExamine(EntityUid uid, WelderComponent welder, ExaminedEvent args)
		{
			if (welder.Lit)
			{
				args.PushMarkup(Loc.GetString("welder-component-on-examine-welder-lit-message"));
			}
			else
			{
				args.PushMarkup(Loc.GetString("welder-component-on-examine-welder-not-lit-message"));
			}
			if (args.IsInDetailsRange)
			{
				ValueTuple<FixedPoint2, FixedPoint2> welderFuelAndCapacity = this.GetWelderFuelAndCapacity(uid, welder, null);
				FixedPoint2 fuel = welderFuelAndCapacity.Item1;
				FixedPoint2 capacity = welderFuelAndCapacity.Item2;
				args.PushMarkup(Loc.GetString("welder-component-on-examine-detailed-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("colorName", (fuel < capacity / FixedPoint2.New(4f)) ? "darkorange" : "orange"),
					new ValueTuple<string, object>("fuelLeft", fuel),
					new ValueTuple<string, object>("fuelCapacity", capacity),
					new ValueTuple<string, object>("status", string.Empty)
				}));
			}
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x000180DF File Offset: 0x000162DF
		private void OnWelderSolutionChange(EntityUid uid, WelderComponent welder, SolutionChangedEvent args)
		{
			this._entityManager.Dirty(welder, null);
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x000180F0 File Offset: 0x000162F0
		private void OnWelderActivate(EntityUid uid, WelderComponent welder, ActivateInWorldEvent args)
		{
			args.Handled = this.TryToggleWelder(uid, new EntityUid?(args.User), welder, null, null, null, null);
			if (args.Handled)
			{
				args.WasLogged = true;
			}
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001812C File Offset: 0x0001632C
		private void OnWelderAfterInteract(EntityUid uid, WelderComponent welder, AfterInteractEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			EntityUid? target2 = args.Target;
			if (target2 != null)
			{
				EntityUid target = target2.GetValueOrDefault();
				if (target.Valid && args.CanReach)
				{
					ReagentTankComponent tank;
					Solution targetSolution;
					Solution welderSolution;
					if (this.EntityManager.TryGetComponent<ReagentTankComponent>(target, ref tank) && tank.TankType == ReagentTankType.Fuel && this._solutionContainerSystem.TryGetDrainableSolution(target, out targetSolution, null, null) && this._solutionContainerSystem.TryGetSolution(uid, welder.FuelSolution, out welderSolution, null))
					{
						FixedPoint2 trans = FixedPoint2.Min(welderSolution.AvailableVolume, targetSolution.Volume);
						if (trans > 0)
						{
							Solution drained = this._solutionContainerSystem.Drain(target, targetSolution, trans, null);
							this._solutionContainerSystem.TryAddSolution(uid, welderSolution, drained);
							this._audioSystem.PlayPvs(welder.WelderRefill, uid, null);
							this._popupSystem.PopupEntity(Loc.GetString("welder-component-after-interact-refueled-message"), uid, args.User, PopupType.Small);
						}
						else if (welderSolution.AvailableVolume <= 0)
						{
							this._popupSystem.PopupEntity(Loc.GetString("welder-component-already-full"), uid, args.User, PopupType.Small);
						}
						else
						{
							this._popupSystem.PopupEntity(Loc.GetString("welder-component-no-fuel-in-tank", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("owner", args.Target)
							}), uid, args.User, PopupType.Small);
						}
					}
					args.Handled = true;
					return;
				}
			}
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x000182AC File Offset: 0x000164AC
		private void OnWelderToolUseAttempt(EntityUid uid, WelderComponent welder, ToolUseAttemptEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			if (!welder.Lit)
			{
				this._popupSystem.PopupEntity(Loc.GetString("welder-component-welder-not-lit-message"), uid, args.User, PopupType.Small);
				args.Cancel();
				return;
			}
			FixedPoint2 fuel = this.GetWelderFuelAndCapacity(uid, welder, null).Item1;
			if (FixedPoint2.New(args.Fuel) > fuel)
			{
				this._popupSystem.PopupEntity(Loc.GetString("welder-component-cannot-weld-message"), uid, args.User, PopupType.Small);
				args.Cancel();
			}
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00018334 File Offset: 0x00016534
		private void OnWelderToolUseFinishAttempt(EntityUid uid, WelderComponent welder, ToolUseFinishAttemptEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			if (!welder.Lit)
			{
				this._popupSystem.PopupEntity(Loc.GetString("welder-component-welder-not-lit-message"), uid, args.User, PopupType.Small);
				args.Cancel();
				return;
			}
			FixedPoint2 fuel = this.GetWelderFuelAndCapacity(uid, welder, null).Item1;
			FixedPoint2 neededFuel = FixedPoint2.New(args.Fuel);
			if (neededFuel > fuel)
			{
				args.Cancel();
			}
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, welder.FuelSolution, out solution, null))
			{
				args.Cancel();
				return;
			}
			solution.RemoveReagent(welder.FuelReagent, neededFuel);
			this._entityManager.Dirty(welder, null);
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x000183D9 File Offset: 0x000165D9
		private void OnWelderShutdown(EntityUid uid, WelderComponent welder, ComponentShutdown args)
		{
			this._activeWelders.Remove(uid);
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x000183E8 File Offset: 0x000165E8
		private void OnWelderGetState(EntityUid uid, WelderComponent welder, ref ComponentGetState args)
		{
			ValueTuple<FixedPoint2, FixedPoint2> welderFuelAndCapacity = this.GetWelderFuelAndCapacity(uid, welder, null);
			FixedPoint2 fuel = welderFuelAndCapacity.Item1;
			FixedPoint2 capacity = welderFuelAndCapacity.Item2;
			args.State = new WelderComponentState(capacity.Float(), fuel.Float(), welder.Lit);
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0001842C File Offset: 0x0001662C
		private void UpdateWelders(float frameTime)
		{
			this._welderTimer += frameTime;
			if (this._welderTimer < 1f)
			{
				return;
			}
			foreach (EntityUid tool in this._activeWelders.ToArray<EntityUid>())
			{
				WelderComponent welder;
				SolutionContainerManagerComponent solutionContainer;
				TransformComponent transform;
				Solution solution;
				if (this.EntityManager.TryGetComponent<WelderComponent>(tool, ref welder) && this.EntityManager.TryGetComponent<SolutionContainerManagerComponent>(tool, ref solutionContainer) && this.EntityManager.TryGetComponent<TransformComponent>(tool, ref transform) && this._solutionContainerSystem.TryGetSolution(tool, welder.FuelSolution, out solution, solutionContainer))
				{
					solution.RemoveReagent(welder.FuelReagent, welder.FuelConsumption * this._welderTimer);
					if (solution.GetReagentQuantity(welder.FuelReagent) <= FixedPoint2.Zero)
					{
						this.TryTurnWelderOff(tool, null, welder, null, null, null);
					}
					this._entityManager.Dirty(welder, null);
				}
			}
			this._welderTimer -= 1f;
		}

		// Token: 0x040002D9 RID: 729
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x040002DA RID: 730
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040002DB RID: 731
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x040002DC RID: 732
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040002DD RID: 733
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040002DE RID: 734
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x040002DF RID: 735
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040002E0 RID: 736
		[Dependency]
		private readonly TileSystem _tile;

		// Token: 0x040002E1 RID: 737
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x040002E2 RID: 738
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040002E3 RID: 739
		[Dependency]
		private readonly AppearanceSystem _appearanceSystem;

		// Token: 0x040002E4 RID: 740
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x040002E5 RID: 741
		private readonly HashSet<EntityUid> _activeWelders = new HashSet<EntityUid>();

		// Token: 0x040002E6 RID: 742
		private const float WelderUpdateTimer = 1f;

		// Token: 0x040002E7 RID: 743
		private float _welderTimer;

		// Token: 0x020008D5 RID: 2261
		[NullableContext(0)]
		private sealed class LatticeCuttingCompleteEvent : EntityEventArgs
		{
			// Token: 0x0600308C RID: 12428 RVA: 0x000FA8B5 File Offset: 0x000F8AB5
			public LatticeCuttingCompleteEvent(EntityCoordinates coordinates, EntityUid user)
			{
				this.Coordinates = coordinates;
				this.User = user;
			}

			// Token: 0x04001DD2 RID: 7634
			public EntityCoordinates Coordinates;

			// Token: 0x04001DD3 RID: 7635
			public EntityUid User;
		}

		// Token: 0x020008D6 RID: 2262
		[NullableContext(0)]
		private sealed class TilePryingCompleteEvent : EntityEventArgs
		{
			// Token: 0x0600308D RID: 12429 RVA: 0x000FA8CB File Offset: 0x000F8ACB
			public TilePryingCompleteEvent(EntityCoordinates coordinates)
			{
				this.Coordinates = coordinates;
			}

			// Token: 0x04001DD4 RID: 7636
			public readonly EntityCoordinates Coordinates;
		}

		// Token: 0x020008D7 RID: 2263
		[NullableContext(0)]
		private sealed class TilePryingCancelledEvent : EntityEventArgs
		{
		}
	}
}
