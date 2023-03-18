using System;
using System.IO;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Chat
{
	// Token: 0x02000602 RID: 1538
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgChatMessage : NetMessage
	{
		// Token: 0x170003CD RID: 973
		// (get) Token: 0x060012D8 RID: 4824 RVA: 0x0003DC9C File Offset: 0x0003BE9C
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0003DCA0 File Offset: 0x0003BEA0
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			int length = buffer.ReadVariableInt32();
			using (MemoryStream stream = NetMessageExt.ReadAlignedMemory(buffer, length))
			{
				serializer.DeserializeDirect<ChatMessage>(stream, ref this.Message);
			}
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x0003DCE8 File Offset: 0x0003BEE8
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			MemoryStream stream = new MemoryStream();
			serializer.SerializeDirect<ChatMessage>(stream, this.Message);
			buffer.WriteVariableInt32((int)stream.Length);
			buffer.Write(StreamExt.AsSpan(stream));
		}

		// Token: 0x0400118B RID: 4491
		public ChatMessage Message;
	}
}
