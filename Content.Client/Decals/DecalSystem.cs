using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Decals.Overlays;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Decals
{
	// Token: 0x0200035B RID: 859
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DecalSystem : SharedDecalSystem
	{
		// Token: 0x06001539 RID: 5433 RVA: 0x0007CBF4 File Offset: 0x0007ADF4
		public override void Initialize()
		{
			base.Initialize();
			this._overlay = new DecalOverlay(this._sprites, this.EntityManager, this.PrototypeManager);
			this._overlayManager.AddOverlay(this._overlay);
			base.SubscribeLocalEvent<DecalGridComponent, ComponentHandleState>(new ComponentEventRefHandler<DecalGridComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeNetworkEvent<DecalChunkUpdateEvent>(new EntityEventHandler<DecalChunkUpdateEvent>(this.OnChunkUpdate), null, null);
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x0007CC5E File Offset: 0x0007AE5E
		public void ToggleOverlay()
		{
			if (this._overlayManager.HasOverlay<DecalOverlay>())
			{
				this._overlayManager.RemoveOverlay(this._overlay);
				return;
			}
			this._overlayManager.AddOverlay(this._overlay);
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x0007CC92 File Offset: 0x0007AE92
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlayManager.RemoveOverlay(this._overlay);
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x0007CCAC File Offset: 0x0007AEAC
		protected override void OnDecalRemoved(EntityUid gridId, uint decalId, DecalGridComponent component, Vector2i indices, DecalGridComponent.DecalChunk chunk)
		{
			base.OnDecalRemoved(gridId, decalId, component, indices, chunk);
			int key;
			if (!component.DecalZIndexIndex.Remove(decalId, out key))
			{
				return;
			}
			SortedDictionary<uint, Decal> sortedDictionary;
			if (!component.DecalRenderIndex.TryGetValue(key, out sortedDictionary))
			{
				return;
			}
			sortedDictionary.Remove(decalId);
			if (sortedDictionary.Count == 0)
			{
				component.DecalRenderIndex.Remove(key);
			}
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x0007CD08 File Offset: 0x0007AF08
		private void OnHandleState(EntityUid gridUid, DecalGridComponent gridComp, ref ComponentHandleState args)
		{
			DecalGridState decalGridState = args.Current as DecalGridState;
			if (decalGridState == null)
			{
				return;
			}
			List<Vector2i> list = new List<Vector2i>();
			if (!decalGridState.FullState)
			{
				using (Dictionary<Vector2i, DecalGridComponent.DecalChunk>.KeyCollection.Enumerator enumerator = gridComp.ChunkCollection.ChunkCollection.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Vector2i item = enumerator.Current;
						if (!decalGridState.AllChunks.Contains(item))
						{
							list.Add(item);
						}
					}
					goto IL_BD;
				}
			}
			foreach (Vector2i vector2i in gridComp.ChunkCollection.ChunkCollection.Keys)
			{
				if (!decalGridState.Chunks.ContainsKey(vector2i))
				{
					list.Add(vector2i);
				}
			}
			IL_BD:
			if (list.Count > 0)
			{
				this.RemoveChunks(gridUid, gridComp, list);
			}
			if (decalGridState.Chunks.Count > 0)
			{
				this.UpdateChunks(gridUid, gridComp, decalGridState.Chunks);
			}
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x0007CE1C File Offset: 0x0007B01C
		private void OnChunkUpdate(DecalChunkUpdateEvent ev)
		{
			foreach (KeyValuePair<EntityUid, Dictionary<Vector2i, DecalGridComponent.DecalChunk>> keyValuePair in ev.Data)
			{
				EntityUid entityUid;
				Dictionary<Vector2i, DecalGridComponent.DecalChunk> dictionary;
				keyValuePair.Deconstruct(out entityUid, out dictionary);
				EntityUid entityUid2 = entityUid;
				Dictionary<Vector2i, DecalGridComponent.DecalChunk> dictionary2 = dictionary;
				if (dictionary2.Count != 0)
				{
					DecalGridComponent gridComp;
					if (!base.TryComp<DecalGridComponent>(entityUid2, ref gridComp))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(68, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Received decal information for an entity without a decal component: ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entityUid2));
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						this.UpdateChunks(entityUid2, gridComp, dictionary2);
					}
				}
			}
			foreach (KeyValuePair<EntityUid, HashSet<Vector2i>> keyValuePair2 in ev.RemovedChunks)
			{
				EntityUid entityUid;
				HashSet<Vector2i> hashSet;
				keyValuePair2.Deconstruct(out entityUid, out hashSet);
				EntityUid entityUid3 = entityUid;
				HashSet<Vector2i> hashSet2 = hashSet;
				if (hashSet2.Count != 0)
				{
					DecalGridComponent gridComp2;
					if (!base.TryComp<DecalGridComponent>(entityUid3, ref gridComp2))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(68, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Received decal information for an entity without a decal component: ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entityUid3));
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						this.RemoveChunks(entityUid3, gridComp2, hashSet2);
					}
				}
			}
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x0007CF6C File Offset: 0x0007B16C
		private void UpdateChunks(EntityUid gridId, DecalGridComponent gridComp, Dictionary<Vector2i, DecalGridComponent.DecalChunk> updatedGridChunks)
		{
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunkCollection = gridComp.ChunkCollection.ChunkCollection;
			SortedDictionary<int, SortedDictionary<uint, Decal>> decalRenderIndex = gridComp.DecalRenderIndex;
			Dictionary<uint, int> decalZIndexIndex = gridComp.DecalZIndexIndex;
			foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> keyValuePair in updatedGridChunks)
			{
				Vector2i vector2i;
				DecalGridComponent.DecalChunk decalChunk;
				keyValuePair.Deconstruct(out vector2i, out decalChunk);
				Vector2i vector2i2 = vector2i;
				DecalGridComponent.DecalChunk decalChunk2 = decalChunk;
				DecalGridComponent.DecalChunk decalChunk3;
				if (chunkCollection.TryGetValue(vector2i2, out decalChunk3))
				{
					HashSet<uint> hashSet = new HashSet<uint>(decalChunk3.Decals.Keys);
					hashSet.ExceptWith(decalChunk2.Decals.Keys);
					foreach (uint num in hashSet)
					{
						this.OnDecalRemoved(gridId, num, gridComp, vector2i2, decalChunk3);
						gridComp.DecalIndex.Remove(num);
					}
				}
				chunkCollection[vector2i2] = decalChunk2;
				foreach (KeyValuePair<uint, Decal> keyValuePair2 in decalChunk2.Decals)
				{
					uint num2;
					Decal decal;
					keyValuePair2.Deconstruct(out num2, out decal);
					uint key = num2;
					Decal decal2 = decal;
					int key2;
					if (decalZIndexIndex.TryGetValue(key, out key2))
					{
						decalRenderIndex[key2].Remove(key);
					}
					Extensions.GetOrNew<int, SortedDictionary<uint, Decal>>(decalRenderIndex, decal2.ZIndex)[key] = decal2;
					decalZIndexIndex[key] = decal2.ZIndex;
					gridComp.DecalIndex[key] = vector2i2;
				}
			}
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x0007D138 File Offset: 0x0007B338
		private void RemoveChunks(EntityUid gridId, DecalGridComponent gridComp, IEnumerable<Vector2i> chunks)
		{
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunkCollection = gridComp.ChunkCollection.ChunkCollection;
			foreach (Vector2i vector2i in chunks)
			{
				DecalGridComponent.DecalChunk decalChunk;
				if (chunkCollection.TryGetValue(vector2i, out decalChunk))
				{
					foreach (uint num in decalChunk.Decals.Keys)
					{
						this.OnDecalRemoved(gridId, num, gridComp, vector2i, decalChunk);
						gridComp.DecalIndex.Remove(num);
					}
					chunkCollection.Remove(vector2i);
				}
			}
		}

		// Token: 0x04000B0F RID: 2831
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x04000B10 RID: 2832
		[Dependency]
		private readonly SpriteSystem _sprites;

		// Token: 0x04000B11 RID: 2833
		private DecalOverlay _overlay;
	}
}
