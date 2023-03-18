using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.White.JoinQueue
{
	// Token: 0x0200003C RID: 60
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgQueueUpdate : NetMessage
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002DC9 File Offset: 0x00000FC9
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00002DCC File Offset: 0x00000FCC
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00002DD4 File Offset: 0x00000FD4
		public int Total { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00002DDD File Offset: 0x00000FDD
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00002DE5 File Offset: 0x00000FE5
		public int Position { get; set; }

		// Token: 0x06000074 RID: 116 RVA: 0x00002DEE File Offset: 0x00000FEE
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.Total = buffer.ReadInt32();
			this.Position = buffer.ReadInt32();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00002E08 File Offset: 0x00001008
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.Write(this.Total);
			buffer.Write(this.Position);
		}
	}
}
