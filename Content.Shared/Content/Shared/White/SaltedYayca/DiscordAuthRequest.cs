using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.White.SaltedYayca
{
	// Token: 0x02000033 RID: 51
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiscordAuthRequest : NetMessage
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002D29 File Offset: 0x00000F29
		public override NetDeliveryMethod DeliveryMethod
		{
			get
			{
				return 34;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002D2D File Offset: 0x00000F2D
		public override MsgGroups MsgGroup
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002D30 File Offset: 0x00000F30
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002D32 File Offset: 0x00000F32
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
		}
	}
}
