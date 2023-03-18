using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles
{
	// Token: 0x0200045A RID: 1114
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GhostRolesEuiState : EuiStateBase
	{
		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000D90 RID: 3472 RVA: 0x0002C934 File Offset: 0x0002AB34
		public GhostRoleInfo[] GhostRoles { get; }

		// Token: 0x06000D91 RID: 3473 RVA: 0x0002C93C File Offset: 0x0002AB3C
		public GhostRolesEuiState(GhostRoleInfo[] ghostRoles)
		{
			this.GhostRoles = ghostRoles;
		}
	}
}
