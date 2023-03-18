using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Arcade
{
	// Token: 0x020006EA RID: 1770
	[NetSerializable]
	[Serializable]
	public struct BlockGameBlock
	{
		// Token: 0x06001576 RID: 5494 RVA: 0x0004613B File Offset: 0x0004433B
		public BlockGameBlock(Vector2i position, BlockGameBlock.BlockGameBlockColor gameBlockColor)
		{
			this.Position = position;
			this.GameBlockColor = gameBlockColor;
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x0004614C File Offset: 0x0004434C
		public static BlockGameBlock.BlockGameBlockColor ToGhostBlockColor(BlockGameBlock.BlockGameBlockColor inColor)
		{
			BlockGameBlock.BlockGameBlockColor result;
			switch (inColor)
			{
			case BlockGameBlock.BlockGameBlockColor.Red:
				result = BlockGameBlock.BlockGameBlockColor.GhostRed;
				break;
			case BlockGameBlock.BlockGameBlockColor.Orange:
				result = BlockGameBlock.BlockGameBlockColor.GhostOrange;
				break;
			case BlockGameBlock.BlockGameBlockColor.Yellow:
				result = BlockGameBlock.BlockGameBlockColor.GhostYellow;
				break;
			case BlockGameBlock.BlockGameBlockColor.Green:
				result = BlockGameBlock.BlockGameBlockColor.GhostGreen;
				break;
			case BlockGameBlock.BlockGameBlockColor.Blue:
				result = BlockGameBlock.BlockGameBlockColor.GhostBlue;
				break;
			case BlockGameBlock.BlockGameBlockColor.LightBlue:
				result = BlockGameBlock.BlockGameBlockColor.GhostLightBlue;
				break;
			case BlockGameBlock.BlockGameBlockColor.Purple:
				result = BlockGameBlock.BlockGameBlockColor.GhostPurple;
				break;
			default:
				result = inColor;
				break;
			}
			return result;
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x000461A4 File Offset: 0x000443A4
		public static Color ToColor(BlockGameBlock.BlockGameBlockColor inColor)
		{
			Color result;
			switch (inColor)
			{
			case BlockGameBlock.BlockGameBlockColor.Red:
				result = Color.Red;
				break;
			case BlockGameBlock.BlockGameBlockColor.Orange:
				result = Color.Orange;
				break;
			case BlockGameBlock.BlockGameBlockColor.Yellow:
				result = Color.Yellow;
				break;
			case BlockGameBlock.BlockGameBlockColor.Green:
				result = Color.Lime;
				break;
			case BlockGameBlock.BlockGameBlockColor.Blue:
				result = Color.Blue;
				break;
			case BlockGameBlock.BlockGameBlockColor.LightBlue:
				result = Color.Cyan;
				break;
			case BlockGameBlock.BlockGameBlockColor.Purple:
				result = Color.DarkOrchid;
				break;
			case BlockGameBlock.BlockGameBlockColor.GhostRed:
				result = Color.Red.WithAlpha(0.33f);
				break;
			case BlockGameBlock.BlockGameBlockColor.GhostOrange:
				result = Color.Orange.WithAlpha(0.33f);
				break;
			case BlockGameBlock.BlockGameBlockColor.GhostYellow:
				result = Color.Yellow.WithAlpha(0.33f);
				break;
			case BlockGameBlock.BlockGameBlockColor.GhostGreen:
				result = Color.Lime.WithAlpha(0.33f);
				break;
			case BlockGameBlock.BlockGameBlockColor.GhostBlue:
				result = Color.Blue.WithAlpha(0.33f);
				break;
			case BlockGameBlock.BlockGameBlockColor.GhostLightBlue:
				result = Color.Cyan.WithAlpha(0.33f);
				break;
			case BlockGameBlock.BlockGameBlockColor.GhostPurple:
				result = Color.DarkOrchid.WithAlpha(0.33f);
				break;
			default:
				result = Color.Olive;
				break;
			}
			return result;
		}

		// Token: 0x0400158C RID: 5516
		public Vector2i Position;

		// Token: 0x0400158D RID: 5517
		public readonly BlockGameBlock.BlockGameBlockColor GameBlockColor;

		// Token: 0x0200086F RID: 2159
		[NetSerializable]
		[Serializable]
		public enum BlockGameBlockColor
		{
			// Token: 0x04001A05 RID: 6661
			Red,
			// Token: 0x04001A06 RID: 6662
			Orange,
			// Token: 0x04001A07 RID: 6663
			Yellow,
			// Token: 0x04001A08 RID: 6664
			Green,
			// Token: 0x04001A09 RID: 6665
			Blue,
			// Token: 0x04001A0A RID: 6666
			LightBlue,
			// Token: 0x04001A0B RID: 6667
			Purple,
			// Token: 0x04001A0C RID: 6668
			GhostRed,
			// Token: 0x04001A0D RID: 6669
			GhostOrange,
			// Token: 0x04001A0E RID: 6670
			GhostYellow,
			// Token: 0x04001A0F RID: 6671
			GhostGreen,
			// Token: 0x04001A10 RID: 6672
			GhostBlue,
			// Token: 0x04001A11 RID: 6673
			GhostLightBlue,
			// Token: 0x04001A12 RID: 6674
			GhostPurple
		}
	}
}
