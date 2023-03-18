using System;
using System.IO;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Eui
{
	// Token: 0x020004B5 RID: 1205
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgEuiState : NetMessage
	{
		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06000E93 RID: 3731 RVA: 0x0002F064 File Offset: 0x0002D264
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x0002F068 File Offset: 0x0002D268
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer ser)
		{
			this.Id = buffer.ReadUInt32();
			int len = buffer.ReadVariableInt32();
			MemoryStream stream = NetMessageExt.ReadAlignedMemory(buffer, len);
			this.State = ser.Deserialize<EuiStateBase>(stream);
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x0002F0A0 File Offset: 0x0002D2A0
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer ser)
		{
			buffer.Write(this.Id);
			MemoryStream stream = new MemoryStream();
			ser.Serialize(stream, this.State);
			int length = (int)stream.Length;
			buffer.WriteVariableInt32(length);
			buffer.Write(stream.GetBuffer().AsSpan(0, length));
		}

		// Token: 0x04000DBC RID: 3516
		public uint Id;

		// Token: 0x04000DBD RID: 3517
		public EuiStateBase State;
	}
}
