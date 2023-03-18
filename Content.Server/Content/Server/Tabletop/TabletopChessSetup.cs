using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Tabletop
{
	// Token: 0x0200012D RID: 301
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TabletopChessSetup : TabletopSetup
	{
		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x0001AD94 File Offset: 0x00018F94
		[DataField("boardPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ChessBoardPrototype { get; } = "ChessBoardTabletop";

		// Token: 0x06000572 RID: 1394 RVA: 0x0001AD9C File Offset: 0x00018F9C
		public override void SetupTabletop(TabletopSession session, IEntityManager entityManager)
		{
			EntityUid chessboard = entityManager.SpawnEntity(this.ChessBoardPrototype, session.Position.Offset(-1f, 0f));
			session.Entities.Add(chessboard);
			this.SpawnPieces(session, entityManager, session.Position.Offset(-4.5f, 3.5f), 1f);
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x0001ADFC File Offset: 0x00018FFC
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
			this.SpawnPiecesRow(session, entityManager, "Black", topLeft, separation);
			this.SpawnPawns(session, entityManager, "Black", new MapCoordinates(x, y - separation, mapId), separation);
			this.SpawnPawns(session, entityManager, "White", new MapCoordinates(x, y - 6f * separation, mapId), separation);
			this.SpawnPiecesRow(session, entityManager, "White", new MapCoordinates(x, y - 7f * separation, mapId), separation);
			EntityUid tempQualifier = entityManager.SpawnEntity("BlackQueen", new MapCoordinates(x + 9f * separation + 0.28125f, y - 3f * separation, mapId));
			session.Entities.Add(tempQualifier);
			EntityUid tempQualifier2 = entityManager.SpawnEntity("WhiteQueen", new MapCoordinates(x + 9f * separation + 0.28125f, y - 4f * separation, mapId));
			session.Entities.Add(tempQualifier2);
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x0001AF00 File Offset: 0x00019100
		private void SpawnPiecesRow(TabletopSession session, IEntityManager entityManager, string color, MapCoordinates left, float separation = 1f)
		{
			MapCoordinates mapCoordinates = left;
			MapId mapId2;
			float num;
			float num2;
			mapCoordinates.Deconstruct(ref mapId2, ref num, ref num2);
			MapId mapId = mapId2;
			float x = num;
			float y = num2;
			for (int i = 0; i < 8; i++)
			{
				char c = "rnbqkbnr"[i];
				if (c != 'b')
				{
					if (c != 'k')
					{
						switch (c)
						{
						case 'n':
						{
							EntityUid tempQualifier = entityManager.SpawnEntity(color + "Knight", new MapCoordinates(x + (float)i * separation, y, mapId));
							session.Entities.Add(tempQualifier);
							break;
						}
						case 'q':
						{
							EntityUid tempQualifier2 = entityManager.SpawnEntity(color + "Queen", new MapCoordinates(x + (float)i * separation, y, mapId));
							session.Entities.Add(tempQualifier2);
							break;
						}
						case 'r':
						{
							EntityUid tempQualifier3 = entityManager.SpawnEntity(color + "Rook", new MapCoordinates(x + (float)i * separation, y, mapId));
							session.Entities.Add(tempQualifier3);
							break;
						}
						}
					}
					else
					{
						EntityUid tempQualifier4 = entityManager.SpawnEntity(color + "King", new MapCoordinates(x + (float)i * separation, y, mapId));
						session.Entities.Add(tempQualifier4);
					}
				}
				else
				{
					EntityUid tempQualifier5 = entityManager.SpawnEntity(color + "Bishop", new MapCoordinates(x + (float)i * separation, y, mapId));
					session.Entities.Add(tempQualifier5);
				}
			}
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x0001B080 File Offset: 0x00019280
		private void SpawnPawns(TabletopSession session, IEntityManager entityManager, string color, MapCoordinates left, float separation = 1f)
		{
			MapCoordinates mapCoordinates = left;
			MapId mapId2;
			float num;
			float num2;
			mapCoordinates.Deconstruct(ref mapId2, ref num, ref num2);
			MapId mapId = mapId2;
			float x = num;
			float y = num2;
			for (int i = 0; i < 8; i++)
			{
				EntityUid tempQualifier = entityManager.SpawnEntity(color + "Pawn", new MapCoordinates(x + (float)i * separation, y, mapId));
				session.Entities.Add(tempQualifier);
			}
		}
	}
}
