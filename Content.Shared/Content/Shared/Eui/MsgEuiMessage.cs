using System;
using System.IO;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Eui
{
	// Token: 0x020004B4 RID: 1204
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgEuiMessage : NetMessage
	{
		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06000E8F RID: 3727 RVA: 0x0002EFCD File Offset: 0x0002D1CD
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x0002EFD0 File Offset: 0x0002D1D0
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer ser)
		{
			this.Id = buffer.ReadUInt32();
			int len = buffer.ReadVariableInt32();
			MemoryStream stream = NetMessageExt.ReadAlignedMemory(buffer, len);
			this.Message = ser.Deserialize<EuiMessageBase>(stream);
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x0002F008 File Offset: 0x0002D208
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer ser)
		{
			buffer.Write(this.Id);
			MemoryStream stream = new MemoryStream();
			ser.Serialize(stream, this.Message);
			int length = (int)stream.Length;
			buffer.WriteVariableInt32(length);
			buffer.Write(stream.GetBuffer().AsSpan(0, length));
		}

		// Token: 0x04000DBA RID: 3514
		public uint Id;

		// Token: 0x04000DBB RID: 3515
		public EuiMessageBase Message;
	}
}
