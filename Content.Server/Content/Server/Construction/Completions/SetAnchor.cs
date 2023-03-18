using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000621 RID: 1569
	[DataDefinition]
	public sealed class SetAnchor : IGraphAction
	{
		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06002170 RID: 8560 RVA: 0x000AEAC1 File Offset: 0x000ACCC1
		// (set) Token: 0x06002171 RID: 8561 RVA: 0x000AEAC9 File Offset: 0x000ACCC9
		[DataField("value", false, 1, false, false, null)]
		public bool Value { get; private set; } = true;

		// Token: 0x06002172 RID: 8562 RVA: 0x000AEAD2 File Offset: 0x000ACCD2
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			entityManager.GetComponent<TransformComponent>(uid).Anchored = this.Value;
		}
	}
}
