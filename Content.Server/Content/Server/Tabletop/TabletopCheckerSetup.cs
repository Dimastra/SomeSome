using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Tabletop
{
	// Token: 0x0200012C RID: 300
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TabletopCheckerSetup : TabletopSetup
	{
		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x0001AB92 File Offset: 0x00018D92
		[DataField("boardPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string CheckerBoardPrototype { get; } = "CheckerBoardTabletop";

		// Token: 0x0600056D RID: 1389 RVA: 0x0001AB9C File Offset: 0x00018D9C
		public override void SetupTabletop(TabletopSession session, IEntityManager entityManager)
		{
			EntityUid checkerboard = entityManager.SpawnEntity(this.CheckerBoardPrototype, session.Position.Offset(-1f, 0f));
			session.Entities.Add(checkerboard);
			this.SpawnPieces(session, entityManager, session.Position.Offset(-4.5f, 3.5f), 1f);
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x0001ABFC File Offset: 0x00018DFC
		private void SpawnPieces(TabletopSession session, IEntityManager entityManager, MapCoordinates topLeft, float separation = 1f)
		{
			MapCoordinates mapCoordinates = topLeft;
			MapId mapId2;
			float num;
			float num2;
			mapCoordinates.Deconstruct(ref mapId2, ref num, ref num2);
			MapId mapId = mapId2;
			float x = num;
			float y = num2;
			this.SpawnPieces(session, entityManager, "Black", topLeft, separation);
			this.SpawnPieces(session, entityManager, "White", new MapCoordinates(x, y - 7f * separation, mapId), separation);
			for (int i = 1; i < 4; i++)
			{
				EntityUid tempQualifier = entityManager.SpawnEntity("BlackCheckerQueen", new MapCoordinates(x + 9f * separation + 0.28125f, y - (float)i * separation, mapId));
				session.Entities.Add(tempQualifier);
				EntityUid tempQualifier2 = entityManager.SpawnEntity("WhiteCheckerQueen", new MapCoordinates(x + 8f * separation + 0.28125f, y - (float)i * separation, mapId));
				session.Entities.Add(tempQualifier2);
			}
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0001ACD4 File Offset: 0x00018ED4
		private void SpawnPieces(TabletopSession session, IEntityManager entityManager, string color, MapCoordinates left, float separation = 1f)
		{
			MapCoordinates mapCoordinates = left;
			MapId mapId2;
			float num;
			float num2;
			mapCoordinates.Deconstruct(ref mapId2, ref num, ref num2);
			MapId mapId = mapId2;
			float x = num;
			float y = num2;
			int reversed = (color == "White") ? 1 : -1;
			for (int i = 0; i < 3; i++)
			{
				int x_offset = i % 2;
				if (reversed == -1)
				{
					x_offset = 1 - x_offset;
				}
				for (int j = 0; j < 8; j += 2)
				{
					if (x_offset + j <= 8)
					{
						EntityUid tempQualifier4 = entityManager.SpawnEntity(color + "CheckerPiece", new MapCoordinates(x + (float)(j + x_offset) * separation, y + (float)(i * reversed) * separation, mapId));
						session.Entities.Add(tempQualifier4);
					}
				}
			}
		}
	}
}
