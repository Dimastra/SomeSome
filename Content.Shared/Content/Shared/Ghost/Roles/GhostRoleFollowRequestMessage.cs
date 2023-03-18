using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles
{
	// Token: 0x0200045C RID: 1116
	[NetSerializable]
	[Serializable]
	public sealed class GhostRoleFollowRequestMessage : EuiMessageBase
	{
		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000D94 RID: 3476 RVA: 0x0002C962 File Offset: 0x0002AB62
		public uint Identifier { get; }

		// Token: 0x06000D95 RID: 3477 RVA: 0x0002C96A File Offset: 0x0002AB6A
		public GhostRoleFollowRequestMessage(uint identifier)
		{
			this.Identifier = identifier;
		}
	}
}
