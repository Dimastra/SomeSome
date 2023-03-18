using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components
{
	// Token: 0x020006E2 RID: 1762
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GasTileOverlayState : ComponentState, IComponentDeltaState
	{
		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06001563 RID: 5475 RVA: 0x00045EB7 File Offset: 0x000440B7
		public bool FullState
		{
			get
			{
				return this.AllChunks == null;
			}
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x00045EC2 File Offset: 0x000440C2
		public GasTileOverlayState(Dictionary<Vector2i, GasOverlayChunk> chunks)
		{
			this.Chunks = chunks;
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x00045ED4 File Offset: 0x000440D4
		public void ApplyToFullState(ComponentState fullState)
		{
			GasTileOverlayState state = (GasTileOverlayState)fullState;
			foreach (Vector2i key in state.Chunks.Keys)
			{
				if (!this.AllChunks.Contains(key))
				{
					state.Chunks.Remove(key);
				}
			}
			foreach (KeyValuePair<Vector2i, GasOverlayChunk> keyValuePair in this.Chunks)
			{
				Vector2i vector2i;
				GasOverlayChunk gasOverlayChunk;
				keyValuePair.Deconstruct(out vector2i, out gasOverlayChunk);
				Vector2i chunk = vector2i;
				GasOverlayChunk data = gasOverlayChunk;
				state.Chunks[chunk] = new GasOverlayChunk(data);
			}
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x00045FAC File Offset: 0x000441AC
		public ComponentState CreateNewFullState(ComponentState fullState)
		{
			GasTileOverlayState state = (GasTileOverlayState)fullState;
			Dictionary<Vector2i, GasOverlayChunk> chunks = new Dictionary<Vector2i, GasOverlayChunk>(state.Chunks.Count);
			foreach (KeyValuePair<Vector2i, GasOverlayChunk> keyValuePair in this.Chunks)
			{
				Vector2i vector2i;
				GasOverlayChunk gasOverlayChunk;
				keyValuePair.Deconstruct(out vector2i, out gasOverlayChunk);
				Vector2i chunk = vector2i;
				GasOverlayChunk data = gasOverlayChunk;
				chunks[chunk] = new GasOverlayChunk(data);
			}
			foreach (KeyValuePair<Vector2i, GasOverlayChunk> keyValuePair in state.Chunks)
			{
				Vector2i vector2i;
				GasOverlayChunk gasOverlayChunk;
				keyValuePair.Deconstruct(out vector2i, out gasOverlayChunk);
				Vector2i chunk2 = vector2i;
				GasOverlayChunk data2 = gasOverlayChunk;
				if (this.AllChunks.Contains(chunk2))
				{
					chunks.TryAdd(chunk2, new GasOverlayChunk(data2));
				}
			}
			return new GasTileOverlayState(chunks);
		}

		// Token: 0x0400157F RID: 5503
		public readonly Dictionary<Vector2i, GasOverlayChunk> Chunks;

		// Token: 0x04001580 RID: 5504
		[Nullable(2)]
		public HashSet<Vector2i> AllChunks;
	}
}
