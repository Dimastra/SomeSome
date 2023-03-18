using System;
using System.IO;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.White.Sponsors
{
	// Token: 0x02000032 RID: 50
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgSponsorInfo : NetMessage
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00002C6B File Offset: 0x00000E6B
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002C70 File Offset: 0x00000E70
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			bool flag = buffer.ReadBoolean();
			buffer.ReadPadBits();
			if (!flag)
			{
				return;
			}
			int length = buffer.ReadVariableInt32();
			using (MemoryStream stream = NetMessageExt.ReadAlignedMemory(buffer, length))
			{
				serializer.DeserializeDirect<SponsorInfo>(stream, ref this.Info);
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002CC4 File Offset: 0x00000EC4
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.Write(this.Info != null);
			buffer.WritePadBits();
			if (this.Info == null)
			{
				return;
			}
			MemoryStream stream = new MemoryStream();
			serializer.SerializeDirect<SponsorInfo>(stream, this.Info);
			buffer.WriteVariableInt32((int)stream.Length);
			buffer.Write(StreamExt.AsSpan(stream));
		}

		// Token: 0x04000099 RID: 153
		[Nullable(2)]
		public SponsorInfo Info;
	}
}
