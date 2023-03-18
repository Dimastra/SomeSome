using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Arcade
{
	// Token: 0x020006EF RID: 1775
	public abstract class SharedSpaceVillainArcadeComponent : Component
	{
		// Token: 0x0200087B RID: 2171
		[NetSerializable]
		[Serializable]
		public enum Indicators
		{
			// Token: 0x04001A2C RID: 6700
			HealthManager,
			// Token: 0x04001A2D RID: 6701
			HealthLimiter
		}

		// Token: 0x0200087C RID: 2172
		[NetSerializable]
		[Serializable]
		public enum PlayerAction
		{
			// Token: 0x04001A2F RID: 6703
			Attack,
			// Token: 0x04001A30 RID: 6704
			Heal,
			// Token: 0x04001A31 RID: 6705
			Recharge,
			// Token: 0x04001A32 RID: 6706
			NewGame,
			// Token: 0x04001A33 RID: 6707
			RequestData
		}

		// Token: 0x0200087D RID: 2173
		[NetSerializable]
		[Serializable]
		public enum SpaceVillainArcadeVisualState
		{
			// Token: 0x04001A35 RID: 6709
			Normal,
			// Token: 0x04001A36 RID: 6710
			Off,
			// Token: 0x04001A37 RID: 6711
			Broken,
			// Token: 0x04001A38 RID: 6712
			Win,
			// Token: 0x04001A39 RID: 6713
			GameOver
		}

		// Token: 0x0200087E RID: 2174
		[NetSerializable]
		[Serializable]
		public enum SpaceVillainArcadeUiKey
		{
			// Token: 0x04001A3B RID: 6715
			Key
		}

		// Token: 0x0200087F RID: 2175
		[NetSerializable]
		[Serializable]
		public sealed class SpaceVillainArcadePlayerActionMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019E8 RID: 6632 RVA: 0x00051A4D File Offset: 0x0004FC4D
			public SpaceVillainArcadePlayerActionMessage(SharedSpaceVillainArcadeComponent.PlayerAction playerAction)
			{
				this.PlayerAction = playerAction;
			}

			// Token: 0x04001A3C RID: 6716
			public readonly SharedSpaceVillainArcadeComponent.PlayerAction PlayerAction;
		}

		// Token: 0x02000880 RID: 2176
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class SpaceVillainArcadeMetaDataUpdateMessage : SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage
		{
			// Token: 0x060019E9 RID: 6633 RVA: 0x00051A5C File Offset: 0x0004FC5C
			public SpaceVillainArcadeMetaDataUpdateMessage(int playerHp, int playerMp, int enemyHp, int enemyMp, string playerActionMessage, string enemyActionMessage, string gameTitle, string enemyName, bool buttonsDisabled) : base(playerHp, playerMp, enemyHp, enemyMp, playerActionMessage, enemyActionMessage)
			{
				this.GameTitle = gameTitle;
				this.EnemyName = enemyName;
				this.ButtonsDisabled = buttonsDisabled;
			}

			// Token: 0x04001A3D RID: 6717
			public readonly string GameTitle;

			// Token: 0x04001A3E RID: 6718
			public readonly string EnemyName;

			// Token: 0x04001A3F RID: 6719
			public readonly bool ButtonsDisabled;
		}

		// Token: 0x02000881 RID: 2177
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Virtual]
		[Serializable]
		public class SpaceVillainArcadeDataUpdateMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060019EA RID: 6634 RVA: 0x00051A85 File Offset: 0x0004FC85
			public SpaceVillainArcadeDataUpdateMessage(int playerHp, int playerMp, int enemyHp, int enemyMp, string playerActionMessage, string enemyActionMessage)
			{
				this.PlayerHP = playerHp;
				this.PlayerMP = playerMp;
				this.EnemyHP = enemyHp;
				this.EnemyMP = enemyMp;
				this.EnemyActionMessage = enemyActionMessage;
				this.PlayerActionMessage = playerActionMessage;
			}

			// Token: 0x04001A40 RID: 6720
			public readonly int PlayerHP;

			// Token: 0x04001A41 RID: 6721
			public readonly int PlayerMP;

			// Token: 0x04001A42 RID: 6722
			public readonly int EnemyHP;

			// Token: 0x04001A43 RID: 6723
			public readonly int EnemyMP;

			// Token: 0x04001A44 RID: 6724
			public readonly string PlayerActionMessage;

			// Token: 0x04001A45 RID: 6725
			public readonly string EnemyActionMessage;
		}
	}
}
