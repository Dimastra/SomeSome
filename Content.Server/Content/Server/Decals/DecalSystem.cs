using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Chunking;
using Content.Shared.Decals;
using Content.Shared.Maps;
using Microsoft.Extensions.ObjectPool;
using Robust.Server.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Threading;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Decals
{
	// Token: 0x020005B1 RID: 1457
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DecalSystem : SharedDecalSystem
	{
		// Token: 0x06001E30 RID: 7728 RVA: 0x0009FAD0 File Offset: 0x0009DCD0
		public override void Initialize()
		{
			base.Initialize();
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
			base.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.OnTileChanged), null, null);
			base.SubscribeLocalEvent<DecalGridComponent, ComponentGetState>(new ComponentEventRefHandler<DecalGridComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeNetworkEvent<RequestDecalPlacementEvent>(new EntitySessionEventHandler<RequestDecalPlacementEvent>(this.OnDecalPlacementRequest), null, null);
			base.SubscribeNetworkEvent<RequestDecalRemovalEvent>(new EntitySessionEventHandler<RequestDecalRemovalEvent>(this.OnDecalRemovalRequest), null, null);
			base.SubscribeLocalEvent<PostGridSplitEvent>(new EntityEventRefHandler<PostGridSplitEvent>(this.OnGridSplit), null, null);
			this._conf.OnValueChanged<bool>(CVars.NetPVS, new Action<bool>(this.OnPvsToggle), true);
		}

		// Token: 0x06001E31 RID: 7729 RVA: 0x0009FB7C File Offset: 0x0009DD7C
		private void OnPvsToggle(bool value)
		{
			if (value == this._pvsEnabled)
			{
				return;
			}
			this._pvsEnabled = value;
			if (value)
			{
				return;
			}
			foreach (Dictionary<EntityUid, HashSet<Vector2i>> dictionary in this._previousSentChunks.Values)
			{
				dictionary.Clear();
			}
			foreach (ValueTuple<DecalGridComponent, MetaDataComponent> valueTuple in base.EntityQuery<DecalGridComponent, MetaDataComponent>(true))
			{
				DecalGridComponent grid = valueTuple.Item1;
				MetaDataComponent meta = valueTuple.Item2;
				grid.ForceTick = this._timing.CurTick;
				base.Dirty(grid, meta);
			}
		}

		// Token: 0x06001E32 RID: 7730 RVA: 0x0009FC44 File Offset: 0x0009DE44
		private void OnGetState(EntityUid uid, DecalGridComponent component, ref ComponentGetState args)
		{
			if (this._pvsEnabled && !args.ReplayState)
			{
				return;
			}
			if (args.FromTick <= component.CreationTick || args.FromTick <= component.ForceTick)
			{
				args.State = new DecalGridState(component.ChunkCollection.ChunkCollection);
				return;
			}
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> data = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>();
			foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> keyValuePair in component.ChunkCollection.ChunkCollection)
			{
				Vector2i vector2i;
				DecalGridComponent.DecalChunk decalChunk;
				keyValuePair.Deconstruct(out vector2i, out decalChunk);
				Vector2i index = vector2i;
				DecalGridComponent.DecalChunk chunk = decalChunk;
				if (chunk.LastModified >= args.FromTick)
				{
					data[index] = chunk;
				}
			}
			args.State = new DecalGridState(data)
			{
				AllChunks = new HashSet<Vector2i>(component.ChunkCollection.ChunkCollection.Keys)
			};
		}

		// Token: 0x06001E33 RID: 7731 RVA: 0x0009FD40 File Offset: 0x0009DF40
		private void OnGridSplit(ref PostGridSplitEvent ev)
		{
			DecalGridComponent oldComp;
			if (!base.TryComp<DecalGridComponent>(ev.OldGrid, ref oldComp))
			{
				return;
			}
			DecalGridComponent newComp;
			if (!base.TryComp<DecalGridComponent>(ev.Grid, ref newComp))
			{
				return;
			}
			GridTileEnumerator enumerator = this.MapManager.GetGrid(ev.Grid).GetAllTilesEnumerator(true);
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> oldChunkCollection = oldComp.ChunkCollection.ChunkCollection;
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunkCollection = newComp.ChunkCollection.ChunkCollection;
			TileRef? tile;
			while (enumerator.MoveNext(ref tile))
			{
				Vector2 tilePos = tile.Value.GridIndices;
				Vector2i chunkIndices = SharedDecalSystem.GetChunkIndices(tilePos);
				DecalGridComponent.DecalChunk oldChunk;
				if (oldChunkCollection.TryGetValue(chunkIndices, out oldChunk))
				{
					Box2 bounds;
					bounds..ctor(tilePos - 0.01f, tilePos + 1.01f);
					RemQueue<uint> toRemove = default(RemQueue<uint>);
					foreach (KeyValuePair<uint, Decal> keyValuePair in oldChunk.Decals)
					{
						uint nextDecalId;
						Decal decal2;
						keyValuePair.Deconstruct(out nextDecalId, out decal2);
						uint oldDecalId = nextDecalId;
						Decal decal = decal2;
						if (bounds.Contains(decal.Coordinates, true))
						{
							DecalGridComponent.DecalGridChunkCollection chunkCollection2 = newComp.ChunkCollection;
							nextDecalId = chunkCollection2.NextDecalId;
							chunkCollection2.NextDecalId = nextDecalId + 1U;
							uint newDecalId = nextDecalId;
							Extensions.GetOrNew<Vector2i, DecalGridComponent.DecalChunk>(chunkCollection, chunkIndices).Decals[newDecalId] = decal;
							newComp.DecalIndex[newDecalId] = chunkIndices;
							toRemove.Add(oldDecalId);
						}
					}
					foreach (uint oldDecalId2 in toRemove)
					{
						oldChunk.Decals.Remove(oldDecalId2);
						oldComp.DecalIndex.Remove(oldDecalId2);
					}
					this.DirtyChunk(ev.Grid, chunkIndices, Extensions.GetOrNew<Vector2i, DecalGridComponent.DecalChunk>(chunkCollection, chunkIndices));
					if (oldChunk.Decals.Count == 0)
					{
						oldChunkCollection.Remove(chunkIndices);
					}
					List<uint> list = toRemove.List;
					if (list != null && list.Count > 0)
					{
						this.DirtyChunk(ev.OldGrid, chunkIndices, oldChunk);
					}
				}
			}
		}

		// Token: 0x06001E34 RID: 7732 RVA: 0x0009FF64 File Offset: 0x0009E164
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.OnPlayerStatusChanged;
			this._conf.UnsubValueChanged<bool>(CVars.NetPVS, new Action<bool>(this.OnPvsToggle));
		}

		// Token: 0x06001E35 RID: 7733 RVA: 0x0009FFA0 File Offset: 0x0009E1A0
		private void OnTileChanged(ref TileChangedEvent args)
		{
			if (!args.NewTile.IsSpace(this._tileDefMan))
			{
				return;
			}
			DecalGridComponent grid;
			if (!base.TryComp<DecalGridComponent>(args.Entity, ref grid))
			{
				return;
			}
			Vector2i indices = SharedDecalSystem.GetChunkIndices(args.NewTile.GridIndices);
			HashSet<uint> toDelete = new HashSet<uint>();
			DecalGridComponent.DecalChunk chunk;
			if (!grid.ChunkCollection.ChunkCollection.TryGetValue(indices, out chunk))
			{
				return;
			}
			foreach (KeyValuePair<uint, Decal> keyValuePair in chunk.Decals)
			{
				uint num;
				Decal decal2;
				keyValuePair.Deconstruct(out num, out decal2);
				uint uid = num;
				Decal decal = decal2;
				if (new Vector2((float)((int)Math.Floor((double)decal.Coordinates.X)), (float)((int)Math.Floor((double)decal.Coordinates.Y))) == args.NewTile.GridIndices)
				{
					toDelete.Add(uid);
				}
			}
			if (toDelete.Count == 0)
			{
				return;
			}
			foreach (uint decalId in toDelete)
			{
				grid.DecalIndex.Remove(decalId);
				chunk.Decals.Remove(decalId);
			}
			this.DirtyChunk(args.Entity, indices, chunk);
			if (chunk.Decals.Count == 0)
			{
				grid.ChunkCollection.ChunkCollection.Remove(indices);
			}
		}

		// Token: 0x06001E36 RID: 7734 RVA: 0x000A0130 File Offset: 0x0009E330
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			SessionStatus newStatus = e.NewStatus;
			if (newStatus == 3)
			{
				this._previousSentChunks[e.Session] = new Dictionary<EntityUid, HashSet<Vector2i>>();
				return;
			}
			if (newStatus != 4)
			{
				return;
			}
			this._previousSentChunks.Remove(e.Session);
		}

		// Token: 0x06001E37 RID: 7735 RVA: 0x000A0178 File Offset: 0x0009E378
		private void OnDecalPlacementRequest(RequestDecalPlacementEvent ev, EntitySessionEventArgs eventArgs)
		{
			IPlayerSession session = eventArgs.SenderSession as IPlayerSession;
			if (session == null)
			{
				return;
			}
			if (!this._adminManager.HasAdminFlag(session, AdminFlags.Spawn))
			{
				return;
			}
			if (!ev.Coordinates.IsValid(this.EntityManager))
			{
				return;
			}
			uint num;
			this.TryAddDecal(ev.Decal, ev.Coordinates, out num);
		}

		// Token: 0x06001E38 RID: 7736 RVA: 0x000A01D0 File Offset: 0x0009E3D0
		private void OnDecalRemovalRequest(RequestDecalRemovalEvent ev, EntitySessionEventArgs eventArgs)
		{
			IPlayerSession session = eventArgs.SenderSession as IPlayerSession;
			if (session == null)
			{
				return;
			}
			if (!this._adminManager.HasAdminFlag(session, AdminFlags.Spawn))
			{
				return;
			}
			if (!ev.Coordinates.IsValid(this.EntityManager))
			{
				return;
			}
			EntityUid? gridId = ev.Coordinates.GetGridUid(this.EntityManager);
			if (gridId == null)
			{
				return;
			}
			foreach (ValueTuple<uint, Decal> valueTuple in this.GetDecalsInRange(gridId.Value, ev.Coordinates.Position, 0.75f, null))
			{
				uint decalId = valueTuple.Item1;
				this.RemoveDecal(gridId.Value, decalId, null);
			}
		}

		// Token: 0x06001E39 RID: 7737 RVA: 0x000A029C File Offset: 0x0009E49C
		protected override void DirtyChunk(EntityUid id, Vector2i chunkIndices, DecalGridComponent.DecalChunk chunk)
		{
			chunk.LastModified = this._timing.CurTick;
			if (!this._dirtyChunks.ContainsKey(id))
			{
				this._dirtyChunks[id] = new HashSet<Vector2i>();
			}
			this._dirtyChunks[id].Add(chunkIndices);
		}

		// Token: 0x06001E3A RID: 7738 RVA: 0x000A02EC File Offset: 0x0009E4EC
		public bool TryAddDecal(string id, EntityCoordinates coordinates, out uint decalId, Color? color = null, Angle? rotation = null, int zIndex = 0, bool cleanable = false)
		{
			Angle value = rotation.GetValueOrDefault();
			if (rotation == null)
			{
				value = Angle.Zero;
				rotation = new Angle?(value);
			}
			Decal decal = new Decal(coordinates.Position, id, color, rotation.Value, zIndex, cleanable);
			return this.TryAddDecal(decal, coordinates, out decalId);
		}

		// Token: 0x06001E3B RID: 7739 RVA: 0x000A033C File Offset: 0x0009E53C
		public bool TryAddDecal(Decal decal, EntityCoordinates coordinates, out uint decalId)
		{
			decalId = 0U;
			if (!this.PrototypeManager.HasIndex<DecalPrototype>(decal.Id))
			{
				return false;
			}
			EntityUid? gridId = coordinates.GetGridUid(this.EntityManager);
			MapGridComponent grid;
			if (!this.MapManager.TryGetGrid(gridId, ref grid))
			{
				return false;
			}
			if (grid.GetTileRef(coordinates).IsSpace(this._tileDefMan))
			{
				return false;
			}
			DecalGridComponent comp;
			if (!base.TryComp<DecalGridComponent>(gridId, ref comp))
			{
				return false;
			}
			DecalGridComponent.DecalGridChunkCollection chunkCollection = comp.ChunkCollection;
			uint nextDecalId = chunkCollection.NextDecalId;
			chunkCollection.NextDecalId = nextDecalId + 1U;
			decalId = nextDecalId;
			Vector2i chunkIndices = SharedDecalSystem.GetChunkIndices(decal.Coordinates);
			DecalGridComponent.DecalChunk chunk = Extensions.GetOrNew<Vector2i, DecalGridComponent.DecalChunk>(comp.ChunkCollection.ChunkCollection, chunkIndices);
			chunk.Decals[decalId] = decal;
			comp.DecalIndex[decalId] = chunkIndices;
			this.DirtyChunk(gridId.Value, chunkIndices, chunk);
			return true;
		}

		// Token: 0x06001E3C RID: 7740 RVA: 0x000A040C File Offset: 0x0009E60C
		[NullableContext(2)]
		public bool RemoveDecal(EntityUid gridId, uint decalId, DecalGridComponent component = null)
		{
			Decal decal;
			return base.RemoveDecalInternal(gridId, decalId, out decal, component);
		}

		// Token: 0x06001E3D RID: 7741 RVA: 0x000A0424 File Offset: 0x0009E624
		[return: TupleElementNames(new string[]
		{
			"Index",
			"Decal"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public HashSet<ValueTuple<uint, Decal>> GetDecalsInRange(EntityUid gridId, Vector2 position, float distance = 0.75f, [Nullable(new byte[]
		{
			2,
			1
		})] Func<Decal, bool> validDelegate = null)
		{
			HashSet<ValueTuple<uint, Decal>> decalIds = new HashSet<ValueTuple<uint, Decal>>();
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunkCollection = base.ChunkCollection(gridId, null);
			Vector2i chunkIndices = SharedDecalSystem.GetChunkIndices(position);
			DecalGridComponent.DecalChunk chunk;
			if (chunkCollection == null || !chunkCollection.TryGetValue(chunkIndices, out chunk))
			{
				return decalIds;
			}
			foreach (KeyValuePair<uint, Decal> keyValuePair in chunk.Decals)
			{
				uint num;
				Decal decal2;
				keyValuePair.Deconstruct(out num, out decal2);
				uint uid = num;
				Decal decal = decal2;
				if ((position - decal.Coordinates - new Vector2(0.5f, 0.5f)).Length <= distance && (validDelegate == null || validDelegate(decal)))
				{
					decalIds.Add(new ValueTuple<uint, Decal>(uid, decal));
				}
			}
			return decalIds;
		}

		// Token: 0x06001E3E RID: 7742 RVA: 0x000A04F8 File Offset: 0x0009E6F8
		[NullableContext(2)]
		public bool SetDecalPosition(EntityUid gridId, uint decalId, EntityCoordinates coordinates, DecalGridComponent comp = null)
		{
			Decal removed;
			uint num;
			return base.Resolve<DecalGridComponent>(gridId, ref comp, true) && base.RemoveDecalInternal(gridId, decalId, out removed, comp) && this.TryAddDecal(removed.WithCoordinates(coordinates.Position), coordinates, out num);
		}

		// Token: 0x06001E3F RID: 7743 RVA: 0x000A0538 File Offset: 0x0009E738
		private bool ModifyDecal(EntityUid gridId, uint decalId, Func<Decal, Decal> modifyDecal, [Nullable(2)] DecalGridComponent comp = null)
		{
			if (!base.Resolve<DecalGridComponent>(gridId, ref comp, true))
			{
				return false;
			}
			Vector2i indices;
			if (!comp.DecalIndex.TryGetValue(decalId, out indices))
			{
				return false;
			}
			DecalGridComponent.DecalChunk chunk = comp.ChunkCollection.ChunkCollection[indices];
			Decal decal = chunk.Decals[decalId];
			chunk.Decals[decalId] = modifyDecal(decal);
			this.DirtyChunk(gridId, indices, chunk);
			return true;
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x000A05A4 File Offset: 0x0009E7A4
		[NullableContext(2)]
		public bool SetDecalColor(EntityUid gridId, uint decalId, Color? value, DecalGridComponent comp = null)
		{
			return this.ModifyDecal(gridId, decalId, (Decal x) => x.WithColor(value), comp);
		}

		// Token: 0x06001E41 RID: 7745 RVA: 0x000A05D4 File Offset: 0x0009E7D4
		[NullableContext(2)]
		public bool SetDecalRotation(EntityUid gridId, uint decalId, Angle value, DecalGridComponent comp = null)
		{
			return this.ModifyDecal(gridId, decalId, (Decal x) => x.WithRotation(value), comp);
		}

		// Token: 0x06001E42 RID: 7746 RVA: 0x000A0604 File Offset: 0x0009E804
		[NullableContext(2)]
		public bool SetDecalZIndex(EntityUid gridId, uint decalId, int value, DecalGridComponent comp = null)
		{
			return this.ModifyDecal(gridId, decalId, (Decal x) => x.WithZIndex(value), comp);
		}

		// Token: 0x06001E43 RID: 7747 RVA: 0x000A0634 File Offset: 0x0009E834
		[NullableContext(2)]
		public bool SetDecalCleanable(EntityUid gridId, uint decalId, bool value, DecalGridComponent comp = null)
		{
			return this.ModifyDecal(gridId, decalId, (Decal x) => x.WithCleanable(value), comp);
		}

		// Token: 0x06001E44 RID: 7748 RVA: 0x000A0664 File Offset: 0x0009E864
		public bool SetDecalId(EntityUid gridId, uint decalId, string id, [Nullable(2)] DecalGridComponent comp = null)
		{
			if (!this.PrototypeManager.HasIndex<DecalPrototype>(id))
			{
				throw new ArgumentOutOfRangeException("Tried to set decal id to invalid prototypeid: " + id);
			}
			return this.ModifyDecal(gridId, decalId, (Decal x) => x.WithId(id), comp);
		}

		// Token: 0x06001E45 RID: 7749 RVA: 0x000A06C0 File Offset: 0x0009E8C0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (EntityUid ent in this._dirtyChunks.Keys)
			{
				DecalGridComponent decals;
				if (base.TryComp<DecalGridComponent>(ent, ref decals))
				{
					base.Dirty(decals, null);
				}
			}
			if (!this._pvsEnabled)
			{
				this._dirtyChunks.Clear();
				return;
			}
			if (this._pvsEnabled)
			{
				IEnumerable<IPlayerSession> source = (from x in this._playerManager.ServerSessions
				where x.Status == 3
				select x).ToArray<IPlayerSession>();
				ParallelOptions opts = new ParallelOptions
				{
					MaxDegreeOfParallelism = this._parMan.ParallelProcessCount
				};
				Parallel.ForEach<IPlayerSession>(source, opts, new Action<IPlayerSession>(this.UpdatePlayer));
			}
			this._dirtyChunks.Clear();
		}

		// Token: 0x06001E46 RID: 7750 RVA: 0x000A07B0 File Offset: 0x0009E9B0
		public void UpdatePlayer(IPlayerSession player)
		{
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			Dictionary<EntityUid, HashSet<Vector2i>> chunksInRange = this._chunking.GetChunksForSession(player, 32, xformQuery, this._chunkIndexPool, this._chunkViewerPool, null);
			Dictionary<EntityUid, HashSet<Vector2i>> staleChunks = this._chunkViewerPool.Get();
			Dictionary<EntityUid, HashSet<Vector2i>> previouslySent = this._previousSentChunks[player];
			foreach (KeyValuePair<EntityUid, HashSet<Vector2i>> keyValuePair in previouslySent)
			{
				EntityUid entityUid;
				HashSet<Vector2i> hashSet;
				keyValuePair.Deconstruct(out entityUid, out hashSet);
				EntityUid gridId = entityUid;
				HashSet<Vector2i> oldIndices = hashSet;
				HashSet<Vector2i> chunks;
				if (!chunksInRange.TryGetValue(gridId, out chunks))
				{
					previouslySent.Remove(gridId);
					if (this.MapManager.IsGrid(gridId))
					{
						staleChunks[gridId] = oldIndices;
					}
					else
					{
						oldIndices.Clear();
						this._chunkIndexPool.Return(oldIndices);
					}
				}
				else
				{
					HashSet<Vector2i> elmo = this._chunkIndexPool.Get();
					foreach (Vector2i chunk in oldIndices)
					{
						if (!chunks.Contains(chunk))
						{
							elmo.Add(chunk);
						}
					}
					if (elmo.Count == 0)
					{
						this._chunkIndexPool.Return(elmo);
					}
					else
					{
						staleChunks.Add(gridId, elmo);
					}
				}
			}
			Dictionary<EntityUid, HashSet<Vector2i>> updatedChunks = this._chunkViewerPool.Get();
			foreach (KeyValuePair<EntityUid, HashSet<Vector2i>> keyValuePair in chunksInRange)
			{
				EntityUid entityUid;
				HashSet<Vector2i> hashSet;
				keyValuePair.Deconstruct(out entityUid, out hashSet);
				EntityUid gridId2 = entityUid;
				HashSet<Vector2i> gridChunks = hashSet;
				HashSet<Vector2i> newChunks = this._chunkIndexPool.Get();
				HashSet<Vector2i> dirtyChunks;
				this._dirtyChunks.TryGetValue(gridId2, out dirtyChunks);
				HashSet<Vector2i> previousChunks;
				if (!previouslySent.TryGetValue(gridId2, out previousChunks))
				{
					newChunks.UnionWith(gridChunks);
				}
				else
				{
					foreach (Vector2i index in gridChunks)
					{
						if (!previousChunks.Contains(index) || (dirtyChunks != null && dirtyChunks.Contains(index)))
						{
							newChunks.Add(index);
						}
					}
					previousChunks.Clear();
					this._chunkIndexPool.Return(previousChunks);
				}
				previouslySent[gridId2] = gridChunks;
				if (newChunks.Count == 0)
				{
					this._chunkIndexPool.Return(newChunks);
				}
				else
				{
					updatedChunks[gridId2] = newChunks;
				}
			}
			this.SendChunkUpdates(player, updatedChunks, staleChunks);
		}

		// Token: 0x06001E47 RID: 7751 RVA: 0x000A0A58 File Offset: 0x0009EC58
		private void ReturnToPool(Dictionary<EntityUid, HashSet<Vector2i>> chunks)
		{
			foreach (KeyValuePair<EntityUid, HashSet<Vector2i>> keyValuePair in chunks)
			{
				EntityUid entityUid;
				HashSet<Vector2i> hashSet;
				keyValuePair.Deconstruct(out entityUid, out hashSet);
				HashSet<Vector2i> previous = hashSet;
				previous.Clear();
				this._chunkIndexPool.Return(previous);
			}
			chunks.Clear();
			this._chunkViewerPool.Return(chunks);
		}

		// Token: 0x06001E48 RID: 7752 RVA: 0x000A0AD4 File Offset: 0x0009ECD4
		private void SendChunkUpdates(IPlayerSession session, Dictionary<EntityUid, HashSet<Vector2i>> updatedChunks, Dictionary<EntityUid, HashSet<Vector2i>> staleChunks)
		{
			Dictionary<EntityUid, Dictionary<Vector2i, DecalGridComponent.DecalChunk>> updatedDecals = new Dictionary<EntityUid, Dictionary<Vector2i, DecalGridComponent.DecalChunk>>();
			foreach (KeyValuePair<EntityUid, HashSet<Vector2i>> keyValuePair in updatedChunks)
			{
				EntityUid entityUid;
				HashSet<Vector2i> hashSet;
				keyValuePair.Deconstruct(out entityUid, out hashSet);
				EntityUid gridId = entityUid;
				HashSet<Vector2i> chunks = hashSet;
				Dictionary<Vector2i, DecalGridComponent.DecalChunk> collection = base.ChunkCollection(gridId, null);
				if (collection != null)
				{
					Dictionary<Vector2i, DecalGridComponent.DecalChunk> gridChunks = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>();
					foreach (Vector2i indices in chunks)
					{
						DecalGridComponent.DecalChunk chunk;
						gridChunks.Add(indices, collection.TryGetValue(indices, out chunk) ? chunk : new DecalGridComponent.DecalChunk());
					}
					updatedDecals[gridId] = gridChunks;
				}
			}
			if (updatedDecals.Count != 0 || staleChunks.Count != 0)
			{
				base.RaiseNetworkEvent(new DecalChunkUpdateEvent
				{
					Data = updatedDecals,
					RemovedChunks = staleChunks
				}, session);
			}
			this.ReturnToPool(updatedChunks);
			this.ReturnToPool(staleChunks);
		}

		// Token: 0x0400134A RID: 4938
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400134B RID: 4939
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x0400134C RID: 4940
		[Dependency]
		private readonly ITileDefinitionManager _tileDefMan;

		// Token: 0x0400134D RID: 4941
		[Dependency]
		private readonly IParallelManager _parMan;

		// Token: 0x0400134E RID: 4942
		[Dependency]
		private readonly ChunkingSystem _chunking;

		// Token: 0x0400134F RID: 4943
		[Dependency]
		private readonly IConfigurationManager _conf;

		// Token: 0x04001350 RID: 4944
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04001351 RID: 4945
		private readonly Dictionary<EntityUid, HashSet<Vector2i>> _dirtyChunks = new Dictionary<EntityUid, HashSet<Vector2i>>();

		// Token: 0x04001352 RID: 4946
		private readonly Dictionary<IPlayerSession, Dictionary<EntityUid, HashSet<Vector2i>>> _previousSentChunks = new Dictionary<IPlayerSession, Dictionary<EntityUid, HashSet<Vector2i>>>();

		// Token: 0x04001353 RID: 4947
		private ObjectPool<HashSet<Vector2i>> _chunkIndexPool = new DefaultObjectPool<HashSet<Vector2i>>(new DefaultPooledObjectPolicy<HashSet<Vector2i>>(), 64);

		// Token: 0x04001354 RID: 4948
		private ObjectPool<Dictionary<EntityUid, HashSet<Vector2i>>> _chunkViewerPool = new DefaultObjectPool<Dictionary<EntityUid, HashSet<Vector2i>>>(new DefaultPooledObjectPolicy<Dictionary<EntityUid, HashSet<Vector2i>>>(), 64);

		// Token: 0x04001355 RID: 4949
		private bool _pvsEnabled;
	}
}
