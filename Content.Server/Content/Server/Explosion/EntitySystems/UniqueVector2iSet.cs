using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x02000510 RID: 1296
	public sealed class UniqueVector2iSet
	{
		// Token: 0x06001B00 RID: 6912 RVA: 0x00090BDC File Offset: 0x0008EDDC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2i ToChunkIndices(Vector2i indices)
		{
			int num = (int)Math.Floor((double)((float)indices.X / 32f));
			int y = (int)Math.Floor((double)((float)indices.Y / 32f));
			return new Vector2i(num, y);
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x00090C18 File Offset: 0x0008EE18
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Add(Vector2i index)
		{
			Vector2i chunkIndex = this.ToChunkIndices(index);
			UniqueVector2iSet.VectorChunk chunk;
			if (this._chunks.TryGetValue(chunkIndex, out chunk))
			{
				return chunk.Add(index);
			}
			chunk = new UniqueVector2iSet.VectorChunk();
			chunk.Add(index);
			this._chunks[chunkIndex] = chunk;
			return true;
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x00090C64 File Offset: 0x0008EE64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Vector2i index)
		{
			UniqueVector2iSet.VectorChunk chunk;
			return this._chunks.TryGetValue(this.ToChunkIndices(index), out chunk) && chunk.Contains(index);
		}

		// Token: 0x0400114D RID: 4429
		private const int ChunkSize = 32;

		// Token: 0x0400114E RID: 4430
		[Nullable(1)]
		private Dictionary<Vector2i, UniqueVector2iSet.VectorChunk> _chunks = new Dictionary<Vector2i, UniqueVector2iSet.VectorChunk>();

		// Token: 0x02000A09 RID: 2569
		private sealed class VectorChunk
		{
			// Token: 0x06003421 RID: 13345 RVA: 0x00109F84 File Offset: 0x00108184
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Add(Vector2i index)
			{
				int x = MathHelper.Mod(index.X, 32);
				int y = MathHelper.Mod(index.Y, 32);
				int oldFlags = this._tiles[x];
				int newFlags = oldFlags | 1 << y;
				if (newFlags == oldFlags)
				{
					return false;
				}
				this._tiles[x] = newFlags;
				return true;
			}

			// Token: 0x06003422 RID: 13346 RVA: 0x00109FD0 File Offset: 0x001081D0
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Contains(Vector2i index)
			{
				int x = MathHelper.Mod(index.X, 32);
				int y = MathHelper.Mod(index.Y, 32);
				return (this._tiles[x] & 1 << y) != 0;
			}

			// Token: 0x04002320 RID: 8992
			[Nullable(1)]
			private readonly int[] _tiles = new int[32];
		}
	}
}
