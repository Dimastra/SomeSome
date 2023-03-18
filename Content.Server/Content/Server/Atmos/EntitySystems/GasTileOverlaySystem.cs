using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Atmos.Components;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.CCVar;
using Content.Shared.Chunking;
using Content.Shared.GameTicking;
using Content.Shared.Rounding;
using Microsoft.Extensions.ObjectPool;
using Robust.Server.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Threading;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x0200079D RID: 1949
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class GasTileOverlaySystem : SharedGasTileOverlaySystem
	{
		// Token: 0x06002A44 RID: 10820 RVA: 0x000DEEF4 File Offset: 0x000DD0F4
		public override void Initialize()
		{
			base.Initialize();
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
			this._confMan.OnValueChanged<float>(CCVars.NetGasOverlayTickRate, new Action<float>(this.UpdateTickRate), true);
			this._confMan.OnValueChanged<int>(CCVars.GasOverlayThresholds, new Action<int>(this.UpdateThresholds), true);
			this._confMan.OnValueChanged<bool>(CVars.NetPVS, new Action<bool>(this.OnPvsToggle), true);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<GasTileOverlayComponent, ComponentGetState>(new ComponentEventRefHandler<GasTileOverlayComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x06002A45 RID: 10821 RVA: 0x000DEFA0 File Offset: 0x000DD1A0
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.OnPlayerStatusChanged;
			this._confMan.UnsubValueChanged<float>(CCVars.NetGasOverlayTickRate, new Action<float>(this.UpdateTickRate));
			this._confMan.UnsubValueChanged<int>(CCVars.GasOverlayThresholds, new Action<int>(this.UpdateThresholds));
			this._confMan.UnsubValueChanged<bool>(CVars.NetPVS, new Action<bool>(this.OnPvsToggle));
		}

		// Token: 0x06002A46 RID: 10822 RVA: 0x000DF020 File Offset: 0x000DD220
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
			foreach (Dictionary<EntityUid, HashSet<Vector2i>> lastSent in this._lastSentChunks.Values)
			{
				foreach (HashSet<Vector2i> set in lastSent.Values)
				{
					set.Clear();
					this._chunkIndexPool.Return(set);
				}
				lastSent.Clear();
			}
			foreach (ValueTuple<GasTileOverlayComponent, MetaDataComponent> valueTuple in base.EntityQuery<GasTileOverlayComponent, MetaDataComponent>(true))
			{
				GasTileOverlayComponent grid = valueTuple.Item1;
				MetaDataComponent meta = valueTuple.Item2;
				grid.ForceTick = this._gameTiming.CurTick;
				base.Dirty(grid, meta);
			}
		}

		// Token: 0x06002A47 RID: 10823 RVA: 0x000DF140 File Offset: 0x000DD340
		private void UpdateTickRate(float value)
		{
			this._updateInterval = ((value > 0f) ? (1f / value) : float.MaxValue);
		}

		// Token: 0x06002A48 RID: 10824 RVA: 0x000DF15E File Offset: 0x000DD35E
		private void UpdateThresholds(int value)
		{
			this._thresholds = value;
		}

		// Token: 0x06002A49 RID: 10825 RVA: 0x000DF167 File Offset: 0x000DD367
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invalidate(EntityUid grid, Vector2i index, GasTileOverlayComponent comp = null)
		{
			if (base.Resolve<GasTileOverlayComponent>(grid, ref comp, true))
			{
				comp.InvalidTiles.Add(index);
			}
		}

		// Token: 0x06002A4A RID: 10826 RVA: 0x000DF184 File Offset: 0x000DD384
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			Dictionary<EntityUid, HashSet<Vector2i>> sets;
			if (e.NewStatus != 3 && this._lastSentChunks.Remove(e.Session, out sets))
			{
				foreach (HashSet<Vector2i> set in sets.Values)
				{
					set.Clear();
					this._chunkIndexPool.Return(set);
				}
			}
			if (!this._lastSentChunks.ContainsKey(e.Session))
			{
				this._lastSentChunks[e.Session] = new Dictionary<EntityUid, HashSet<Vector2i>>();
			}
		}

		// Token: 0x06002A4B RID: 10827 RVA: 0x000DF22C File Offset: 0x000DD42C
		private bool UpdateChunkTile(GridAtmosphereComponent gridAtmosphere, GasOverlayChunk chunk, Vector2i index, GameTick curTick)
		{
			ref SharedGasTileOverlaySystem.GasOverlayData oldData = ref chunk.GetData(index);
			TileAtmosphere tile;
			if (!gridAtmosphere.Tiles.TryGetValue(index, out tile))
			{
				if (oldData.Equals(default(SharedGasTileOverlaySystem.GasOverlayData)))
				{
					return false;
				}
				chunk.LastUpdate = curTick;
				oldData = default(SharedGasTileOverlaySystem.GasOverlayData);
				return true;
			}
			else
			{
				bool changed = false;
				if (oldData.Equals(default(SharedGasTileOverlaySystem.GasOverlayData)))
				{
					changed = true;
					oldData = new SharedGasTileOverlaySystem.GasOverlayData(tile.Hotspot.State, new byte[this.VisibleGasId.Length]);
				}
				else if (oldData.FireState != tile.Hotspot.State)
				{
					changed = true;
					oldData = new SharedGasTileOverlaySystem.GasOverlayData(tile.Hotspot.State, oldData.Opacity);
				}
				if (tile.Air != null)
				{
					for (int i = 0; i < this.VisibleGasId.Length; i++)
					{
						int id = this.VisibleGasId[i];
						GasPrototype gas = this._atmosphereSystem.GetGas(id);
						float moles = tile.Air.Moles[id];
						ref byte oldOpacity = ref oldData.Opacity[i];
						if (moles < gas.GasMolesVisible)
						{
							if (oldOpacity != 0)
							{
								oldOpacity = 0;
								changed = true;
							}
						}
						else
						{
							byte opacity = (byte)(ContentHelpers.RoundToLevels((double)(MathHelper.Clamp01((moles - gas.GasMolesVisible) / (gas.GasMolesVisibleMax - gas.GasMolesVisible)) * 255f), 255.0, this._thresholds) * 255 / (this._thresholds - 1));
							if (oldOpacity != opacity)
							{
								oldOpacity = opacity;
								changed = true;
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < this.VisibleGasId.Length; j++)
					{
						changed |= (oldData.Opacity[j] > 0);
						oldData.Opacity[j] = 0;
					}
				}
				if (!changed)
				{
					return false;
				}
				chunk.LastUpdate = curTick;
				return true;
			}
		}

		// Token: 0x06002A4C RID: 10828 RVA: 0x000DF3F4 File Offset: 0x000DD5F4
		private void UpdateOverlayData(GameTick curTick)
		{
			foreach (ValueTuple<GasTileOverlayComponent, GridAtmosphereComponent, MetaDataComponent> valueTuple in base.EntityQuery<GasTileOverlayComponent, GridAtmosphereComponent, MetaDataComponent>(true))
			{
				GasTileOverlayComponent overlay = valueTuple.Item1;
				GridAtmosphereComponent gam = valueTuple.Item2;
				MetaDataComponent meta = valueTuple.Item3;
				bool changed = false;
				foreach (Vector2i index in overlay.InvalidTiles)
				{
					Vector2i chunkIndex = SharedGasTileOverlaySystem.GetGasChunkIndices(index);
					GasOverlayChunk chunk;
					if (!overlay.Chunks.TryGetValue(chunkIndex, out chunk))
					{
						chunk = (overlay.Chunks[chunkIndex] = new GasOverlayChunk(chunkIndex));
					}
					changed |= this.UpdateChunkTile(gam, chunk, index, curTick);
				}
				if (changed)
				{
					base.Dirty(overlay, meta);
				}
				overlay.InvalidTiles.Clear();
			}
		}

		// Token: 0x06002A4D RID: 10829 RVA: 0x000DF4F0 File Offset: 0x000DD6F0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.AccumulatedFrameTime += frameTime;
			if (this.AccumulatedFrameTime < this._updateInterval)
			{
				return;
			}
			this.AccumulatedFrameTime -= this._updateInterval;
			GameTick curTick = this._gameTiming.CurTick;
			this.UpdateOverlayData(curTick);
			if (!this._pvsEnabled)
			{
				return;
			}
			IEnumerable<IPlayerSession> source = (from x in this._playerManager.ServerSessions
			where x.Status == 3
			select x).ToArray<IPlayerSession>();
			ParallelOptions opts = new ParallelOptions
			{
				MaxDegreeOfParallelism = this._parMan.ParallelProcessCount
			};
			Parallel.ForEach<IPlayerSession>(source, opts, delegate(IPlayerSession p)
			{
				this.UpdatePlayer(p, curTick);
			});
		}

		// Token: 0x06002A4E RID: 10830 RVA: 0x000DF5C8 File Offset: 0x000DD7C8
		private void UpdatePlayer(IPlayerSession playerSession, GameTick curTick)
		{
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			Dictionary<EntityUid, HashSet<Vector2i>> chunksInRange = this._chunkingSys.GetChunksForSession(playerSession, 8, xformQuery, this._chunkIndexPool, this._chunkViewerPool, null);
			Dictionary<EntityUid, HashSet<Vector2i>> previouslySent = this._lastSentChunks[playerSession];
			SharedGasTileOverlaySystem.GasOverlayUpdateEvent ev = new SharedGasTileOverlaySystem.GasOverlayUpdateEvent();
			foreach (KeyValuePair<EntityUid, HashSet<Vector2i>> keyValuePair in previouslySent)
			{
				EntityUid entityUid;
				HashSet<Vector2i> hashSet;
				keyValuePair.Deconstruct(out entityUid, out hashSet);
				EntityUid grid = entityUid;
				HashSet<Vector2i> oldIndices = hashSet;
				HashSet<Vector2i> chunks;
				if (!chunksInRange.TryGetValue(grid, out chunks))
				{
					previouslySent.Remove(grid);
					if (this._mapManager.IsGrid(grid))
					{
						ev.RemovedChunks[grid] = oldIndices;
					}
					else
					{
						oldIndices.Clear();
						this._chunkIndexPool.Return(oldIndices);
					}
				}
				else
				{
					HashSet<Vector2i> old = this._chunkIndexPool.Get();
					foreach (Vector2i chunk in oldIndices)
					{
						if (!chunks.Contains(chunk))
						{
							old.Add(chunk);
						}
					}
					if (old.Count == 0)
					{
						this._chunkIndexPool.Return(old);
					}
					else
					{
						ev.RemovedChunks.Add(grid, old);
					}
				}
			}
			foreach (KeyValuePair<EntityUid, HashSet<Vector2i>> keyValuePair in chunksInRange)
			{
				EntityUid entityUid;
				HashSet<Vector2i> hashSet;
				keyValuePair.Deconstruct(out entityUid, out hashSet);
				EntityUid grid2 = entityUid;
				HashSet<Vector2i> gridChunks = hashSet;
				GasTileOverlayComponent overlay;
				if (!base.TryComp<GasTileOverlayComponent>(grid2, ref overlay))
				{
					return;
				}
				List<GasOverlayChunk> dataToSend = new List<GasOverlayChunk>();
				ev.UpdatedChunks[grid2] = dataToSend;
				HashSet<Vector2i> previousChunks;
				previouslySent.TryGetValue(grid2, out previousChunks);
				foreach (Vector2i index in gridChunks)
				{
					GasOverlayChunk value;
					if (overlay.Chunks.TryGetValue(index, out value) && (previousChunks == null || !previousChunks.Contains(index) || !(value.LastUpdate != curTick)))
					{
						dataToSend.Add(value);
					}
				}
				previouslySent[grid2] = gridChunks;
				if (previousChunks != null)
				{
					previousChunks.Clear();
					this._chunkIndexPool.Return(previousChunks);
				}
			}
			if (ev.UpdatedChunks.Count != 0 || ev.RemovedChunks.Count != 0)
			{
				base.RaiseNetworkEvent(ev, playerSession.ConnectedClient);
			}
		}

		// Token: 0x06002A4F RID: 10831 RVA: 0x000DF878 File Offset: 0x000DDA78
		public void Reset(RoundRestartCleanupEvent ev)
		{
			foreach (Dictionary<EntityUid, HashSet<Vector2i>> data in this._lastSentChunks.Values)
			{
				foreach (HashSet<Vector2i> previous in data.Values)
				{
					previous.Clear();
					this._chunkIndexPool.Return(previous);
				}
				data.Clear();
			}
		}

		// Token: 0x06002A50 RID: 10832 RVA: 0x000DF91C File Offset: 0x000DDB1C
		private void OnGetState(EntityUid uid, GasTileOverlayComponent component, ref ComponentGetState args)
		{
			if (this._pvsEnabled && !args.ReplayState)
			{
				return;
			}
			if (args.FromTick <= component.CreationTick || args.FromTick <= component.ForceTick)
			{
				args.State = new GasTileOverlayState(component.Chunks);
				return;
			}
			Dictionary<Vector2i, GasOverlayChunk> data = new Dictionary<Vector2i, GasOverlayChunk>();
			foreach (KeyValuePair<Vector2i, GasOverlayChunk> keyValuePair in component.Chunks)
			{
				Vector2i vector2i;
				GasOverlayChunk gasOverlayChunk;
				keyValuePair.Deconstruct(out vector2i, out gasOverlayChunk);
				Vector2i index = vector2i;
				GasOverlayChunk chunk = gasOverlayChunk;
				if (chunk.LastUpdate >= args.FromTick)
				{
					data[index] = chunk;
				}
			}
			args.State = new GasTileOverlayState(data)
			{
				AllChunks = new HashSet<Vector2i>(component.Chunks.Keys)
			};
		}

		// Token: 0x04001A1C RID: 6684
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001A1D RID: 6685
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001A1E RID: 6686
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001A1F RID: 6687
		[Dependency]
		private readonly IConfigurationManager _confMan;

		// Token: 0x04001A20 RID: 6688
		[Dependency]
		private readonly IParallelManager _parMan;

		// Token: 0x04001A21 RID: 6689
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001A22 RID: 6690
		[Dependency]
		private readonly ChunkingSystem _chunkingSys;

		// Token: 0x04001A23 RID: 6691
		private readonly Dictionary<IPlayerSession, Dictionary<EntityUid, HashSet<Vector2i>>> _lastSentChunks = new Dictionary<IPlayerSession, Dictionary<EntityUid, HashSet<Vector2i>>>();

		// Token: 0x04001A24 RID: 6692
		private ObjectPool<HashSet<Vector2i>> _chunkIndexPool = new DefaultObjectPool<HashSet<Vector2i>>(new DefaultPooledObjectPolicy<HashSet<Vector2i>>(), 64);

		// Token: 0x04001A25 RID: 6693
		private ObjectPool<Dictionary<EntityUid, HashSet<Vector2i>>> _chunkViewerPool = new DefaultObjectPool<Dictionary<EntityUid, HashSet<Vector2i>>>(new DefaultPooledObjectPolicy<Dictionary<EntityUid, HashSet<Vector2i>>>(), 64);

		// Token: 0x04001A26 RID: 6694
		private float _updateInterval;

		// Token: 0x04001A27 RID: 6695
		private int _thresholds;

		// Token: 0x04001A28 RID: 6696
		private bool _pvsEnabled;
	}
}
