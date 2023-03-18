using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x02000737 RID: 1847
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GamePrototypeLoadMessage : NetMessage
	{
		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06001658 RID: 5720 RVA: 0x0004925F File Offset: 0x0004745F
		public override MsgGroups MsgGroup
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06001659 RID: 5721 RVA: 0x00049262 File Offset: 0x00047462
		// (set) Token: 0x0600165A RID: 5722 RVA: 0x0004926A File Offset: 0x0004746A
		public string PrototypeData { get; set; } = string.Empty;

		// Token: 0x0600165B RID: 5723 RVA: 0x00049273 File Offset: 0x00047473
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.PrototypeData = buffer.ReadString();
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x00049281 File Offset: 0x00047481
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.Write(this.PrototypeData);
		}
	}
}
