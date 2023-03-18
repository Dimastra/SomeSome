using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals
{
	// Token: 0x02000526 RID: 1318
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DecalGridState : ComponentState, IComponentDeltaState
	{
		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x00033541 File Offset: 0x00031741
		public bool FullState
		{
			get
			{
				return this.AllChunks == null;
			}
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x0003354C File Offset: 0x0003174C
		public DecalGridState(Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunks)
		{
			this.Chunks = chunks;
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x0003355C File Offset: 0x0003175C
		public void ApplyToFullState(ComponentState fullState)
		{
			DecalGridState state = (DecalGridState)fullState;
			foreach (Vector2i key in state.Chunks.Keys)
			{
				if (!this.AllChunks.Contains(key))
				{
					state.Chunks.Remove(key);
				}
			}
			foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> keyValuePair in this.Chunks)
			{
				Vector2i vector2i;
				DecalGridComponent.DecalChunk decalChunk;
				keyValuePair.Deconstruct(out vector2i, out decalChunk);
				Vector2i chunk = vector2i;
				DecalGridComponent.DecalChunk data = decalChunk;
				state.Chunks[chunk] = new DecalGridComponent.DecalChunk(data);
			}
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x00033634 File Offset: 0x00031834
		public ComponentState CreateNewFullState(ComponentState fullState)
		{
			DecalGridState state = (DecalGridState)fullState;
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunks = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>(state.Chunks.Count);
			foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> keyValuePair in this.Chunks)
			{
				Vector2i vector2i;
				DecalGridComponent.DecalChunk decalChunk;
				keyValuePair.Deconstruct(out vector2i, out decalChunk);
				Vector2i chunk = vector2i;
				DecalGridComponent.DecalChunk data = decalChunk;
				chunks[chunk] = new DecalGridComponent.DecalChunk(data);
			}
			foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> keyValuePair in state.Chunks)
			{
				Vector2i vector2i;
				DecalGridComponent.DecalChunk decalChunk;
				keyValuePair.Deconstruct(out vector2i, out decalChunk);
				Vector2i chunk2 = vector2i;
				DecalGridComponent.DecalChunk data2 = decalChunk;
				if (this.AllChunks.Contains(chunk2))
				{
					chunks.TryAdd(chunk2, new DecalGridComponent.DecalChunk(data2));
				}
			}
			return new DecalGridState(chunks);
		}

		// Token: 0x04000F1D RID: 3869
		public Dictionary<Vector2i, DecalGridComponent.DecalChunk> Chunks;

		// Token: 0x04000F1E RID: 3870
		[Nullable(2)]
		public HashSet<Vector2i> AllChunks;
	}
}
