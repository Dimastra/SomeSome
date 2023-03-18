using System;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles
{
	// Token: 0x0200045E RID: 1118
	[NetSerializable]
	[Serializable]
	public sealed class MakeGhostRoleEuiState : EuiStateBase
	{
		// Token: 0x06000D97 RID: 3479 RVA: 0x0002C981 File Offset: 0x0002AB81
		public MakeGhostRoleEuiState(EntityUid entityUid)
		{
			this.EntityUid = entityUid;
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000D98 RID: 3480 RVA: 0x0002C990 File Offset: 0x0002AB90
		public EntityUid EntityUid { get; }
	}
}
