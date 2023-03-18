using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Administration
{
	// Token: 0x0200073B RID: 1851
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NetworkResourceUploadMessage : NetMessage
	{
		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06001666 RID: 5734 RVA: 0x000493D4 File Offset: 0x000475D4
		public override NetDeliveryMethod DeliveryMethod
		{
			get
			{
				return 34;
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06001667 RID: 5735 RVA: 0x000493D8 File Offset: 0x000475D8
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06001668 RID: 5736 RVA: 0x000493DB File Offset: 0x000475DB
		// (set) Token: 0x06001669 RID: 5737 RVA: 0x000493E3 File Offset: 0x000475E3
		public byte[] Data { get; set; } = Array.Empty<byte>();

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x0600166A RID: 5738 RVA: 0x000493EC File Offset: 0x000475EC
		// (set) Token: 0x0600166B RID: 5739 RVA: 0x000493F4 File Offset: 0x000475F4
		public ResourcePath RelativePath { get; set; } = ResourcePath.Self;

		// Token: 0x0600166C RID: 5740 RVA: 0x000493FD File Offset: 0x000475FD
		public NetworkResourceUploadMessage()
		{
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x0004941B File Offset: 0x0004761B
		public NetworkResourceUploadMessage(byte[] data, ResourcePath relativePath)
		{
			this.Data = data;
			this.RelativePath = relativePath;
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x00049448 File Offset: 0x00047648
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			int dataLength = buffer.ReadVariableInt32();
			this.Data = buffer.ReadBytes(dataLength);
			this.RelativePath = new ResourcePath(buffer.ReadString(), buffer.ReadString());
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x00049480 File Offset: 0x00047680
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.WriteVariableInt32(this.Data.Length);
			buffer.Write(this.Data);
			buffer.Write(this.RelativePath.ToString());
			buffer.Write(this.RelativePath.Separator);
		}
	}
}
