using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Eui
{
	// Token: 0x020004B3 RID: 1203
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgEuiCtl : NetMessage
	{
		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06000E8B RID: 3723 RVA: 0x0002EF59 File Offset: 0x0002D159
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x0002EF5C File Offset: 0x0002D15C
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.Id = buffer.ReadUInt32();
			this.Type = (MsgEuiCtl.CtlType)buffer.ReadByte();
			if (this.Type == MsgEuiCtl.CtlType.Open)
			{
				this.OpenType = buffer.ReadString();
			}
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x0002EF8B File Offset: 0x0002D18B
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.Write(this.Id);
			buffer.Write((byte)this.Type);
			if (this.Type == MsgEuiCtl.CtlType.Open)
			{
				buffer.Write(this.OpenType);
			}
		}

		// Token: 0x04000DB7 RID: 3511
		public MsgEuiCtl.CtlType Type;

		// Token: 0x04000DB8 RID: 3512
		public string OpenType = string.Empty;

		// Token: 0x04000DB9 RID: 3513
		public uint Id;

		// Token: 0x02000816 RID: 2070
		[NullableContext(0)]
		public enum CtlType : byte
		{
			// Token: 0x040018D2 RID: 6354
			Open,
			// Token: 0x040018D3 RID: 6355
			Close
		}
	}
}
