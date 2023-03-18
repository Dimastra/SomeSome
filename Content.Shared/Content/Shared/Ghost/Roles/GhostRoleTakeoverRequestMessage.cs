using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles
{
	// Token: 0x0200045B RID: 1115
	[NetSerializable]
	[Serializable]
	public sealed class GhostRoleTakeoverRequestMessage : EuiMessageBase
	{
		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000D92 RID: 3474 RVA: 0x0002C94B File Offset: 0x0002AB4B
		public uint Identifier { get; }

		// Token: 0x06000D93 RID: 3475 RVA: 0x0002C953 File Offset: 0x0002AB53
		public GhostRoleTakeoverRequestMessage(uint identifier)
		{
			this.Identifier = identifier;
		}
	}
}
