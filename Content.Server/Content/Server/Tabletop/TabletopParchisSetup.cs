using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Tabletop
{
	// Token: 0x0200012E RID: 302
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TabletopParchisSetup : TabletopSetup
	{
		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000577 RID: 1399 RVA: 0x0001B0F9 File Offset: 0x000192F9
		[DataField("boardPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ParchisBoardPrototype { get; } = "ParchisBoardTabletop";

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x0001B101 File Offset: 0x00019301
		[DataField("redPiecePrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string RedPiecePrototype { get; } = "RedTabletopPiece";

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000579 RID: 1401 RVA: 0x0001B109 File Offset: 0x00019309
		[DataField("greenPiecePrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string GreenPiecePrototype { get; } = "GreenTabletopPiece";

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x0001B111 File Offset: 0x00019311
		[DataField("yellowPiecePrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string YellowPiecePrototype { get; } = "YellowTabletopPiece";

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x0001B119 File Offset: 0x00019319
		[DataField("bluePiecePrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string BluePiecePrototype { get; } = "BlueTabletopPiece";

		// Token: 0x0600057C RID: 1404 RVA: 0x0001B124 File Offset: 0x00019324
		public override void SetupTabletop(TabletopSession session, IEntityManager entityManager)
		{
			entityManager.SpawnEntity(this.ParchisBoardPrototype, session.Position);
			MapCoordinates center = session.Position;
			EntityUid tempQualifier = entityManager.SpawnEntity(this.RedPiecePrototype, center.Offset(-6.25f, -6.25f));
			session.Entities.Add(tempQualifier);
			EntityUid tempQualifier2 = entityManager.SpawnEntity(this.RedPiecePrototype, center.Offset(-6.25f, -4.25f));
			session.Entities.Add(tempQualifier2);
			EntityUid tempQualifier3 = entityManager.SpawnEntity(this.RedPiecePrototype, center.Offset(-4.25f, -6.25f));
			session.Entities.Add(tempQualifier3);
			EntityUid tempQualifier4 = entityManager.SpawnEntity(this.RedPiecePrototype, center.Offset(-4.25f, -4.25f));
			session.Entities.Add(tempQualifier4);
			EntityUid tempQualifier5 = entityManager.SpawnEntity(this.GreenPiecePrototype, center.Offset(6.25f, -6.25f));
			session.Entities.Add(tempQualifier5);
			EntityUid tempQualifier6 = entityManager.SpawnEntity(this.GreenPiecePrototype, center.Offset(6.25f, -4.25f));
			session.Entities.Add(tempQualifier6);
			EntityUid tempQualifier7 = entityManager.SpawnEntity(this.GreenPiecePrototype, center.Offset(4.25f, -6.25f));
			session.Entities.Add(tempQualifier7);
			EntityUid tempQualifier8 = entityManager.SpawnEntity(this.GreenPiecePrototype, center.Offset(4.25f, -4.25f));
			session.Entities.Add(tempQualifier8);
			EntityUid tempQualifier9 = entityManager.SpawnEntity(this.YellowPiecePrototype, center.Offset(6.25f, 6.25f));
			session.Entities.Add(tempQualifier9);
			EntityUid tempQualifier10 = entityManager.SpawnEntity(this.YellowPiecePrototype, center.Offset(6.25f, 4.25f));
			session.Entities.Add(tempQualifier10);
			EntityUid tempQualifier11 = entityManager.SpawnEntity(this.YellowPiecePrototype, center.Offset(4.25f, 6.25f));
			session.Entities.Add(tempQualifier11);
			EntityUid tempQualifier12 = entityManager.SpawnEntity(this.YellowPiecePrototype, center.Offset(4.25f, 4.25f));
			session.Entities.Add(tempQualifier12);
			EntityUid tempQualifier13 = entityManager.SpawnEntity(this.BluePiecePrototype, center.Offset(-6.25f, 6.25f));
			session.Entities.Add(tempQualifier13);
			EntityUid tempQualifier14 = entityManager.SpawnEntity(this.BluePiecePrototype, center.Offset(-6.25f, 4.25f));
			session.Entities.Add(tempQualifier14);
			EntityUid tempQualifier15 = entityManager.SpawnEntity(this.BluePiecePrototype, center.Offset(-4.25f, 6.25f));
			session.Entities.Add(tempQualifier15);
			EntityUid tempQualifier16 = entityManager.SpawnEntity(this.BluePiecePrototype, center.Offset(-4.25f, 4.25f));
			session.Entities.Add(tempQualifier16);
		}
	}
}
