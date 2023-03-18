using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Info
{
	// Token: 0x020003EC RID: 1004
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedRulesManager
	{
		// Token: 0x020007F3 RID: 2035
		[Nullable(0)]
		public sealed class ShowRulesPopupMessage : NetMessage
		{
			// Token: 0x17000502 RID: 1282
			// (get) Token: 0x06001886 RID: 6278 RVA: 0x0004E380 File Offset: 0x0004C580
			public override MsgGroups MsgGroup
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x17000503 RID: 1283
			// (get) Token: 0x06001887 RID: 6279 RVA: 0x0004E383 File Offset: 0x0004C583
			// (set) Token: 0x06001888 RID: 6280 RVA: 0x0004E38B File Offset: 0x0004C58B
			public float PopupTime { get; set; }

			// Token: 0x06001889 RID: 6281 RVA: 0x0004E394 File Offset: 0x0004C594
			public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
			{
				this.PopupTime = buffer.ReadFloat();
			}

			// Token: 0x0600188A RID: 6282 RVA: 0x0004E3A2 File Offset: 0x0004C5A2
			public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
			{
				buffer.Write(this.PopupTime);
			}
		}

		// Token: 0x020007F4 RID: 2036
		[Nullable(0)]
		public sealed class ShouldShowRulesPopupMessage : NetMessage
		{
			// Token: 0x17000504 RID: 1284
			// (get) Token: 0x0600188C RID: 6284 RVA: 0x0004E3B8 File Offset: 0x0004C5B8
			public override MsgGroups MsgGroup
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x0600188D RID: 6285 RVA: 0x0004E3BB File Offset: 0x0004C5BB
			public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
			{
			}

			// Token: 0x0600188E RID: 6286 RVA: 0x0004E3BD File Offset: 0x0004C5BD
			public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
			{
			}
		}

		// Token: 0x020007F5 RID: 2037
		[Nullable(0)]
		public sealed class RulesAcceptedMessage : NetMessage
		{
			// Token: 0x17000505 RID: 1285
			// (get) Token: 0x06001890 RID: 6288 RVA: 0x0004E3C7 File Offset: 0x0004C5C7
			public override MsgGroups MsgGroup
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x06001891 RID: 6289 RVA: 0x0004E3CA File Offset: 0x0004C5CA
			public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
			{
			}

			// Token: 0x06001892 RID: 6290 RVA: 0x0004E3CC File Offset: 0x0004C5CC
			public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
			{
			}
		}
	}
}
