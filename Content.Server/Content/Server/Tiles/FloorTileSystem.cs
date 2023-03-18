using System;
using System.Runtime.CompilerServices;
using Content.Server.Stack;
using Content.Shared.Audio;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Tiles
{
	// Token: 0x02000121 RID: 289
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FloorTileSystem : EntitySystem
	{
		// Token: 0x06000538 RID: 1336 RVA: 0x00019558 File Offset: 0x00017758
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FloorTileComponent, AfterInteractEvent>(new ComponentEventHandler<FloorTileComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00019574 File Offset: 0x00017774
		private void OnAfterInteract(EntityUid uid, FloorTileComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach)
			{
				return;
			}
			StackComponent stack;
			if (!base.TryComp<StackComponent>(uid, ref stack))
			{
				return;
			}
			if (component.OutputTiles == null)
			{
				return;
			}
			EntityCoordinates location = CoordinatesExtensions.AlignWithClosestGridTile(args.ClickLocation, 1.5f, null, null);
			EntityQuery<PhysicsComponent> physics = base.GetEntityQuery<PhysicsComponent>();
			foreach (EntityUid ent in location.GetEntitiesInTile(4, this._lookup))
			{
				PhysicsComponent phys;
				if (physics.TryGetComponent(ent, ref phys) && phys.BodyType == 4 && phys.Hard && (phys.CollisionLayer & 2) != 0)
				{
					return;
				}
			}
			MapCoordinates locationMap = location.ToMap(this.EntityManager);
			if (locationMap.MapId == MapId.Nullspace)
			{
				return;
			}
			MapGridComponent mapGrid;
			this._mapManager.TryGetGrid(new EntityUid?(location.EntityId), ref mapGrid);
			foreach (string currentTile in component.OutputTiles)
			{
				ContentTileDefinition currentTileDefinition = (ContentTileDefinition)this._tileDefinitionManager[currentTile];
				if (mapGrid != null)
				{
					TileRef tile = mapGrid.GetTileRef(location);
					ContentTileDefinition baseTurf = (ContentTileDefinition)this._tileDefinitionManager[(int)tile.Tile.TypeId];
					if (this.HasBaseTurf(currentTileDefinition, baseTurf.ID) && this._stackSystem.Use(uid, 1, stack))
					{
						this.PlaceAt(mapGrid, location, currentTileDefinition.TileId, component.PlaceTileSound, 0f);
					}
				}
				else if (this.HasBaseTurf(currentTileDefinition, "Space"))
				{
					mapGrid = this._mapManager.CreateGrid(locationMap.MapId);
					base.Transform(mapGrid.Owner).WorldPosition = locationMap.Position;
					location = new EntityCoordinates(mapGrid.Owner, Vector2.Zero);
					this.PlaceAt(mapGrid, location, this._tileDefinitionManager[component.OutputTiles[0]].TileId, component.PlaceTileSound, (float)mapGrid.TileSize / 2f);
				}
			}
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x000197D0 File Offset: 0x000179D0
		public bool HasBaseTurf(ContentTileDefinition tileDef, string baseTurf)
		{
			foreach (string tileBaseTurf in tileDef.BaseTurfs)
			{
				if (baseTurf == tileBaseTurf)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001982C File Offset: 0x00017A2C
		private void PlaceAt(MapGridComponent mapGrid, EntityCoordinates location, ushort tileId, SoundSpecifier placeSound, float offset = 0f)
		{
			byte variant = RandomExtensions.Pick<byte>(this._random, ((ContentTileDefinition)this._tileDefinitionManager[(int)tileId]).PlacementVariants);
			mapGrid.SetTile(location.Offset(new Vector2(offset, offset)), new Tile(tileId, 0, variant));
			this._audio.Play(placeSound, Filter.Pvs(location, 2f, null, null), location, true, new AudioParams?(AudioHelpers.WithVariation(0.125f, this._random)));
		}

		// Token: 0x04000322 RID: 802
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x04000323 RID: 803
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000324 RID: 804
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000325 RID: 805
		[Dependency]
		private readonly StackSystem _stackSystem;

		// Token: 0x04000326 RID: 806
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000327 RID: 807
		[Dependency]
		private readonly EntityLookupSystem _lookup;
	}
}
