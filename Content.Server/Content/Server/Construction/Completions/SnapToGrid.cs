using System;
using System.Runtime.CompilerServices;
using Content.Server.Coordinates.Helpers;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000623 RID: 1571
	[DataDefinition]
	public sealed class SnapToGrid : IGraphAction
	{
		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06002177 RID: 8567 RVA: 0x000AEB26 File Offset: 0x000ACD26
		// (set) Token: 0x06002178 RID: 8568 RVA: 0x000AEB2E File Offset: 0x000ACD2E
		[DataField("southRotation", false, 1, false, false, null)]
		public bool SouthRotation { get; private set; }

		// Token: 0x06002179 RID: 8569 RVA: 0x000AEB38 File Offset: 0x000ACD38
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(uid);
			if (!transform.Anchored)
			{
				transform.Coordinates = transform.Coordinates.SnapToGrid(entityManager, null);
			}
			if (this.SouthRotation)
			{
				transform.LocalRotation = Angle.Zero;
			}
		}
	}
}
