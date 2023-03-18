using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.GhostKick
{
	// Token: 0x0200044E RID: 1102
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgGhostKick : NetMessage
	{
		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06000D69 RID: 3433 RVA: 0x0002C6FE File Offset: 0x0002A8FE
		public override MsgGroups MsgGroup
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x0002C701 File Offset: 0x0002A901
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x0002C703 File Offset: 0x0002A903
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
		}
	}
}
