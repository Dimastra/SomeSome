using System;
using System.Runtime.CompilerServices;
using Content.Server.Electrocution;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x0200060F RID: 1551
	[DataDefinition]
	public sealed class AttemptElectrocute : IGraphAction
	{
		// Token: 0x06002136 RID: 8502 RVA: 0x000ADBBC File Offset: 0x000ABDBC
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (userUid == null)
			{
				return;
			}
			entityManager.EntitySysManager.GetEntitySystem<ElectrocutionSystem>().TryDoElectrifiedAct(uid, userUid.Value, 1f, null, null, null);
		}
	}
}
