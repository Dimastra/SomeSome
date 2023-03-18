using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.NPC
{
	// Token: 0x020002D1 RID: 721
	public abstract class SharedPathfindingSystem : EntitySystem
	{
		// Token: 0x060007E0 RID: 2016 RVA: 0x0001A296 File Offset: 0x00018496
		public Vector2 GetCoordinate(Vector2i chunk, Vector2i index)
		{
			return new Vector2((float)index.X, (float)index.Y) / 4f + chunk * 8 + 0.125f;
		}

		// Token: 0x0400081E RID: 2078
		public const byte SubStep = 4;

		// Token: 0x0400081F RID: 2079
		public const byte ChunkSize = 8;

		// Token: 0x04000820 RID: 2080
		protected const float StepOffset = 0.125f;
	}
}
