using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
	// Token: 0x0200024B RID: 587
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgSelectCharacter : NetMessage
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x0001807C File Offset: 0x0001627C
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x0001807F File Offset: 0x0001627F
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.SelectedCharacterIndex = buffer.ReadVariableInt32();
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0001808D File Offset: 0x0001628D
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.WriteVariableInt32(this.SelectedCharacterIndex);
		}

		// Token: 0x0400069D RID: 1693
		public int SelectedCharacterIndex;
	}
}
