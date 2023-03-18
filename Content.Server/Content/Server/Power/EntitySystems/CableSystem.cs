using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Electrocution;
using Content.Server.Power.Components;
using Content.Server.Stack;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Stacks;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200028C RID: 652
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CableSystem : EntitySystem
	{
		// Token: 0x06000D17 RID: 3351 RVA: 0x00044808 File Offset: 0x00042A08
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeCablePlacer();
			base.SubscribeLocalEvent<CableComponent, InteractUsingEvent>(new ComponentEventHandler<CableComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<CableComponent, CuttingFinishedEvent>(new ComponentEventHandler<CableComponent, CuttingFinishedEvent>(this.OnCableCut), null, null);
			base.SubscribeLocalEvent<CableComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<CableComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x00044860 File Offset: 0x00042A60
		private void OnInteractUsing(EntityUid uid, CableComponent cable, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToolEventData toolEvData = new ToolEventData(new CuttingFinishedEvent(args.User), 0f, null, new EntityUid?(uid));
			args.Handled = this._toolSystem.UseTool(args.Used, args.User, new EntityUid?(uid), cable.CuttingDelay, new string[]
			{
				cable.CuttingQuality
			}, toolEvData, 0f, null, null, null);
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x000448D4 File Offset: 0x00042AD4
		private void OnCableCut(EntityUid uid, CableComponent cable, CuttingFinishedEvent args)
		{
			if (this._electrocutionSystem.TryDoElectrifiedAct(uid, args.User, 1f, null, null, null))
			{
				return;
			}
			ISharedAdminLogManager adminLogs = this._adminLogs;
			LogType type = LogType.CableCut;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(21, 3);
			logStringHandler.AppendLiteral("The ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" at ");
			logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(uid).Coordinates, "Transform(uid).Coordinates");
			logStringHandler.AppendLiteral(" was cut by ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(".");
			adminLogs.Add(type, impact, ref logStringHandler);
			base.Spawn(cable.CableDroppedOnCutPrototype, base.Transform(uid).Coordinates);
			base.QueueDel(uid);
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x000449AC File Offset: 0x00042BAC
		private void OnAnchorChanged(EntityUid uid, CableComponent cable, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				return;
			}
			EntityLifeStage? life;
			if (base.TryLifeStage(uid, ref life, null))
			{
				EntityLifeStage? entityLifeStage = life;
				EntityLifeStage entityLifeStage2 = 4;
				if (!(entityLifeStage.GetValueOrDefault() >= entityLifeStage2 & entityLifeStage != null))
				{
					base.Spawn(cable.CableDroppedOnCutPrototype, base.Transform(uid).Coordinates);
					base.QueueDel(uid);
					return;
				}
			}
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x00044A0A File Offset: 0x00042C0A
		private void InitializeCablePlacer()
		{
			base.SubscribeLocalEvent<CablePlacerComponent, AfterInteractEvent>(new ComponentEventHandler<CablePlacerComponent, AfterInteractEvent>(this.OnCablePlacerAfterInteract), null, null);
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x00044A20 File Offset: 0x00042C20
		private void OnCablePlacerAfterInteract(EntityUid uid, CablePlacerComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			if (component.CablePrototypeId == null)
			{
				return;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(args.ClickLocation.GetGridUid(this.EntityManager), ref grid))
			{
				return;
			}
			Vector2i snapPos = grid.TileIndicesFor(args.ClickLocation);
			ContentTileDefinition tileDef = (ContentTileDefinition)this._tileManager[(int)grid.GetTileRef(snapPos).Tile.TypeId];
			if (!tileDef.IsSubFloor || !tileDef.Sturdy)
			{
				return;
			}
			foreach (EntityUid anchored in grid.GetAnchoredEntities(snapPos))
			{
				CableComponent wire;
				if (base.TryComp<CableComponent>(anchored, ref wire) && wire.CableType == component.BlockingCableType)
				{
					return;
				}
			}
			StackComponent stack;
			if (base.TryComp<StackComponent>(component.Owner, ref stack) && !this._stack.Use(component.Owner, 1, stack))
			{
				return;
			}
			EntityUid newCable = this.EntityManager.SpawnEntity(component.CablePrototypeId, grid.GridTileToLocal(snapPos));
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Construction;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(12, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" placed ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(newCable), "cable", "ToPrettyString(newCable)");
			logStringHandler.AppendLiteral(" at ");
			logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(newCable).Coordinates, "Transform(newCable).Coordinates");
			adminLogger.Add(type, impact, ref logStringHandler);
			args.Handled = true;
		}

		// Token: 0x040007E7 RID: 2023
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040007E8 RID: 2024
		[Dependency]
		private readonly ITileDefinitionManager _tileManager;

		// Token: 0x040007E9 RID: 2025
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x040007EA RID: 2026
		[Dependency]
		private readonly StackSystem _stack;

		// Token: 0x040007EB RID: 2027
		[Dependency]
		private readonly ElectrocutionSystem _electrocutionSystem;

		// Token: 0x040007EC RID: 2028
		[Dependency]
		private readonly IAdminLogManager _adminLogs;

		// Token: 0x040007ED RID: 2029
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
