using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Animations;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Client.Storage
{
	// Token: 0x02000123 RID: 291
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ClientStorageComponent : SharedStorageComponent
	{
		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060007FE RID: 2046 RVA: 0x0002E8AF File Offset: 0x0002CAAF
		public override IReadOnlyList<EntityUid> StoredEntities
		{
			get
			{
				return this._storedEntities;
			}
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x0002E8B8 File Offset: 0x0002CAB8
		public void HandleAnimatingInsertingEntities(AnimateInsertingEntitiesEvent msg)
		{
			int num = 0;
			while (msg.StoredEntities.Count > num)
			{
				EntityUid entityUid = msg.StoredEntities[num];
				EntityCoordinates initialPosition = msg.EntityPositions[num];
				if (this._entityManager.EntityExists(entityUid))
				{
					ReusableAnimations.AnimateEntityPickup(entityUid, initialPosition, this._entityManager.GetComponent<TransformComponent>(base.Owner).LocalPosition, this._entityManager);
				}
				num++;
			}
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x00003C59 File Offset: 0x00001E59
		public override bool Remove(EntityUid entity)
		{
			return false;
		}

		// Token: 0x0400040E RID: 1038
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x0400040F RID: 1039
		private List<EntityUid> _storedEntities = new List<EntityUid>();
	}
}
