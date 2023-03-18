using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.White.SaltedYayca
{
	// Token: 0x02000034 RID: 52
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiscordAuthResponse : NetMessage
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00002D3C File Offset: 0x00000F3C
		public override NetDeliveryMethod DeliveryMethod
		{
			get
			{
				return 34;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00002D40 File Offset: 0x00000F40
		public override MsgGroups MsgGroup
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002D43 File Offset: 0x00000F43
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.Uri = buffer.ReadString();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002D51 File Offset: 0x00000F51
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.Write(this.Uri);
		}

		// Token: 0x0400009A RID: 154
		public string Uri = string.Empty;
	}
}
