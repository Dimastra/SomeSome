using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Atmos
{
	// Token: 0x02000692 RID: 1682
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Access(new Type[]
	{
		typeof(SharedGasTileOverlaySystem)
	})]
	[Serializable]
	public sealed class GasOverlayChunk
	{
		// Token: 0x06001493 RID: 5267 RVA: 0x00044790 File Offset: 0x00042990
		public GasOverlayChunk(Vector2i index)
		{
			this.Index = index;
			this.Origin = this.Index * 8;
			for (int i = 0; i < 8; i++)
			{
				this.TileData[i] = new SharedGasTileOverlaySystem.GasOverlayData[8];
			}
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x000447E4 File Offset: 0x000429E4
		public GasOverlayChunk(GasOverlayChunk data)
		{
			this.Index = data.Index;
			this.Origin = data.Origin;
			for (int i = 0; i < 8; i++)
			{
				SharedGasTileOverlaySystem.GasOverlayData[] array = this.TileData[i] = new SharedGasTileOverlaySystem.GasOverlayData[8];
				Array.Copy(data.TileData[i], array, 8);
			}
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x00044848 File Offset: 0x00042A48
		public ref SharedGasTileOverlaySystem.GasOverlayData GetData(Vector2i gridIndices)
		{
			return ref this.TileData[gridIndices.X - this.Origin.X][gridIndices.Y - this.Origin.Y];
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x0004487C File Offset: 0x00042A7C
		private bool InBounds(Vector2i gridIndices)
		{
			return gridIndices.X >= this.Origin.X && gridIndices.Y >= this.Origin.Y && gridIndices.X < this.Origin.X + 8 && gridIndices.Y < this.Origin.Y + 8;
		}

		// Token: 0x04001475 RID: 5237
		public readonly Vector2i Index;

		// Token: 0x04001476 RID: 5238
		public readonly Vector2i Origin;

		// Token: 0x04001477 RID: 5239
		public SharedGasTileOverlaySystem.GasOverlayData[][] TileData = new SharedGasTileOverlaySystem.GasOverlayData[8][];

		// Token: 0x04001478 RID: 5240
		[NonSerialized]
		public GameTick LastUpdate;
	}
}
