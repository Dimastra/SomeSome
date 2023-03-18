using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost
{
	// Token: 0x02000457 RID: 1111
	[NetSerializable]
	[Serializable]
	public sealed class GhostUpdateGhostRoleCountEvent : EntityEventArgs
	{
		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000D85 RID: 3461 RVA: 0x0002C8D1 File Offset: 0x0002AAD1
		public int AvailableGhostRoles { get; }

		// Token: 0x06000D86 RID: 3462 RVA: 0x0002C8D9 File Offset: 0x0002AAD9
		public GhostUpdateGhostRoleCountEvent(int availableGhostRoleCount)
		{
			this.AvailableGhostRoles = availableGhostRoleCount;
		}
	}
}
