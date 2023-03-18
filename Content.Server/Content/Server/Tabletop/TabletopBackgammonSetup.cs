using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Tabletop
{
	// Token: 0x0200012B RID: 299
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TabletopBackgammonSetup : TabletopSetup
	{
		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x0001A9D1 File Offset: 0x00018BD1
		[DataField("boardPrototype", false, 1, false, false, null)]
		public string BackgammonBoardPrototype { get; } = "BackgammonBoardTabletop";

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000565 RID: 1381 RVA: 0x0001A9D9 File Offset: 0x00018BD9
		[DataField("whitePiecePrototype", false, 1, false, false, null)]
		public string WhitePiecePrototype { get; } = "WhiteTabletopPiece";

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x0001A9E1 File Offset: 0x00018BE1
		[DataField("blackPiecePrototype", false, 1, false, false, null)]
		public string BlackPiecePrototype { get; } = "BlackTabletopPiece";

		// Token: 0x06000567 RID: 1383 RVA: 0x0001A9EC File Offset: 0x00018BEC
		public override void SetupTabletop(TabletopSession session, IEntityManager entityManager)
		{
			TabletopBackgammonSetup.<>c__DisplayClass9_0 CS$<>8__locals1;
			CS$<>8__locals1.session = session;
			CS$<>8__locals1.entityManager = entityManager;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.entityManager.SpawnEntity(this.BackgammonBoardPrototype, CS$<>8__locals1.session.Position);
			this.<SetupTabletop>g__AddPieces|9_2(0f, 5, true, true, true, ref CS$<>8__locals1);
			this.<SetupTabletop>g__AddPieces|9_2(4f, 3, false, true, true, ref CS$<>8__locals1);
			this.<SetupTabletop>g__AddPieces|9_2(5f, 5, false, true, false, ref CS$<>8__locals1);
			this.<SetupTabletop>g__AddPieces|9_2(0f, 2, true, true, false, ref CS$<>8__locals1);
			this.<SetupTabletop>g__AddPieces|9_2(0f, 5, false, false, true, ref CS$<>8__locals1);
			this.<SetupTabletop>g__AddPieces|9_2(4f, 3, true, false, true, ref CS$<>8__locals1);
			this.<SetupTabletop>g__AddPieces|9_2(5f, 5, true, false, false, ref CS$<>8__locals1);
			this.<SetupTabletop>g__AddPieces|9_2(0f, 2, false, false, false, ref CS$<>8__locals1);
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x0001AAE0 File Offset: 0x00018CE0
		[CompilerGenerated]
		internal static float <SetupTabletop>g__GetXPosition|9_0(float distanceFromSide, bool isLeftSide)
		{
			float pos = 7.35f - distanceFromSide * 1.25f;
			if (!isLeftSide)
			{
				return pos;
			}
			return -pos;
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0001AB04 File Offset: 0x00018D04
		[CompilerGenerated]
		internal static float <SetupTabletop>g__GetYPosition|9_1(float positionNumber, bool isTop)
		{
			float pos = 5.6f - 0.8f * positionNumber;
			if (!isTop)
			{
				return -pos;
			}
			return pos;
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001AB28 File Offset: 0x00018D28
		[CompilerGenerated]
		private void <SetupTabletop>g__AddPieces|9_2(float distanceFromSide, int numberOfPieces, bool isBlackPiece, bool isTop, bool isLeftSide, ref TabletopBackgammonSetup.<>c__DisplayClass9_0 A_6)
		{
			for (int i = 0; i < numberOfPieces; i++)
			{
				A_6.session.Entities.Add(A_6.entityManager.SpawnEntity(isBlackPiece ? this.BlackPiecePrototype : this.WhitePiecePrototype, A_6.session.Position.Offset(TabletopBackgammonSetup.<SetupTabletop>g__GetXPosition|9_0(distanceFromSide, isLeftSide), TabletopBackgammonSetup.<SetupTabletop>g__GetYPosition|9_1((float)i, isTop))));
			}
		}
	}
}
