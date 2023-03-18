using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
	// Token: 0x02000249 RID: 585
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgDeleteCharacter : NetMessage
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x00017F1B File Offset: 0x0001611B
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x00017F1E File Offset: 0x0001611E
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.Slot = buffer.ReadInt32();
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00017F2C File Offset: 0x0001612C
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.Write(this.Slot);
		}

		// Token: 0x0400069A RID: 1690
		public int Slot;
	}
}
