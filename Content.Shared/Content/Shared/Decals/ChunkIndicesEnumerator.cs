using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Maths;

namespace Content.Shared.Decals
{
	// Token: 0x02000529 RID: 1321
	public struct ChunkIndicesEnumerator
	{
		// Token: 0x06001004 RID: 4100 RVA: 0x0003393C File Offset: 0x00031B3C
		public ChunkIndicesEnumerator(Box2 localAABB, int chunkSize)
		{
			this._chunkLB = new Vector2i((int)Math.Floor((double)(localAABB.Left / (float)chunkSize)), (int)Math.Floor((double)(localAABB.Bottom / (float)chunkSize)));
			this._chunkRT = new Vector2i((int)Math.Floor((double)(localAABB.Right / (float)chunkSize)), (int)Math.Floor((double)(localAABB.Top / (float)chunkSize)));
			this._xIndex = this._chunkLB.X;
			this._yIndex = this._chunkLB.Y;
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x000339C4 File Offset: 0x00031BC4
		public bool MoveNext([NotNullWhen(true)] out Vector2i? indices)
		{
			if (this._yIndex > this._chunkRT.Y)
			{
				this._yIndex = this._chunkLB.Y;
				this._xIndex++;
			}
			indices = new Vector2i?(new Vector2i(this._xIndex, this._yIndex));
			this._yIndex++;
			return this._xIndex <= this._chunkRT.X;
		}

		// Token: 0x04000F27 RID: 3879
		private Vector2i _chunkLB;

		// Token: 0x04000F28 RID: 3880
		private Vector2i _chunkRT;

		// Token: 0x04000F29 RID: 3881
		private int _xIndex;

		// Token: 0x04000F2A RID: 3882
		private int _yIndex;
	}
}
