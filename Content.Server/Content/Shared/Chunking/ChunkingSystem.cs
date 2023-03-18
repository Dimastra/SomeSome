using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Decals;
using Microsoft.Extensions.ObjectPool;
using Robust.Server.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Shared.Chunking
{
	// Token: 0x02000016 RID: 22
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChunkingSystem : EntitySystem
	{
		// Token: 0x0600003C RID: 60 RVA: 0x00002D45 File Offset: 0x00000F45
		public override void Initialize()
		{
			base.Initialize();
			this._configurationManager.OnValueChanged<float>(CVars.NetMaxUpdateRange, new Action<float>(this.OnPvsRangeChanged), true);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002D6A File Offset: 0x00000F6A
		public override void Shutdown()
		{
			base.Shutdown();
			this._configurationManager.UnsubValueChanged<float>(CVars.NetMaxUpdateRange, new Action<float>(this.OnPvsRangeChanged));
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002D8E File Offset: 0x00000F8E
		private void OnPvsRangeChanged(float value)
		{
			this._baseViewBounds = Box2.UnitCentered.Scale(value);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002DA4 File Offset: 0x00000FA4
		public Dictionary<EntityUid, HashSet<Vector2i>> GetChunksForSession(IPlayerSession session, int chunkSize, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, ObjectPool<HashSet<Vector2i>> indexPool, ObjectPool<Dictionary<EntityUid, HashSet<Vector2i>>> viewerPool, float? viewEnlargement = null)
		{
			HashSet<EntityUid> viewers = this.GetSessionViewers(session);
			return this.GetChunksForViewers(viewers, chunkSize, indexPool, viewerPool, viewEnlargement ?? ((float)chunkSize), xformQuery);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002DE0 File Offset: 0x00000FE0
		private HashSet<EntityUid> GetSessionViewers(IPlayerSession session)
		{
			HashSet<EntityUid> viewers = new HashSet<EntityUid>();
			if (session.Status != 3 || session.AttachedEntity == null)
			{
				return viewers;
			}
			viewers.Add(session.AttachedEntity.Value);
			foreach (EntityUid uid in session.ViewSubscriptions)
			{
				viewers.Add(uid);
			}
			return viewers;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002E68 File Offset: 0x00001068
		private Dictionary<EntityUid, HashSet<Vector2i>> GetChunksForViewers(HashSet<EntityUid> viewers, int chunkSize, ObjectPool<HashSet<Vector2i>> indexPool, ObjectPool<Dictionary<EntityUid, HashSet<Vector2i>>> viewerPool, float viewEnlargement, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			Dictionary<EntityUid, HashSet<Vector2i>> chunks = viewerPool.Get();
			foreach (EntityUid viewerUid in viewers)
			{
				TransformComponent xform;
				if (!xformQuery.TryGetComponent(viewerUid, ref xform))
				{
					Logger.Error("Player has deleted viewer entities? Viewers: " + string.Join<EntityStringRepresentation>(", ", from x in viewers
					select base.ToPrettyString(x)));
				}
				else
				{
					Vector2 pos = this._transform.GetWorldPosition(xform, xformQuery);
					Box2 bounds = this._baseViewBounds.Translated(pos).Enlarged(viewEnlargement);
					foreach (MapGridComponent grid in this._mapManager.FindGridsIntersecting(xform.MapID, bounds, true))
					{
						HashSet<Vector2i> set;
						if (!chunks.TryGetValue(grid.Owner, out set))
						{
							set = (chunks[grid.Owner] = indexPool.Get());
						}
						ChunkIndicesEnumerator enumerator = new ChunkIndicesEnumerator(this._transform.GetInvWorldMatrix(grid.Owner, xformQuery).TransformBox(ref bounds), chunkSize);
						Vector2i? indices;
						while (enumerator.MoveNext(out indices))
						{
							set.Add(indices.Value);
						}
					}
				}
			}
			return chunks;
		}

		// Token: 0x04000025 RID: 37
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000026 RID: 38
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000027 RID: 39
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000028 RID: 40
		private Box2 _baseViewBounds;
	}
}
