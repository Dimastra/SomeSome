using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Voting
{
	// Token: 0x02000080 RID: 128
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgVoteMenu : NetMessage
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00008DDA File Offset: 0x00006FDA
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008DDD File Offset: 0x00006FDD
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008DDF File Offset: 0x00006FDF
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000188 RID: 392 RVA: 0x00008DE1 File Offset: 0x00006FE1
		public override NetDeliveryMethod DeliveryMethod
		{
			get
			{
				return 34;
			}
		}
	}
}
