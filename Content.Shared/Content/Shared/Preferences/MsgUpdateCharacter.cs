using System;
using System.IO;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
	// Token: 0x0200024C RID: 588
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgUpdateCharacter : NetMessage
	{
		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060006D9 RID: 1753 RVA: 0x000180A4 File Offset: 0x000162A4
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x000180A8 File Offset: 0x000162A8
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			this.Slot = buffer.ReadInt32();
			int length = buffer.ReadVariableInt32();
			using (MemoryStream stream = NetMessageExt.ReadAlignedMemory(buffer, length))
			{
				this.Profile = serializer.Deserialize<ICharacterProfile>(stream);
			}
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x000180FC File Offset: 0x000162FC
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.Write(this.Slot);
			using (MemoryStream stream = new MemoryStream())
			{
				serializer.Serialize(stream, this.Profile);
				buffer.WriteVariableInt32((int)stream.Length);
				ArraySegment<byte> segment;
				stream.TryGetBuffer(out segment);
				buffer.Write(segment);
			}
		}

		// Token: 0x0400069E RID: 1694
		public int Slot;

		// Token: 0x0400069F RID: 1695
		public ICharacterProfile Profile;
	}
}
