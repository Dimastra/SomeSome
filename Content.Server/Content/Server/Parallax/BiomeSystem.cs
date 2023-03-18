using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Decals;
using Content.Shared.Decals;
using Content.Shared.Parallax.Biomes;
using Robust.Server.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Noise;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Random;

namespace Content.Server.Parallax
{
	// Token: 0x020002EC RID: 748
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BiomeSystem : SharedBiomeSystem
	{
		// Token: 0x06000F56 RID: 3926 RVA: 0x0004E9DF File Offset: 0x0004CBDF
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BiomeComponent, MapInitEvent>(new ComponentEventHandler<BiomeComponent, MapInitEvent>(this.OnBiomeMapInit), null, null);
			this._configManager.OnValueChanged<float>(CVars.NetMaxUpdateRange, new Action<float>(this.SetLoadRange), true);
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x0004EA18 File Offset: 0x0004CC18
		public override void Shutdown()
		{
			base.Shutdown();
			this._configManager.UnsubValueChanged<float>(CVars.NetMaxUpdateRange, new Action<float>(this.SetLoadRange));
		}

		// Token: 0x06000F58 RID: 3928 RVA: 0x0004EA3C File Offset: 0x0004CC3C
		private void SetLoadRange(float obj)
		{
			this._loadRange = MathF.Ceiling(obj / 8f) * 8f;
			this._loadArea = new Box2(-this._loadRange, -this._loadRange, this._loadRange, this._loadRange);
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x0004EA7B File Offset: 0x0004CC7B
		private void OnBiomeMapInit(EntityUid uid, BiomeComponent component, MapInitEvent args)
		{
			component.Seed = this._random.Next();
			base.Dirty(component, null);
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0004EA98 File Offset: 0x0004CC98
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			EntityQuery<BiomeComponent> biomeQuery = base.GetEntityQuery<BiomeComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			BiomeComponent biome;
			while (base.AllEntityQuery<BiomeComponent>().MoveNext(ref biome))
			{
				this._activeChunks.Add(biome, new HashSet<Vector2i>());
			}
			foreach (ICommonSession commonSession in Filter.GetAllPlayers(this._playerManager))
			{
				IPlayerSession pSession = (IPlayerSession)commonSession;
				TransformComponent xform;
				BiomeComponent biome2;
				if (xformQuery.TryGetComponent(pSession.AttachedEntity, ref xform) && this._handledEntities.Add(pSession.AttachedEntity.Value) && biomeQuery.TryGetComponent(xform.MapUid, ref biome2))
				{
					this.AddChunksInRange(biome2, this._transform.GetWorldPosition(xform, xformQuery));
				}
				foreach (EntityUid viewer in pSession.ViewSubscriptions)
				{
					if (this._handledEntities.Add(viewer) && xformQuery.TryGetComponent(viewer, ref xform) && biomeQuery.TryGetComponent(xform.MapUid, ref biome2))
					{
						this.AddChunksInRange(biome2, this._transform.GetWorldPosition(xform, xformQuery));
					}
				}
			}
			BiomeComponent biome3;
			MapGridComponent grid;
			while (base.AllEntityQuery<BiomeComponent, MapGridComponent>().MoveNext(ref biome3, ref grid))
			{
				FastNoise noise = new FastNoise(biome3.Seed);
				EntityUid gridUid = grid.Owner;
				this.LoadChunks(biome3, gridUid, grid, noise, xformQuery);
				this.UnloadChunks(biome3, gridUid, grid, noise);
			}
			this._handledEntities.Clear();
			this._activeChunks.Clear();
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0004EC68 File Offset: 0x0004CE68
		private void AddChunksInRange(BiomeComponent biome, Vector2 worldPos)
		{
			ChunkIndicesEnumerator enumerator = new ChunkIndicesEnumerator(this._loadArea.Translated(worldPos), 8);
			Vector2i? chunkOrigin;
			while (enumerator.MoveNext(out chunkOrigin))
			{
				this._activeChunks[biome].Add(chunkOrigin.Value * 8);
			}
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0004ECB8 File Offset: 0x0004CEB8
		private void LoadChunks(BiomeComponent component, EntityUid gridUid, MapGridComponent grid, FastNoise noise, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			HashSet<Vector2i> hashSet = this._activeChunks[component];
			BiomePrototype prototype = this.ProtoManager.Index<BiomePrototype>(component.BiomePrototype);
			List<ValueTuple<Vector2i, Tile>> tiles = null;
			foreach (Vector2i chunk in hashSet)
			{
				if (component.LoadedChunks.Add(chunk))
				{
					if (tiles == null)
					{
						tiles = new List<ValueTuple<Vector2i, Tile>>(64);
					}
					this.LoadChunk(component, gridUid, grid, chunk, noise, prototype, tiles, xformQuery);
				}
			}
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x0004ED48 File Offset: 0x0004CF48
		private void LoadChunk(BiomeComponent component, EntityUid gridUid, MapGridComponent grid, Vector2i chunk, FastNoise noise, BiomePrototype prototype, [Nullable(new byte[]
		{
			1,
			0
		})] List<ValueTuple<Vector2i, Tile>> tiles, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			HashSet<Vector2i> modified;
			component.ModifiedTiles.TryGetValue(chunk, out modified);
			if (modified == null)
			{
				modified = new HashSet<Vector2i>();
			}
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					Vector2i indices;
					indices..ctor(x + chunk.X, y + chunk.Y);
					TileRef tileRef;
					Tile? biomeTile;
					if (!modified.Contains(indices) && (!grid.TryGetTileRef(indices, ref tileRef) || tileRef.Tile.IsEmpty) && base.TryGetBiomeTile(indices, prototype, noise, null, out biomeTile) && !(biomeTile.Value == tileRef.Tile))
					{
						tiles.Add(new ValueTuple<Vector2i, Tile>(indices, biomeTile.Value));
					}
				}
			}
			grid.SetTiles(tiles);
			tiles.Clear();
			List<EntityUid> loadedEntities = new List<EntityUid>();
			component.LoadedEntities.Add(chunk, loadedEntities);
			EntityUid? entityUid;
			for (int x2 = 0; x2 < 8; x2++)
			{
				for (int y2 = 0; y2 < 8; y2++)
				{
					Vector2i indices2;
					indices2..ctor(x2 + chunk.X, y2 + chunk.Y);
					string entPrototype;
					if (!modified.Contains(indices2) && !grid.GetAnchoredEntitiesEnumerator(indices2).MoveNext(ref entityUid) && base.TryGetEntity(indices2, prototype, noise, grid, out entPrototype))
					{
						EntityUid ent = base.Spawn(entPrototype, grid.GridTileToLocal(indices2));
						TransformComponent xform;
						if (xformQuery.TryGetComponent(ent, ref xform) && !xform.Anchored)
						{
							this._transform.AnchorEntity(ent, xform, grid, indices2);
						}
						loadedEntities.Add(ent);
					}
				}
			}
			Dictionary<uint, Vector2i> loadedDecals = new Dictionary<uint, Vector2i>();
			component.LoadedDecals.Add(chunk, loadedDecals);
			for (int x3 = 0; x3 < 8; x3++)
			{
				for (int y3 = 0; y3 < 8; y3++)
				{
					Vector2i indices3;
					indices3..ctor(x3 + chunk.X, y3 + chunk.Y);
					List<ValueTuple<string, Vector2>> decals;
					if (!modified.Contains(indices3) && !grid.GetAnchoredEntitiesEnumerator(indices3).MoveNext(ref entityUid) && base.TryGetDecals(indices3, prototype, noise, grid, out decals))
					{
						foreach (ValueTuple<string, Vector2> decal in decals)
						{
							uint dec;
							if (this._decals.TryAddDecal(decal.Item1, new EntityCoordinates(gridUid, decal.Item2), out dec, null, null, 0, false))
							{
								loadedDecals.Add(dec, indices3);
							}
						}
					}
				}
			}
			if (modified.Count == 0)
			{
				component.ModifiedTiles.Remove(chunk);
				return;
			}
			component.ModifiedTiles[chunk] = modified;
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x0004F020 File Offset: 0x0004D220
		private void UnloadChunks(BiomeComponent component, EntityUid gridUid, MapGridComponent grid, FastNoise noise)
		{
			HashSet<Vector2i> active = this._activeChunks[component];
			List<ValueTuple<Vector2i, Tile>> tiles = null;
			foreach (Vector2i chunk in component.LoadedChunks)
			{
				if (!active.Contains(chunk) && component.LoadedChunks.Remove(chunk))
				{
					if (tiles == null)
					{
						tiles = new List<ValueTuple<Vector2i, Tile>>(64);
					}
					this.UnloadChunk(component, gridUid, grid, chunk, noise, tiles);
				}
			}
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x0004F0AC File Offset: 0x0004D2AC
		private void UnloadChunk(BiomeComponent component, EntityUid gridUid, MapGridComponent grid, Vector2i chunk, FastNoise noise, [Nullable(new byte[]
		{
			1,
			0
		})] List<ValueTuple<Vector2i, Tile>> tiles)
		{
			BiomePrototype prototype = this.ProtoManager.Index<BiomePrototype>(component.BiomePrototype);
			HashSet<Vector2i> modified;
			component.ModifiedTiles.TryGetValue(chunk, out modified);
			if (modified == null)
			{
				modified = new HashSet<Vector2i>();
			}
			foreach (KeyValuePair<uint, Vector2i> keyValuePair in component.LoadedDecals[chunk])
			{
				uint num;
				Vector2i vector2i;
				keyValuePair.Deconstruct(out num, out vector2i);
				uint dec = num;
				Vector2i indices = vector2i;
				if (!this._decals.RemoveDecal(gridUid, dec, null))
				{
					modified.Add(indices);
				}
			}
			component.LoadedDecals.Remove(chunk);
			component.LoadedEntities.Remove(chunk);
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					Vector2i indices2;
					indices2..ctor(x + chunk.X, y + chunk.Y);
					if (!modified.Contains(indices2))
					{
						EntityUid? entityUid;
						Tile? biomeTile;
						TileRef tileRef;
						if (grid.GetAnchoredEntitiesEnumerator(indices2).MoveNext(ref entityUid))
						{
							modified.Add(indices2);
						}
						else if (!base.TryGetBiomeTile(indices2, prototype, noise, null, out biomeTile) || (grid.TryGetTileRef(indices2, ref tileRef) && tileRef.Tile != biomeTile.Value))
						{
							modified.Add(indices2);
						}
						else
						{
							tiles.Add(new ValueTuple<Vector2i, Tile>(indices2, Tile.Empty));
						}
					}
				}
			}
			grid.SetTiles(tiles);
			tiles.Clear();
			component.LoadedChunks.Remove(chunk);
			if (modified.Count == 0)
			{
				component.ModifiedTiles.Remove(chunk);
				return;
			}
			component.ModifiedTiles[chunk] = modified;
		}

		// Token: 0x04000900 RID: 2304
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000901 RID: 2305
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000902 RID: 2306
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000903 RID: 2307
		[Dependency]
		private readonly DecalSystem _decals;

		// Token: 0x04000904 RID: 2308
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000905 RID: 2309
		private readonly HashSet<EntityUid> _handledEntities = new HashSet<EntityUid>();

		// Token: 0x04000906 RID: 2310
		private const float DefaultLoadRange = 16f;

		// Token: 0x04000907 RID: 2311
		private float _loadRange = 16f;

		// Token: 0x04000908 RID: 2312
		private Box2 _loadArea = new Box2(-16f, -16f, 16f, 16f);

		// Token: 0x04000909 RID: 2313
		private readonly Dictionary<BiomeComponent, HashSet<Vector2i>> _activeChunks = new Dictionary<BiomeComponent, HashSet<Vector2i>>();
	}
}
