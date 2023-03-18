using System;
using Robust.Shared.Maths;

namespace Content.Shared.Arcade
{
	// Token: 0x020006EB RID: 1771
	public static class BlockGameVector2Extensions
	{
		// Token: 0x06001579 RID: 5497 RVA: 0x000462DE File Offset: 0x000444DE
		public static BlockGameBlock ToBlockGameBlock(this Vector2i vector2, BlockGameBlock.BlockGameBlockColor gameBlockColor)
		{
			return new BlockGameBlock(vector2, gameBlockColor);
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x000462E7 File Offset: 0x000444E7
		public static Vector2i AddToX(this Vector2i vector2, int amount)
		{
			return new Vector2i(vector2.X + amount, vector2.Y);
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x000462FC File Offset: 0x000444FC
		public static Vector2i AddToY(this Vector2i vector2, int amount)
		{
			return new Vector2i(vector2.X, vector2.Y + amount);
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x00046311 File Offset: 0x00044511
		public static Vector2i Rotate90DegreesAsOffset(this Vector2i vector)
		{
			return new Vector2i(-vector.Y, vector.X);
		}
	}
}
