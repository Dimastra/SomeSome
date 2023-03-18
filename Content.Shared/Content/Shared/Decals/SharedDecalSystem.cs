using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared.Decals
{
	// Token: 0x02000528 RID: 1320
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedDecalSystem : EntitySystem
	{
		// Token: 0x06000FFB RID: 4091 RVA: 0x00033765 File Offset: 0x00031965
		public static Vector2i GetChunkIndices(Vector2 coordinates)
		{
			return new Vector2i((int)Math.Floor((double)(coordinates.X / 32f)), (int)Math.Floor((double)(coordinates.Y / 32f)));
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x00033792 File Offset: 0x00031992
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GridInitializeEvent>(new EntityEventHandler<GridInitializeEvent>(this.OnGridInitialize), null, null);
			base.SubscribeLocalEvent<DecalGridComponent, ComponentStartup>(new ComponentEventHandler<DecalGridComponent, ComponentStartup>(this.OnCompStartup), null, null);
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x000337C2 File Offset: 0x000319C2
		private void OnGridInitialize(GridInitializeEvent msg)
		{
			base.EnsureComp<DecalGridComponent>(msg.EntityUid);
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x000337D4 File Offset: 0x000319D4
		private void OnCompStartup(EntityUid uid, DecalGridComponent component, ComponentStartup args)
		{
			foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> keyValuePair in component.ChunkCollection.ChunkCollection)
			{
				Vector2i vector2i;
				DecalGridComponent.DecalChunk decalChunk;
				keyValuePair.Deconstruct(out vector2i, out decalChunk);
				Vector2i indices = vector2i;
				foreach (uint decalUid in decalChunk.Decals.Keys)
				{
					component.DecalIndex[decalUid] = indices;
				}
			}
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x00033884 File Offset: 0x00031A84
		[NullableContext(2)]
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		protected Dictionary<Vector2i, DecalGridComponent.DecalChunk> ChunkCollection(EntityUid gridEuid, DecalGridComponent comp = null)
		{
			if (!base.Resolve<DecalGridComponent>(gridEuid, ref comp, true))
			{
				return null;
			}
			return comp.ChunkCollection.ChunkCollection;
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x0003389F File Offset: 0x00031A9F
		protected virtual void DirtyChunk(EntityUid id, Vector2i chunkIndices, DecalGridComponent.DecalChunk chunk)
		{
		}

		// Token: 0x06001001 RID: 4097 RVA: 0x000338A4 File Offset: 0x00031AA4
		[NullableContext(2)]
		protected bool RemoveDecalInternal(EntityUid gridId, uint decalId, [NotNullWhen(true)] out Decal removed, DecalGridComponent component = null)
		{
			removed = null;
			if (!base.Resolve<DecalGridComponent>(gridId, ref component, true))
			{
				return false;
			}
			Vector2i indices;
			DecalGridComponent.DecalChunk chunk;
			if (!component.DecalIndex.Remove(decalId, out indices) || !component.ChunkCollection.ChunkCollection.TryGetValue(indices, out chunk) || !chunk.Decals.Remove(decalId, out removed))
			{
				return false;
			}
			if (chunk.Decals.Count == 0)
			{
				component.ChunkCollection.ChunkCollection.Remove(indices);
			}
			this.DirtyChunk(gridId, indices, chunk);
			this.OnDecalRemoved(gridId, decalId, component, indices, chunk);
			return true;
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x00033930 File Offset: 0x00031B30
		protected virtual void OnDecalRemoved(EntityUid gridId, uint decalId, DecalGridComponent component, Vector2i indices, DecalGridComponent.DecalChunk chunk)
		{
		}

		// Token: 0x04000F24 RID: 3876
		[Dependency]
		protected readonly IPrototypeManager PrototypeManager;

		// Token: 0x04000F25 RID: 3877
		[Dependency]
		protected readonly IMapManager MapManager;

		// Token: 0x04000F26 RID: 3878
		public const int ChunkSize = 32;
	}
}
