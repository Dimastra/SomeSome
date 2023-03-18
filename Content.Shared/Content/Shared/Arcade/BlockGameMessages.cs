using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Arcade
{
	// Token: 0x020006EC RID: 1772
	public static class BlockGameMessages
	{
		// Token: 0x02000870 RID: 2160
		[NetSerializable]
		[Serializable]
		public sealed class BlockGamePlayerActionMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019DE RID: 6622 RVA: 0x00051970 File Offset: 0x0004FB70
			public BlockGamePlayerActionMessage(BlockGamePlayerAction playerAction)
			{
				this.PlayerAction = playerAction;
			}

			// Token: 0x04001A13 RID: 6675
			public readonly BlockGamePlayerAction PlayerAction;
		}

		// Token: 0x02000871 RID: 2161
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class BlockGameVisualUpdateMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019DF RID: 6623 RVA: 0x0005197F File Offset: 0x0004FB7F
			public BlockGameVisualUpdateMessage(BlockGameBlock[] blocks, BlockGameMessages.BlockGameVisualType gameVisualType)
			{
				this.Blocks = blocks;
				this.GameVisualType = gameVisualType;
			}

			// Token: 0x04001A14 RID: 6676
			public readonly BlockGameMessages.BlockGameVisualType GameVisualType;

			// Token: 0x04001A15 RID: 6677
			public readonly BlockGameBlock[] Blocks;
		}

		// Token: 0x02000872 RID: 2162
		public enum BlockGameVisualType
		{
			// Token: 0x04001A17 RID: 6679
			GameField,
			// Token: 0x04001A18 RID: 6680
			HoldBlock,
			// Token: 0x04001A19 RID: 6681
			NextBlock
		}

		// Token: 0x02000873 RID: 2163
		[NetSerializable]
		[Serializable]
		public sealed class BlockGameScoreUpdateMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019E0 RID: 6624 RVA: 0x00051995 File Offset: 0x0004FB95
			public BlockGameScoreUpdateMessage(int points)
			{
				this.Points = points;
			}

			// Token: 0x04001A1A RID: 6682
			public readonly int Points;
		}

		// Token: 0x02000874 RID: 2164
		[NetSerializable]
		[Serializable]
		public sealed class BlockGameUserStatusMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019E1 RID: 6625 RVA: 0x000519A4 File Offset: 0x0004FBA4
			public BlockGameUserStatusMessage(bool isPlayer)
			{
				this.IsPlayer = isPlayer;
			}

			// Token: 0x04001A1B RID: 6683
			public readonly bool IsPlayer;
		}

		// Token: 0x02000875 RID: 2165
		[NetSerializable]
		[Virtual]
		[Serializable]
		public class BlockGameSetScreenMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019E2 RID: 6626 RVA: 0x000519B3 File Offset: 0x0004FBB3
			public BlockGameSetScreenMessage(BlockGameMessages.BlockGameScreen screen, bool isStarted = true)
			{
				this.Screen = screen;
				this.IsStarted = isStarted;
			}

			// Token: 0x04001A1C RID: 6684
			public readonly BlockGameMessages.BlockGameScreen Screen;

			// Token: 0x04001A1D RID: 6685
			public readonly bool IsStarted;
		}

		// Token: 0x02000876 RID: 2166
		[NetSerializable]
		[Serializable]
		public sealed class BlockGameGameOverScreenMessage : BlockGameMessages.BlockGameSetScreenMessage
		{
			// Token: 0x060019E3 RID: 6627 RVA: 0x000519C9 File Offset: 0x0004FBC9
			public BlockGameGameOverScreenMessage(int finalScore, int? localPlacement, int? globalPlacement) : base(BlockGameMessages.BlockGameScreen.Gameover, true)
			{
				this.FinalScore = finalScore;
				this.LocalPlacement = localPlacement;
				this.GlobalPlacement = globalPlacement;
			}

			// Token: 0x04001A1E RID: 6686
			public readonly int FinalScore;

			// Token: 0x04001A1F RID: 6687
			public readonly int? LocalPlacement;

			// Token: 0x04001A20 RID: 6688
			public readonly int? GlobalPlacement;
		}

		// Token: 0x02000877 RID: 2167
		[NetSerializable]
		[Serializable]
		public enum BlockGameScreen
		{
			// Token: 0x04001A22 RID: 6690
			Game,
			// Token: 0x04001A23 RID: 6691
			Pause,
			// Token: 0x04001A24 RID: 6692
			Gameover,
			// Token: 0x04001A25 RID: 6693
			Highscores
		}

		// Token: 0x02000878 RID: 2168
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class BlockGameHighScoreUpdateMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019E4 RID: 6628 RVA: 0x000519E8 File Offset: 0x0004FBE8
			public BlockGameHighScoreUpdateMessage(List<BlockGameMessages.HighScoreEntry> localHighscores, List<BlockGameMessages.HighScoreEntry> globalHighscores)
			{
				this.LocalHighscores = localHighscores;
				this.GlobalHighscores = globalHighscores;
			}

			// Token: 0x04001A26 RID: 6694
			public List<BlockGameMessages.HighScoreEntry> LocalHighscores;

			// Token: 0x04001A27 RID: 6695
			public List<BlockGameMessages.HighScoreEntry> GlobalHighscores;
		}

		// Token: 0x02000879 RID: 2169
		[NetSerializable]
		[Serializable]
		public sealed class HighScoreEntry : IComparable
		{
			// Token: 0x060019E5 RID: 6629 RVA: 0x000519FE File Offset: 0x0004FBFE
			[NullableContext(1)]
			public HighScoreEntry(string name, int score)
			{
				this.Name = name;
				this.Score = score;
			}

			// Token: 0x060019E6 RID: 6630 RVA: 0x00051A14 File Offset: 0x0004FC14
			[NullableContext(2)]
			public int CompareTo(object obj)
			{
				BlockGameMessages.HighScoreEntry entry = obj as BlockGameMessages.HighScoreEntry;
				if (entry == null)
				{
					return 0;
				}
				return this.Score.CompareTo(entry.Score);
			}

			// Token: 0x04001A28 RID: 6696
			[Nullable(1)]
			public string Name;

			// Token: 0x04001A29 RID: 6697
			public int Score;
		}

		// Token: 0x0200087A RID: 2170
		[NetSerializable]
		[Serializable]
		public sealed class BlockGameLevelUpdateMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019E7 RID: 6631 RVA: 0x00051A3E File Offset: 0x0004FC3E
			public BlockGameLevelUpdateMessage(int level)
			{
				this.Level = level;
			}

			// Token: 0x04001A2A RID: 6698
			public readonly int Level;
		}
	}
}
