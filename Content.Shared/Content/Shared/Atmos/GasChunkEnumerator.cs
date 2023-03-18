using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.EntitySystems;

namespace Content.Shared.Atmos
{
	// Token: 0x02000693 RID: 1683
	[NullableContext(1)]
	[Nullable(0)]
	public struct GasChunkEnumerator
	{
		// Token: 0x06001497 RID: 5271 RVA: 0x000448DB File Offset: 0x00042ADB
		public GasChunkEnumerator(GasOverlayChunk chunk)
		{
			this.X = 0;
			this.Y = -1;
			this._chunk = chunk;
			this._column = this._chunk.TileData[0];
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x00044908 File Offset: 0x00042B08
		public bool MoveNext(out SharedGasTileOverlaySystem.GasOverlayData gas)
		{
			while (this.X < 8)
			{
				while (this.Y < 7)
				{
					this.Y++;
					gas = this._column[this.Y];
					if (!gas.Equals(default(SharedGasTileOverlaySystem.GasOverlayData)))
					{
						return true;
					}
				}
				this.X++;
				if (this.X < 8)
				{
					this._column = this._chunk.TileData[this.X];
				}
				this.Y = -1;
			}
			gas = default(SharedGasTileOverlaySystem.GasOverlayData);
			return false;
		}

		// Token: 0x04001479 RID: 5241
		private GasOverlayChunk _chunk;

		// Token: 0x0400147A RID: 5242
		public int X;

		// Token: 0x0400147B RID: 5243
		public int Y;

		// Token: 0x0400147C RID: 5244
		private SharedGasTileOverlaySystem.GasOverlayData[] _column;
	}
}
